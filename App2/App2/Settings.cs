using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;

using System.Text;
using Xamarin.Forms;

namespace App2
{
    public static class Settings
    {
        public static Color BackgroundColor { get { return Color.LightBlue; } }
        public static Event CurrentEvent { get; set; }
        public const string Version = "0.8.9";

        #region Saveable
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private static readonly string SettingsDefault = string.Empty;
        private const string KeyStands = "stands";
        private const string KeyPermission = "permission";
        private const string KeyFinishedEvents = "finishedevents";

        #endregion

        public static string Stands
        {
            get
            {
                return AppSettings.GetValueOrDefault(KeyStands, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(KeyStands, value);
            }
        }

        public static string FinishedEvents
        {
            get
            {
                return AppSettings.GetValueOrDefault(KeyFinishedEvents, SettingsDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue(KeyFinishedEvents, value);
            }
        }

        public static bool Permission
        {
            get
            {
                return AppSettings.GetValueOrDefault(KeyPermission, true);
            }
            set
            {
                AppSettings.AddOrUpdateValue(KeyPermission, value);
            }
        } 

        #endregion
    }
}
