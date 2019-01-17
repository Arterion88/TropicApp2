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
            if (await Settings.DownloadFile(this))
                await Navigation.PushAsync(new MainPage());
        }

        
    }
}