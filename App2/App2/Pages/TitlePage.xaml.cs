using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class TitlePage : ContentPage
	{
        List<Event> events = new List<Event>();

        public TitlePage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            this.BackgroundColor = Settings.BackgroundColor;
        }

        private async void BtnNext_Clicked(object sender, EventArgs e)
        {
            if (await DownloadFileFTP())
                await Navigation.PushAsync(new MainPage());
        }

        private async Task<bool> DownloadFileFTP()
        {
            string serverPath = "ftp://tropicnews.eu//AppEvents.xml";

            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(serverPath);

            request.KeepAlive = false;
            request.UsePassive = true;
            request.UseBinary = true;

            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential("app.tropicnews.eu", "fCqjHlmqgB54");

            // Read the file from the server & write to destination  
            try
            {
                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse()) // Error here

                    return await ProcessFile(response.GetResponseStream());
            }
            catch (Exception ex)
            {
                await DisplayAlert("title", ex.Message, "cancel");
                return false;
            }
        }

        private async Task<bool> ProcessFile(Stream xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xml);
            XmlNode versionNode = doc.GetElementsByTagName("Version")[0];
            if (versionNode.Attributes["number"].Value != Settings.Version)
                if (await DisplayAlert("", "Nová verze k dispozici. Chcete přesměrovat na stránku pro stažení nové verze?", "Ano", "Ne"))
                {
                    Device.OpenUri(new Uri(versionNode.Attributes["link"].Value));
                    return false;
                }
            foreach (XmlNode node in doc.GetElementsByTagName("Event"))
            {
                Event event1 = new Event(node);
                if (DateTime.Today < event1.To)
                    if (!Settings.FinishedEvents.Split(';').ToList().Contains(event1.Id.ToString()))
                    {
                        events.Add(event1);
                    }



            }

            return true;
        }
    }
}