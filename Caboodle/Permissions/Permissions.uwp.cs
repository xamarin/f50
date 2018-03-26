using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Windows.Devices.Geolocation;

namespace Microsoft.Caboodle
{
    internal static partial class Permissions
    {
        const string appManifestFilename = "AppxManifest.xml";
        const string appManifestXmlns = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";

        static Task PlatformEnsureDeclaredAsync(PermissionType permission)
        {
            var uwpCapabilities = permission.ToUWPCapabilities();

            if (uwpCapabilities == null || !uwpCapabilities.Any())
                return Task.CompletedTask;

            var doc = XDocument.Load(appManifestFilename, LoadOptions.None);
            var xname = XNamespace.Get(appManifestXmlns);

            foreach (var cap in uwpCapabilities)
            {
                if (!doc.Root.XPathSelectElements($"//{xname}Capabilities[@Name='{cap}'")?.Any() ?? false)
                    throw new PermissionException($"You need to declare the capability `{cap}` in your AppxManifest.xml file");
            }

            return Task.CompletedTask;
        }

        static Task<PermissionStatus> PlatformCheckStatusAsync(PermissionType permission)
        {
            switch (permission)
            {
                case PermissionType.LocationWhenInUse:
                    return CheckLocationAsync();
            }

            return Task.FromResult(PermissionStatus.Granted);
        }

        static Task<PermissionStatus> PlatformRequestAsync(PermissionType permission) =>
            PlatformCheckStatusAsync(permission);

        static async Task<PermissionStatus> CheckLocationAsync()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();

            switch (accessStatus)
            {
                case GeolocationAccessStatus.Allowed:
                    return PermissionStatus.Granted;
                case GeolocationAccessStatus.Unspecified:
                    return PermissionStatus.Unknown;
            }

            return PermissionStatus.Denied;
        }
    }

    internal static class PermissionTypeExtensions
    {
        internal static string[] ToUWPCapabilities(this PermissionType permissionType)
        {
            switch (permissionType)
            {
                case PermissionType.LocationWhenInUse:
                    return new[] { "location" };
            }

            return null;
        }
    }
}
