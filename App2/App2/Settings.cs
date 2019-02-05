using App2.Pages;
using Microsoft.AppCenter.Crashes;
using Plugin.Settings;
using Plugin.Settings.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;

namespace App2
{
    public static class Settings
    {
        //Todo: Add proguard.cfg https://docs.microsoft.com/en-us/appcenter/sdk/push/xamarin-android

        public static Color BackgroundColor { get { return Color.LightBlue; } }
        public static Event CurrentEvent { get; set; } = new Event();
        public static List<Event> Events = new List<Event>();
        public const string Version = "0.9.5";

        public const string FtpServer = "ftp://tropicnews.eu/QRApp/";
        public const string FtpLogin = "app.tropicnews.eu";
        public const string FtpPassword = "fCqjHlmqgB54";

        public const string Server = "http://app.tropicnews.eu/QRApp/";
        public const string XmlPath = Server + "AppEvents.xml";
        public const string imgFolder = Server+"Images/";

        public static Stream DownloadImage(string imagePath)
        {
            string serverPath = Server + imgFolder + imagePath;

            WebRequest request = WebRequest.Create(serverPath);

            request.Method = WebRequestMethods.File.DownloadFile;

            // Read the file from the server & write to destination  
            try
            {
                using (WebResponse response = request.GetResponse()) // Error here
                using (Stream stream = response.GetResponseStream())
                    return stream;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static XmlDocument LoadXml(Page page)
        {
            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(XmlPath);
                return doc;
            }
            catch (WebException ex)
            {
                ((LoadPage)page).LoadPageLbl = ex.Message;
                return null;
            }
            catch (Exception ex)
            {
                ((LoadPage)page).LoadPageLbl = ex.Message;
                return null;
            }
            
        }

        public static bool ProcessXml(Page page)
        {
            XmlDocument doc = LoadXml(page);
            if (doc == null)
                return false;
            try
            {
                //foreach (XmlNode node in doc.GetElementsByTagName("Event"))
                //{
                //    Event event1 = new Event(node);

                //    if (DateTime.Now < event1.To)
                //        if (!FinishedEvents.Split(';').ToList().Contains(event1.Id.ToString()))
                //            Events.Add(event1);
                //}
                CurrentEvent = new Event(doc.GetElementsByTagName("Event")[0]);

                //CurrentEvent = Events[0];
            }
            catch (Exception ex)
            {
                page.DisplayAlert("Chyba", ex.Message, "Ok");
                Crashes.TrackError(ex);

            }
            return true;
        }
        public static async Task<bool> CheckVersion(Page page)
        {
            XmlDocument doc = LoadXml(page);
            if (doc == null)
                return true;

            #region Check new version
            XmlNode versionNode = doc.GetElementsByTagName("Version")[0];
            if (versionNode.Attributes["number"].Value != Version)
                if (!await page.DisplayAlert("Nová verze aplikace", "Nová verze k dispozici. Chcete přesměrovat na stránky na stažení nové verze?", "Ne", "Ano"))
                {
                    Device.OpenUri(new Uri(versionNode.Attributes["link"].Value));
                    return true;
                }
            return false;
            #endregion
        }

        public static bool CheckEvents(Page page)
        {
            XmlDocument doc = LoadXml(page);
            if (doc == null)
                return false;

            #region Check if event is available
            int count = 0;
            foreach (XmlNode node in doc.GetElementsByTagName("Event"))
            {
                if (DateTime.Now < DateTime.Parse(node.Attributes["To"].Value))
                    if (!FinishedEvents.Split(';').ToList().Contains(node.Attributes["Id"].Value))
                        count++;
            }
            return count > 0; 
            #endregion
        }



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
        private const string KeyPickerIndex = "pickerindex";

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

        public static int PickerSelectedIndex
        {
            get
            {
                return AppSettings.GetValueOrDefault(KeyPickerIndex, 1);
            }
            set
            {
                AppSettings.AddOrUpdateValue(KeyPickerIndex, value);
            }
        }

        #endregion
    }
}
