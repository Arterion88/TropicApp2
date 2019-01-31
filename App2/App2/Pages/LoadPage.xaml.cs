using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App2.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoadPage : ContentPage
	{
        List<Event> events = new List<Event>();

        public LoadPage()
        {        
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            CheckXML();
            
        }

        private async void CheckXML()
        {
            if(await Settings.CheckVersion(this))
            {
                await Navigation.PopAsync();
                return;
            }
            if (Settings.CheckEvents(this))
            {
                lbl.Text = "V tuto chvíly nejsou dostupné žádné soutěže.";
                return;
            }

            Settings.ProcessXml(this);
            await Navigation.PushAsync(new MainPage());

        }

    }
}