using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Azure;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VdrioEForms.UserManagement
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SetupUserPage : ContentPage
	{
		public SetupUserPage (string userEntry = "", string passEntry = "")
		{

            InitializeComponent ();
            if (!string.IsNullOrEmpty(userEntry))
            {
                EmailEntry.Text = userEntry;
                EmailEntry.IsEnabled = true;
                CheckEmailForMore();
            }
            if (!string.IsNullOrEmpty(passEntry))
            {
                PasswordEntry.Text = passEntry;
            }
        }

        async void CheckEmailForMore()
        {
            EFUser u = await AzureTableManager.GetUser(EmailEntry.Text);
            if (u != null)
            {
                FirstNameEntry.Text = u.FirstName;
                LastNameEntry.Text = u.LastName;

            }
        }

        async void SetupClicked(object sender, EventArgs e)
        {
            FeedbackText.Text = "Setting up user...";
            if (string.IsNullOrEmpty(UserEntry.Text) || string.IsNullOrEmpty(EmailEntry.Text)|| string.IsNullOrEmpty(LastNameEntry.Text)||string.IsNullOrEmpty(FirstNameEntry.Text)
                || string.IsNullOrEmpty(PasswordEntry.Text) || string.IsNullOrEmpty(ConfirmPasswordEntry.Text))
            {
                await DisplayAlert("Failed", "Must fill in all entries", "Ok");
                FeedbackText.Text = "Failed";
                return;
            }

            if (PasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Failed", "Passwords do not match", "Ok");
                FeedbackText.Text = "Failed";
                return;
            }
            
            if (await AzureTableManager.GetUser("", EmailEntry.Text) == null)
            {
                await DisplayAlert("Failed", "Email not valid", "Ok");
                FeedbackText.Text = "Failed";
                return;
            }

            EFUser u = await AzureTableManager.Setup(EmailEntry.Text, UserEntry.Text, FirstNameEntry.Text,
                LastNameEntry.Text, PasswordEntry.Text, ConfirmPasswordEntry.Text);
            if (u != null)
            {
                LoginPage.CurrentUser = u;
                FeedbackText.Text = "Setup successful, logging in...";
                LoginPage.SavedInfo.UserEntry = EmailEntry.Text;
                LoginPage.SavedInfo.Password = PasswordEntry.Text;
                LoginPage.Storage.UpdateSettings(LoginPage.SavedInfo);
                App.Current.MainPage = new EFMasterPage();
            }
            else
            {
                await DisplayAlert("Failed", "Username must be unique", "Ok");
                FeedbackText.Text = "Failed";
            }

        }
	}
}