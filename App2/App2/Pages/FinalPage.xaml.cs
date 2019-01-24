using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Crashes;
using System.Net.Mail;

namespace App2.Pages
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FinalPage : ContentPage
	{
        int Visited;

        public FinalPage(int visited, int total)
        {
            Visited = visited;
            NavigationPage.SetHasNavigationBar(this, false);

            InitializeComponent();
            lbl1.Text = "Gratulujeme" + Environment.NewLine + Environment.NewLine
                        + "Vaše skóre: " + visited + "/" + total + Environment.NewLine + Environment.NewLine
                        + "Pro zařazení do soutěže prosím vyplňte informace na následující řádek a odešlete.";
            this.BackgroundColor = Settings.BackgroundColor;
        }

        private void Edit_Focused(object sender, FocusEventArgs e) => ((Entry)sender).BackgroundColor = Color.Default;

        public void BtnSubmit_Clicked(object sender, EventArgs e)
        {
            //if (FormValidation()) //TODO: Remove comments
            //    return;

            #region WriteMessage
            string msg = "Jméno:" + editName.Text + Environment.NewLine +
                         "Příjmení:" + editName2.Text + Environment.NewLine +
                         "Email:" + editMail.Text + Environment.NewLine +
                         "Telefon:" + editPhone.Text + Environment.NewLine +
                         "Počet bodů:" + Visited;
            MailMessage message;
            try
            {
                message = new MailMessage()
                {
                    From = new MailAddress("tropicliberec.h@gmail.com"),
                    To = {new MailAddress("holan@tropicliberec.cz")},
                    Subject = "Tropic - Soutěž",
                    Body = msg
                };
            }
            catch (Exception ex)
            {
                DisplayAlert("Chyba", ex.ToString(),"Ok");
                return;
            }
            
            #endregion

            #region SendEmail
            try
            {
                using (var client = new MailKit.Net.Smtp.SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587);
                    client.SslProtocols = System.Security.Authentication.SslProtocols.Default;

                    client.Authenticate("tropicliberec.h@gmail.com", "tropic213021");
                    client.Send((MimeMessage)message);
                    client.Disconnect(true);
                }


                Settings.FinishedEvents += Settings.CurrentEvent.Id.ToString() + ";";
                List<string> list = Settings.Stands.Split('.').ToList();
                list.RemoveAll(x => x.StartsWith(Settings.CurrentEvent.Id + "."));
                Navigation.PushAsync(new TitlePage());
            }
            catch (Exception ex)
            {
                DisplayAlert("Chyba", ex.Message, "Ok");
                DisplayAlert("Chyba", ex.InnerException.Message, "Ok");
                Crashes.TrackError(ex);
                return;
            } 
            #endregion
        }

        private bool FormValidation()
        {
            editMail.BackgroundColor = editMail2.BackgroundColor = editName.BackgroundColor = editName2.BackgroundColor = Color.Default;

            List<View> arrEntry = gridFrm.Children.Where(x => x.GetType() == typeof(Entry)).ToList();
            arrEntry.Remove(editPhone);

            bool missing = false; //True if any entry is missing value
            foreach (View view in arrEntry)
            {
                Entry entry = (Entry)view;

                if (!string.IsNullOrWhiteSpace(entry.Text))
                    continue;
                entry.BackgroundColor = Color.Red;
                missing = true;
            }
            if (missing)
            {
                DisplayAlert("Chybějící údaje", "Vyplňte prosím chybějící údaje!", "Ok");
                return true;
            }
            if (IsValidEmail(editMail.Text))
            {
                DisplayAlert("Neplatný email", "Zadaný e-mail není platný", "Ok");
                return true;
            }
            if (editMail.Text != editMail2.Text)
            {
                DisplayAlert("Neschodné emaily", "E-mail se neschoduje s e-mailem zadaným pro potvrzení. Překontrolujte si prosím vložené e-maily", "Ok");
                return true;
            }

            return false;
        }

        bool IsValidEmail(string email)
        {
            try
            {
                System.Net.Mail.MailAddress m = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private void BtnBack_Clicked(object sender, EventArgs e) => Navigation.PopAsync();

        private void Switch_Toggled(object sender, ToggledEventArgs e) => btnSubmit.IsEnabled = ((Xamarin.Forms.Switch)sender).IsToggled;

        
    }
}