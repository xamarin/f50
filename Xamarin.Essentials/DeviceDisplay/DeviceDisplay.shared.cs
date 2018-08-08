﻿using System;

namespace Xamarin.Essentials
{
    public static partial class DeviceDisplay
    {
        static event EventHandler<ScreenMetricsChangedEventArgs> ScreenMetricsChangedInternal;

        static ScreenMetrics currentMetrics;

        public static ScreenMetrics ScreenMetrics => GetScreenMetrics();

        static void SetCurrent()
        {
            var metrics = GetScreenMetrics();
            currentMetrics = new ScreenMetrics(metrics.Width, metrics.Height, metrics.Density, metrics.Orientation, metrics.Rotation);
        }

        public static event EventHandler<ScreenMetricsChangedEventArgs> ScreenMetricsChanged
        {
            add
            {
                var wasRunning = ScreenMetricsChangedInternal != null;

                ScreenMetricsChangedInternal += value;

                if (!wasRunning && ScreenMetricsChangedInternal != null)
                {
                    SetCurrent();
                    StartScreenMetricsListeners();
                }
            }

            remove
            {
                var wasRunning = ScreenMetricsChangedInternal != null;

                ScreenMetricsChangedInternal -= value;

                if (wasRunning && ScreenMetricsChangedInternal == null)
                    StopScreenMetricsListeners();
            }
        }

        static void OnScreenMetricsChanged(ScreenMetrics metrics)
            => OnScreenMetricsChanged(new ScreenMetricsChangedEventArgs(metrics));

        static void OnScreenMetricsChanged(ScreenMetricsChangedEventArgs e)
        {
            if (!currentMetrics.Equals(e.Metrics))
            {
                SetCurrent();
                ScreenMetricsChangedInternal?.Invoke(null, e);
            }
        }
    }

    public class ScreenMetricsChangedEventArgs : EventArgs
    {
        public ScreenMetricsChangedEventArgs(ScreenMetrics metrics)
        {
            Metrics = metrics;
        }

        public ScreenMetrics Metrics { get; }
    }

    [Preserve(AllMembers = true)]
    public struct ScreenMetrics
    {
        internal ScreenMetrics(double width, double height, double density, ScreenOrientation orientation, ScreenRotation rotation)
        {
            Width = width;
            Height = height;
            Density = density;
            Orientation = orientation;
            Rotation = rotation;
        }

        public double Width { get; set; }

        public double Height { get; set; }

        public double Density { get; set; }

        public ScreenOrientation Orientation { get; set; }

        public ScreenRotation Rotation { get; set; }

        public static bool operator ==(ScreenMetrics left, ScreenMetrics right) =>
            Equals(left, right);

        public static bool operator !=(ScreenMetrics left, ScreenMetrics right) =>
            !Equals(left, right);

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (!(obj is ScreenMetrics metrics))
                return false;

            return Equals(metrics);
        }

        public bool Equals(ScreenMetrics other) =>
            Width.Equals(other.Width) &&
            Height.Equals(other.Height) &&
            Density.Equals(other.Density) &&
            Orientation.Equals(other.Orientation) &&
            Rotation.Equals(other.Rotation);

        public override int GetHashCode() =>
            (Height, Width, Density, Orientation, Rotation).GetHashCode();
    }

    public enum ScreenOrientation
    {
        Unknown,

        Portrait,
        Landscape
    }

    public enum ScreenRotation
    {
        Rotation0,
        Rotation90,
        Rotation180,
        Rotation270
    }
}
