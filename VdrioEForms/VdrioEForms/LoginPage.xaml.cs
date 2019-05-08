using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Azure;
using VdrioEForms.EFForms;
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
            /*if (!string.IsNullOrEmpty(SavedInfo.PendingFormsJson))
                SavedInfo.PendingForms = JsonConvert.DeserializeObject<List<EFForm>>(SavedInfo.PendingFormsJson);
            if (!string.IsNullOrEmpty(SavedInfo.SavedMainFormsJson))
                SavedInfo.SavedMainForms = JsonConvert.DeserializeObject<List<EFForm>>(SavedInfo.SavedMainFormsJson);
            if (!string.IsNullOrEmpty(SavedInfo.LastUserJson))
            {
                SavedInfo.LastUser = JsonConvert.DeserializeObject<EFUser>(SavedInfo.LastUserJson);
                SavedInfo.LastUser.DecryptUser();
            }
            if (SavedInfo.PendingForms == null)
            {
                SavedInfo.PendingForms = new List<EFForm>();
            }
            if (SavedInfo.SavedMainForms == null)
            {
                SavedInfo.SavedMainForms = new List<EFForm>();
            }
            else
            {
                foreach(EFForm f in SavedInfo.SavedMainForms)
                {
                    f.Entries = JsonConvert.DeserializeObject<List<EFEntry>>(f.EntriesJson);
                    foreach(EFEntry e in f.Entries)
                    {
                        e.DecryptEntry();
                    }
                }
            }*/

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
            catch {
                if (SavedInfo.LastUser != null)
                {
                    if ((SavedInfo.LastUser.Email == UserEntry.Text || SavedInfo.LastUser.UserName == UserEntry.Text) && SavedInfo.LastUser.Password == PasswordEntry.Text)
                    {
                        CurrentUser = SavedInfo.LastUser;
                        Device.BeginInvokeOnMainThread(() => { FeedbackText.Text = "Login Successful, Loading..."; });
                        await DisplayAlert("Offline", "Logging in offline", "Ok");
                        App.Current.MainPage = new EFMasterPage();
                        return;
                    }
                }
                Debug.WriteLine("Caught at login page");
                FeedbackText.Text = "Login failed"; return;
            }

            if (CurrentUser == null)
            {
                if (SavedInfo.LastUser != null)
                {
                    if ((SavedInfo.LastUser.Email == UserEntry.Text || SavedInfo.LastUser.UserName == UserEntry.Text) && SavedInfo.LastUser.Password == PasswordEntry.Text)
                    {
                        CurrentUser = SavedInfo.LastUser;
                        Device.BeginInvokeOnMainThread(() => { FeedbackText.Text = "Login Successful, Loading..."; });
                        await DisplayAlert("Offline", "Logging in offline", "Ok");
                        App.Current.MainPage = new EFMasterPage();
                        return;
                    }
                }
            }
            else
            {
                SavedInfo.LastUser = CurrentUser;
                Storage.UpdateSettings(SavedInfo);
            }

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
                Debug.WriteLine("Current User null");
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