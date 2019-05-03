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
	public partial class ChangePasswordPage : ContentPage
	{
		public ChangePasswordPage ()
		{
			InitializeComponent ();
		}

        async void ChangePasswordClicked(object sender, EventArgs e)
        {
            if (CurrentPasswordEntry.Text != LoginPage.CurrentUser.Password)
            {
                await DisplayAlert("Failed", "New passwords do not match", "Ok");
                return;
            }
            if (NewPasswordEntry.Text != ConfirmPasswordEntry.Text)
            {
                await DisplayAlert("Failed", "New passwords do not match", "Ok");
                return;
            }
            LoginPage.CurrentUser.Password = NewPasswordEntry.Text;
            if (await AzureTableManager.UpdateUser(LoginPage.CurrentUser) != null)
            {
                await DisplayAlert("Success", "Password changed successfully", "Ok");
                await EFMasterPage.MainPage.Detail.Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Failed", "Failed to change password", "Ok");
            }
        }
	}
}