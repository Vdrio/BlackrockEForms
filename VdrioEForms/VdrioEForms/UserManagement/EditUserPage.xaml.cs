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
	public partial class EditUserPage : ContentPage
	{
        static List<EFUser> Users;
        static EFUser SelectedUser;

        public EditUserPage()
        {
            InitializeComponent();
            LoadUserPicker();
        }

        async void LoadUserPicker()
        {
            Users = await AzureTableManager.GetAllUsers(ShowDeletedSwitch.IsToggled);
            Users = Users.FindAll(x => x.RowKey != LoginPage.CurrentUser.RowKey);
            UserPicker.Items?.Clear();
            foreach(EFUser u in Users)
            {
                UserPicker.Items.Add(u.Email);
            }
            UserPicker.SelectedIndexChanged -= SelectedUserChanged;
            UserPicker.SelectedIndexChanged += SelectedUserChanged;
            UserPicker.SelectedIndex = 0;
        }

        void SelectedUserChanged(object sender, EventArgs e)
        {
            if (UserPicker.SelectedIndex >= 0)
                SelectedUser = Users[UserPicker.SelectedIndex];
            else
                return;
            LoadPicker();
        }

        void LoadPicker()
        {
            UserTypePicker.Items?.Clear();

            UserTypePicker.Items.Add("Default");
            UserTypePicker.Items.Add("Admin");
            if (SelectedUser.UserType == 2)
            {
                UserTypePicker.Items.Add("Dev Admin");
                UserTypePicker.Items.Add("Default Tester");
                UserTypePicker.Items.Add("Admin Tester");
            }
            if (SelectedUser.UserType > 2)
            {
                UserTypePicker.SelectedIndex = SelectedUser.UserType - 3;
            }
            else
            {
                UserTypePicker.SelectedIndex = SelectedUser.UserType;
            }
            EmailEntry.Text = SelectedUser.Email;
            if (!string.IsNullOrEmpty(SelectedUser.UserName))
            {
                UserEntry.Text = SelectedUser.UserName;
            }
            if (!string.IsNullOrEmpty(SelectedUser.LastName))
            {
                LastNameEntry.Text = SelectedUser.LastName;
            }
            if (!string.IsNullOrEmpty(SelectedUser.FirstName))
            {
                FirstNameEntry.Text = SelectedUser.FirstName;
            }
            if (SelectedUser.Deleted)
            {
                DeleteButton.Text = "Restore";
            }
            else
            {
                DeleteButton.Text = "Delete";
            }
        }

        public async void UpdateClicked(object sender, EventArgs e)
        {
            AddButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            int userType = UserTypePicker.SelectedIndex;
            if (SelectedUser.UserType == 4)
            {
                userType += 3;
            }
            if (!string.IsNullOrEmpty(EmailEntry.Text))
            {
                SelectedUser.Email = EmailEntry.Text;
                SelectedUser.UserName = UserEntry.Text;
                SelectedUser.FirstName = FirstNameEntry.Text;
                SelectedUser.LastName = LastNameEntry.Text;
                if (await AzureTableManager.UpdateUser(SelectedUser) != null)
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
            DeleteButton.IsEnabled = true;
        }

        public async void DeleteClicked(object sender, EventArgs e)
        {
            AddButton.IsEnabled = false;
            DeleteButton.IsEnabled = false;
            int userType = UserTypePicker.SelectedIndex;
            if (SelectedUser.UserType == 4)
            {
                userType += 3;
            }
            if (!string.IsNullOrEmpty(EmailEntry.Text))
            {
                SelectedUser.Email = EmailEntry.Text;
                SelectedUser.UserName = UserEntry.Text;
                SelectedUser.FirstName = FirstNameEntry.Text;
                SelectedUser.LastName = LastNameEntry.Text;
                if (SelectedUser.Deleted)
                {
                    SelectedUser.Deleted = false;
                }
                else
                {
                    SelectedUser.Deleted = true;
                }
                if (await AzureTableManager.UpdateUser(SelectedUser) != null)
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
            DeleteButton.IsEnabled = true;
        }

        public void ShowDeletedToggled(object sender, EventArgs e)
        {
            LoadUserPicker();
        }
       
    }
}