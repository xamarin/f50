﻿using System;
using System.Collections.Generic;

namespace Xamarin.Essentials
{
    public static partial class AppActions
    {
        internal static bool PlatformIsSupported
            => throw ExceptionUtils.NotSupportedOrImplementedException;

        static IEnumerable<AppAction> PlatformGetActions() =>
            throw ExceptionUtils.NotSupportedOrImplementedException;

        static void PlatformSetActions(IEnumerable<AppAction> actions) =>
            throw ExceptionUtils.NotSupportedOrImplementedException;
    }
}