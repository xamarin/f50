﻿using Xamarin.Essentials;
using Xamarin.Forms;

namespace Samples.ViewModel
{
    public class AppInfoViewModel : BaseViewModel
    {
        public string AppPackageName => AppInfo.PackageName;

        public string AppName => AppInfo.Name;

        public string AppVersion => AppInfo.VersionString;

        public string AppBuild => AppInfo.BuildString;

        public string WindowAppSize => AppInfo.AppViewInfo.ToString();

        public Command ShowSettingsUICommand { get; }

        public AppInfoViewModel()
        {
            ShowSettingsUICommand = new Command(() => AppInfo.ShowSettingsUI());
        }
    }
}
