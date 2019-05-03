using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Azure;
using VdrioEForms.Storage;
using VdrioEForms.UserManagement;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VdrioEForms
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
        public static EFStorage Storage;
        public static Settings SavedInfo;
        public static EFUser CurrentUser;

		public LoginPage ()
		{
            Storage = new EFStorage();
            SavedInfo = Storage.CreateAndOrOpenSettings();
            
            
			InitializeComponent ();
            if (SavedInfo.SaveLoginInfo)
            {
                if (!string.IsNullOrEmpty(SavedInfo.UserEntry))
                    UserEntry.Text = SavedInfo.UserEntry;
                if (!string.IsNullOrEmpty(SavedInfo.Password))
                    PasswordEntry.Text = SavedInfo.Password;

                SaveLoginInfoSwitch.IsToggled = true;
            }

        }

        async void CreateUser()
        {
            await AzureTableManager.AddUser(new EFUser { Email = "appdev@apptester.com", UserName = "AppDev", Password = "AppDeveloper", UserType = 2, FirstName = "App", LastName = "Developer" });
        }

        async void LoginClicked(object sender, EventArgs e)
        {
           // await Task.Run(()=>CreateUser());
            Device.BeginInvokeOnMainThread(()=> { FeedbackText.Text = "Logging in..."; });
            try
            {
                CurrentUser = await AzureTableManager.Login(UserEntry.Text, PasswordEntry.Text);
            }
            catch { FeedbackText.Text = "Login failed"; return; }
            
            if (CurrentUser != null)
            {
                if (string.IsNullOrEmpty(CurrentUser.Password))
                {
                    Device.BeginInvokeOnMainThread(() => { FeedbackText.Text = ""; });
                    SetupClicked(sender, e);
                    return;
                }
                if (SaveLoginInfoSwitch.IsToggled)
                {
                    SavedInfo.UserEntry = UserEntry.Text;
                    SavedInfo.Password = PasswordEntry.Text;
                    SavedInfo.SaveLoginInfo = true;
                }
                else
                {
                    SavedInfo.SaveLoginInfo = false;
                }
                Storage.UpdateSettings(SavedInfo);
                Device.BeginInvokeOnMainThread(() => { FeedbackText.Text = "Login Successful, Loading..."; });
                await Task.Delay(250);
                App.Current.MainPage = new EFMasterPage();
            }
            else
            {
                FeedbackText.Text = "Login failed";
            }
        }

        void ShowPasswordSwitched(object sender, EventArgs e)
        {
            PasswordEntry.IsPassword = !ShowPassPhraseSwitch.IsToggled;
        }

        void SaveLoginInfoSwitched(object sender, EventArgs e)
        {

        }

        async void SetupClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SetupUserPage(UserEntry.Text, PasswordEntry.Text));
        }
        
    }
}