using App2.Pages;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App2
{
    public partial class MainPage : ContentPage
    {
        List<Stand> stands = Settings.CurrentEvent.Stands;

        int ClearedPoints
        {
            get
            {
                int value = 0;
                foreach (Stand item in stands)
                {
                    if (item.Visited)
                        value += item.Points;
                }
                return value;
            }
        }

        private int GetTotalPoints()
        {

            int value = 0;
            foreach (Stand item in stands)
                value += item.Points;
            return value;
        }

        protected override void OnAppearing()
        {
            //if (Settings.FinishedEvents.Split(';').ToList().Contains(Settings.CurrentEvent.Id.ToString()))
            //    Navigation.PushAsync(new TitlePage());
            //DisplayAlert("Test","Test","cancel");
            base.OnAppearing();

        }

        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            //DownloadFile();
            //DownloadFileFTP();


            //stands.Add(new Stand("Hobby", "hobby"));
            //stands.Add(new Stand("Fox", "fox", 5));
            //stands.Add(new Stand("Mivardi", "mivardi", 2));
            //stands.Add(new Stand("Dam", "dam",9));
            //stands.Add(new Stand("Egerfish", "egerfish",2));
            //stands.Add(new Stand("Saenger", "saenger",7));
            //stands.Add(new Stand("Nikl", "nikl",5));
            //stands.Add(new Stand("Mikado", "mikado",3));
            //stands.Add(new Stand("Mikbaits", "mikbaits",6));
            //stands.Add(new Stand("Sportcarp", "sportcarp",8));
            //stands.Add(new Stand("Moss", "moss",4));
            //stands.Add(new Stand("Normark", "normark",10));
            //stands.Add(new Stand("Stormkloth", "stormkloth",6));
            //stands.Add(new Stand("Slovimex", "slovimex",2));
            //stands.Add(new Stand("Svendsen", "svendsen",3));

            string[] array = Settings.Stands.Split(';');
            for (int i = 0; i < array.Length; i++)
                if (int.TryParse(array[i], out int result))
                    stands[result].Visited = true;

            this.BackgroundColor = Settings.BackgroundColor;
            Show();
        }

        private void Show()
        {
            btnQR.IsVisible = Settings.Permission;
            lblTitle2.Text = "Celkově bodů: " + ClearedPoints + " / " + GetTotalPoints();

            parent.Children.Clear();
            #region Header

            parent.Children.Add(new Label { Text = "\u2713", HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, Opacity =  0 },0,0);
            parent.Children.Add(new Label { Text = "Image", HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, Opacity = 0 }, 1, 0);
            parent.Children.Add(new Label { Text = "Text", HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Start }, 2, 0);
            parent.Children.Add(new Label { Text = "Body", HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End }, 3, 0);

            #endregion


            for (int y = 0; y < stands.Count; y++)
            {
                List<View> list = stands[y].AddRow();
                for (int x = 0; x < list.Count; x++)
                    parent.Children.Add(list[x], x, y);
            }
                
        }

        public void CheckCode(string result)
        {
            editPass.Text = "";

            if (!stands.Exists(x => x.Pass == result))
            {
                DisplayAlert("Chyba", "Tento kód není platný", "Ok");
                return;
            }
            Stand stand = stands.First(x => x.Pass == result);

            if (stand.Visited)
            {
                DisplayAlert(" ", "Tento kód jste již zadali", "Ok");
                return;
            }
            stand.Visited = true;

            Settings.Stands += stands.IndexOf(stand) + ";";
            if (!stands.Exists(x => !x.Visited))
                Submit();
            Show();

        }

        private void BtnCode_Clicked(object sender, EventArgs e)
        {
            CheckCode(editPass.Text);
            Show();
        }

        private async void BtnQR_Clicked(object sender, EventArgs e)
        {
            if (!await TestPermission())
                return;

            await Navigation.PushAsync(new ScanPage(this));

            Show();
        }

        private async Task<bool> TestPermission()
        {
            try
            {
                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                var status = PermissionStatus.Unknown;
                //Best practice to always check that the key exists
                if (results.ContainsKey(Permission.Camera))
                    status = results[Permission.Camera];
                if (status != PermissionStatus.Granted)
                {
                    Settings.Permission = false;
                    Show();
                    return false;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Test", ex.Message, "Ok");
                return false;
            }
            return true;
        }

        private void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            Submit();
        }

        private void Submit()
        {
            FinalPage finalPage = new FinalPage(ClearedPoints, GetTotalPoints());
            finalPage.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);
            Navigation.PushAsync(finalPage);
        }
    }
}
