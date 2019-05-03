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
	public partial class AddUserPage : ContentPage
	{
		public AddUserPage ()
		{
			InitializeComponent ();
            LoadPicker();
		}

        void LoadPicker()
        {
            UserTypePicker.Items.Add("Default");
            UserTypePicker.Items.Add("Admin");
            if (LoginPage.CurrentUser.UserType == 2)
            {
                UserTypePicker.Items.Add("Dev Admin");
                UserTypePicker.Items.Add("Default Tester");
                UserTypePicker.Items.Add("Admin Tester");
            }
            UserTypePicker.SelectedIndex = 0;
        }

        public async void AddClicked(object sender, EventArgs e)
        {
            AddButton.IsEnabled = false;
            int userType = UserTypePicker.SelectedIndex;
            if (LoginPage.CurrentUser.UserType == 4)
            {
                userType += 3;
            }
            if (!string.IsNullOrEmpty(EmailEntry.Text))
            {
                EFUser u = new EFUser { Email = EmailEntry.Text, FirstName = FirstNameEntry.Text, LastName = LastNameEntry.Text, UserType = userType };
                if (await AzureTableManager.AddUser(u) != null)
                {
                    await DisplayAlert("Success", "User added successfully", "Ok");
                    await EFMasterPage.MainPage.Detail.Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Failed", "Failed to add user", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Failed", "E-Mail address required to create user.", "Ok");
            }
            AddButton.IsEnabled = true;
        }
	}
}