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
        public static Event CurrentEvent { get; set; }
        public static List<Event> events = new List<Event>();
        public const string Version = "0.8.9";

        public const string FtpServer = "ftp://tropicnews.eu/QRApp/";
        public const string FtpLogin = "app.tropicnews.eu";
        public const string FtpPassword = "fCqjHlmqgB54";

        public const string Server = "http://app.tropicnews.eu/QRApp/";

        public const string imgFolder = "Images/";

        #region Saveable
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }
        
        public static async Task<bool> DownloadFileFTP(Page page)
        {
            //await page.DisplayAlert("PushEnabled", (await Microsoft.AppCenter.Push.Push.IsEnabledAsync()).ToString(), "Ok");
            //await DisplayAlert("Now",DateTime.Now.ToString(),"Ok");
            string serverPath = FtpServer + "AppEvents.xml";
            serverPath = "http://app.tropicnews.eu/QRApp/AppEvents.xml";
            WebRequest request = WebRequest.Create(serverPath);
            request.Method = WebRequestMethods.File.DownloadFile;
            //FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverPath);

            //request.KeepAlive = false;
            //request.UsePassive = true;
            //request.UseBinary = true;

            //request.Method = WebRequestMethods.Ftp.DownloadFile;
            //request.Credentials = new NetworkCredential(FtpLogin, FtpPassword);

            // Read the file from the server & write to destination  
            try
            {
                //using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) // Error here
                //return await ProcessFile(response.GetResponseStream(),page);
                using (WebResponse response = request.GetResponse())
                    return await ProcessFile(response.GetResponseStream(), page);
            }
            catch (Exception)
            {
                await page.DisplayAlert("Chyba", "Jste připojení k internetu?", "Ok");
                return false;
            }
        }

        public static async Task<bool> DownloadFile(Page page)
        {
            string serverPath = Server + "AppEvents.xml";
            WebRequest request = WebRequest.Create(serverPath);
            request.Method = WebRequestMethods.File.DownloadFile;

            // Read the file from the server & write to destination  
            try
            {
                using (WebResponse response = request.GetResponse())
                    return await ProcessFile(response.GetResponseStream(), page);
            }
            catch (Exception)
            {
                await page.DisplayAlert("Chyba", "Jste připojení k internetu?", "Ok");
                return false;
            }
        }

        public static Stream DownloadImage(string imagePath)
        {
            //await page.DisplayAlert("PushEnabled", (await Microsoft.AppCenter.Push.Push.IsEnabledAsync()).ToString(), "Ok");
            //await DisplayAlert("Now",DateTime.Now.ToString(),"Ok");

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

        private static async Task<bool> ProcessFile(Stream xml, Page page)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xml);
                XmlNode versionNode = doc.GetElementsByTagName("Version")[0];
                if (versionNode.Attributes["number"].Value != Settings.Version)
                {
                    if (!await page.DisplayAlert("Nová verze aplikace", "Nová verze k dispozici. Chcete přesměrovat na stránky na stažení nové verze?", "Ne", "Ano"))
                    {
                        Device.OpenUri(new Uri(versionNode.Attributes["link"].Value));
                        return false;
                    }
                    //return false;
                }
                foreach (XmlNode node in doc.GetElementsByTagName("Event"))
                {
                    Event event1 = new Event(node);

                    //if (DateTime.Today < event1.To)
                    if (!Settings.FinishedEvents.Split(';').ToList().Contains(event1.Id.ToString()))
                    {
                        events.Add(event1);
                    }
                }
                if (events.Count < 0)
                {
                    await page.DisplayAlert("Žádná soutěž", "V tuto chvíli neběží žádná aktuální soutěž", "Ok");
                    return false;
                }
                Settings.CurrentEvent = events[0];
            }
            catch (Exception ex)
            {
                await page.DisplayAlert("title", ex.Message, "cancel");
                Crashes.TrackError(ex);
                throw;
            }

            return true;
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
