using App2.Pages;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter.Push;
using Microsoft.AppCenter.Distribute;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace App2
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new TitlePage());
            MainPage.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);
        }

        protected override void OnStart()
        {
            AppCenter.Start("android=1fb3ced4-b621-4adc-a1be-f8ebf0d4d294;" +
                            "uwp=0039cfbe-f7b2-4499-ae28-859854b77e30;" +
                            "ios=f7528c53-caab-4714-9bea-5ad5b241e5d0",
                            typeof(Analytics), 
                            typeof(Crashes), 
                            typeof(Push));
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
