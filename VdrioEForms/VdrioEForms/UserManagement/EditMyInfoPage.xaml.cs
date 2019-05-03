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
	public partial class EditMyInfoPage : ContentPage
	{

        public EditMyInfoPage()
        {
            InitializeComponent();
            LoadPicker();
        }

        void LoadPicker()
        {
            if (LoginPage.CurrentUser.UserType == 0 || LoginPage.CurrentUser.UserType == 3)
            {
                UserTypePicker.IsEnabled = false;
            }
            
            UserTypePicker.Items.Add("Default");
            UserTypePicker.Items.Add("Admin");
            if (LoginPage.CurrentUser.UserType == 2)
            {
                UserTypePicker.Items.Add("Dev Admin");
                UserTypePicker.Items.Add("Default Tester");
                UserTypePicker.Items.Add("Admin Tester");
            }
            if (LoginPage.CurrentUser.UserType > 2)
            {
                UserTypePicker.SelectedIndex = LoginPage.CurrentUser.UserType - 3;
            }
            else
            {
                UserTypePicker.SelectedIndex = LoginPage.CurrentUser.UserType;
            }
            EmailEntry.Text = LoginPage.CurrentUser.Email;
            if (!string.IsNullOrEmpty(LoginPage.CurrentUser.UserName))
            {
                UserEntry.Text = LoginPage.CurrentUser.UserName;
            }
            if (!string.IsNullOrEmpty(LoginPage.CurrentUser.LastName))
            {
                LastNameEntry.Text = LoginPage.CurrentUser.LastName;
            }
            if (!string.IsNullOrEmpty(LoginPage.CurrentUser.FirstName))
            {
                FirstNameEntry.Text = LoginPage.CurrentUser.FirstName;
            }
        }

        public async void UpdateClicked(object sender, EventArgs e)
        {
            AddButton.IsEnabled = false;
            int userType = UserTypePicker.SelectedIndex;
            if (LoginPage.CurrentUser.UserType == 4)
            {
                userType += 3;
            }
            if (!string.IsNullOrEmpty(EmailEntry.Text))
            {
                LoginPage.CurrentUser.Email = EmailEntry.Text;
                LoginPage.CurrentUser.UserName = UserEntry.Text;
                LoginPage.CurrentUser.FirstName = FirstNameEntry.Text;
                LoginPage.CurrentUser.LastName = LastNameEntry.Text;
                if (await AzureTableManager.UpdateUser(LoginPage.CurrentUser) != null)
                {
                    await DisplayAlert("Success", "User updated successfully", "Ok");
                    await EFMasterPage.MainPage.Detail.Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Failed", "Failed to update user", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Failed", "E-Mail address required for user.", "Ok");
            }
            AddButton.IsEnabled = true;
        }

        async void ChangePasswordClicked(object sender, EventArgs e)
        {
            await EFMasterPage.MainPage.Detail.Navigation.PushAsync(new ChangePasswordPage());
        }
    }
}