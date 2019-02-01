using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Microsoft.AppCenter.Crashes;

namespace App2.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoadPage : ContentPage
	{
        List<Event> events = new List<Event>();

        public  LoadPage()
        {        
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();

            CheckXML();
            
        }

        private async void CheckXML()
        {
            //await Task.Delay(TimeSpan.FromSeconds(3));
            //if (await Settings.CheckVersion(this))
            //{
            //    return;
            //}
            //if (Settings.CheckEvents(this))
            //{
            //    lbl.Text = "V tuto chvíly nejsou dostupné žádné soutěže.";
            //    return;
            //}
            //await DisplayAlert("Test2", "Test2", "Ok");
            Settings.ProcessXml(this);
            
            await Navigation.PushAsync(new MainPage());

            //Navigation.RemovePage(this);
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            CheckXML();
        }
    }
}