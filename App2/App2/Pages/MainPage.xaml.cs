using App2.Pages;
using Microsoft.AppCenter.Crashes;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace App2
{
    public partial class MainPage : ContentPage
    {
        #region Variables

        List<Stand> stands = Settings.CurrentEvent.Stands;
        public int PickerItem => picker.SelectedIndex == 0 ? stands.Count : int.Parse(picker.Items[picker.SelectedIndex]);
        int pageIndex = 0;
        int ClearedPoints => stands.Where(x => x.Visited).Sum(x => x.Points);
        private int TotalPoints => stands.Sum(x => x.Points);

        #endregion

        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            //this.BackgroundColor = Settings.BackgroundColor;

            InitializeGrid();

            picker.SelectedIndex = Settings.PickerSelectedIndex;           
        }

        private void InitializeGrid()
        {
            if (Settings.Stands == string.Empty)
                return;
            var array = Settings.Stands.Split(';');
            for (int i = 0; i < array.Length - 1; i++)
            {
                if (array == null)
                    break;
                var eventArray = array[i].Split('.');
                int eventId = int.Parse(eventArray[0]);
                int standId = int.Parse(eventArray[1]);

                if (Settings.Events.Find(x => x.Id == eventId) == null)
                    Settings.Stands.Replace(array[i], "");
                try
                {
                    stands[standId].Visited = true;
                }
                catch (Exception) { }
            }
        }

        private void Show()
        {
            btnQR.IsVisible = Settings.Permission;
            lblTitle2.Text = "Celkově bodů: " + ClearedPoints + " / " + TotalPoints;
            if (picker.SelectedIndex == 0)
                btnPageBack.IsVisible = btnPageNext.IsVisible = false;
            else
            {
                btnPageBack.IsVisible = !(pageIndex == 0);
                btnPageNext.IsVisible = !(pageIndex == stands.Count / int.Parse(picker.Items[picker.SelectedIndex]));
            }

            AddItemsToGrid();
        }

        public void AddItemsToGrid()
        {
            parent.Children.Clear();

            #region Header

            //parent.Children.Add(new Label { Text = "\u2713", HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, Opacity =  0 },0,0);
            //parent.Children.Add(new Label { Text = "Image", HorizontalOptions = LayoutOptions.Start, HorizontalTextAlignment = TextAlignment.Start, Opacity = 0 }, 1, 0);
            //parent.Children.Add(new Label { Text = "Text", HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Start }, 2, 0);
            //parent.Children.Add(new Label { Text = "Body", HorizontalOptions = LayoutOptions.End, HorizontalTextAlignment = TextAlignment.End }, 3, 0);

            #endregion           

            int start = pageIndex * PickerItem;
            int end = Math.Min(start + PickerItem, stands.Count);

            for (int y = start; y < end; y++)
                stands[y].AddRow(parent, y - start);

            int secItem = int.Parse(picker.Items[1]);
            int difference = end - start;

            if (difference < secItem)
                for (int y = difference; y < secItem; y++)
                    Stand.AddEmptyRow(parent, y);
        }

        public void CheckCode(string result)
        {
            editPass.Text = string.Empty;

            if (!stands.Exists(x => x.Pass == result.ToLower()))
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

            Settings.Stands += Settings.CurrentEvent.Id+"."+stands.IndexOf(stand) + ";";
            if (!stands.Exists(x => !x.Visited))
                BtnSubmit_Clicked(null,null);
            Show();

        }

        private void BtnCode_Clicked(object sender, EventArgs e) => CheckCode(editPass.Text);

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
                await DisplayAlert("Chyba", ex.Message, "Ok");
                Crashes.TrackError(ex);
                return false;
            }
            return true;
        }

        private void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            FinalPage finalPage = new FinalPage(ClearedPoints, TotalPoints);
            finalPage.SetValue(NavigationPage.BarBackgroundColorProperty, Color.Black);
            Navigation.PushAsync(finalPage);
        }

        private void BtnPage_Clicked(object sender, EventArgs e)
        {
            if (picker.SelectedIndex == 0)
                return;
            if ((sender == btnPageBack && pageIndex == 0) || (sender == btnPageNext && pageIndex == stands.Count / int.Parse(picker.Items[picker.SelectedIndex])))
                return;
            pageIndex = sender == btnPageBack ? pageIndex - 1 : pageIndex + 1;
            Show();
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.PickerSelectedIndex = picker.SelectedIndex;
            pageIndex = 0;
            Show();
        }
    }
}
