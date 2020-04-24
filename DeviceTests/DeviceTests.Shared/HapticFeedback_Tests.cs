﻿using System;
using Xamarin.Essentials;
using Xunit;

namespace DeviceTests
{
    public class HapticFeedback_Tests
    {
        [Fact]
        public void Click()
        {
#if __ANDROID__
            // API 23+ we need user interaction for camera permission
            // can't really test so easily on device.
            if (Platform.HasApiLevel(Android.OS.BuildVersionCodes.M))
                return;
#elif __IOS__
            // TODO: remove this as soon as the test harness can filter
            // the iOS simulator does not emulate a flashlight
            if (DeviceInfo.DeviceType == DeviceType.Virtual && DeviceInfo.Platform == DevicePlatform.iOS)
                return;
#endif

            HapticFeedback.Execute(HapticFeedbackType.Click);
        }

        [Fact]
        public void LongPress()
        {
#if __ANDROID__
            // API 23+ we need user interaction for camera permission
            // can't really test so easily on device.
            if (Platform.HasApiLevel(Android.OS.BuildVersionCodes.M))
                return;
#elif __IOS__
            // TODO: remove this as soon as the test harness can filter
            // the iOS simulator does not emulate a flashlight
            if (DeviceInfo.DeviceType == DeviceType.Virtual && DeviceInfo.Platform == DevicePlatform.iOS)
                return;
#endif

            HapticFeedback.Execute(HapticFeedbackType.LongPress);
        }
    }
}