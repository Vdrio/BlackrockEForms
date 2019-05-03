using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VdrioEForms.UserManagement
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ManageUsersPage : ContentPage
	{
        

		public ManageUsersPage ()
		{
			InitializeComponent ();
            if (LoginPage.CurrentUser.UserType == 0 || LoginPage.CurrentUser.UserType == 3)
            {
                EditButton.IsVisible = false;
                AddButton.IsVisible = false;
            }
		}

        void EditMyInfoClicked(object sender, EventArgs e)
        {
            EFMasterPage.MainPage.Detail.Navigation.PushAsync(new EditMyInfoPage());
        }
        void EditUserClicked(object sender, EventArgs e)
        {
            EFMasterPage.MainPage.Detail.Navigation.PushAsync(new EditUserPage());
        }
        void AddUserClicked(object sender, EventArgs e)
        {
            EFMasterPage.MainPage.Detail.Navigation.PushAsync(new AddUserPage());
        }
    }
}