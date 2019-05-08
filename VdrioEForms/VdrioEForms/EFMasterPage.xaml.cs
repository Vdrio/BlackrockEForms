using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Create;
using VdrioEForms.Edit;
using VdrioEForms.Export;
using VdrioEForms.UserManagement;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VdrioEForms
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EFMasterPage : MasterDetailPage
	{
        public static EFMasterPage MainPage;

		public EFMasterPage ()
		{
			InitializeComponent ();
            MainPage = this;
            if (Device.RuntimePlatform == "iOS")
            {
                iOSBox.IsVisible = true;
            }
            Detail = new NavigationPage(new FillOutPage());
            if (LoginPage.CurrentUser.UserType == 0 || LoginPage.CurrentUser.UserType == 3)
            {
                CreateFormStack.IsVisible = false;
                EditFormStack.IsVisible = false;
            }
        }

        private void FillOutForm_Tapped(object sender, EventArgs e)
        {
            if (FillOutFormStack.BackgroundColor == new Button().BackgroundColor)
            {
                IsPresented = false;
                return;
            }
            FillOutFormStack.BackgroundColor = new Button().BackgroundColor;
            FillOutLabel.TextColor = Color.White;
            FillOutImage.Source = ImageSource.FromFile("FillOutFormWhite.png");
            PendingDataStack.BackgroundColor = Color.Transparent;
            PendingDataImage.Source = ImageSource.FromFile("PendingIconBlack.png");
            PendingDataLabel.TextColor = Color.Black;
            EditDataStack.BackgroundColor = Color.Transparent;
            EditDataLabel.TextColor = Color.Black;
            EditDataImage.Source = ImageSource.FromFile("EditDataBlack.png");
            CreateFormStack.BackgroundColor = Color.Transparent;
            CreateLabel.TextColor = Color.Black;
            CreateImage.Source = ImageSource.FromFile("CreateFormBlack.png");
            EditFormStack.BackgroundColor = Color.Transparent;
            EditLabel.TextColor = Color.Black;
            EditImage.Source = ImageSource.FromFile("EditFormBlack.png");
            ExportDataStack.BackgroundColor = Color.Transparent;
            ExportLabel.TextColor = Color.Black;
            ExportImage.Source = ImageSource.FromFile("ExportBlack.png");
            UserManagementStack.BackgroundColor = Color.Transparent;
            UserLabel.TextColor = Color.Black;
            UserImage.Source = ImageSource.FromFile("UserBlack.png");

            IsPresented = false;
            Detail = new NavigationPage(new FillOutPage());
        }

        private void PendingData_Tapped(object sender, EventArgs e)
        {
            if (PendingDataStack.BackgroundColor == new Button().BackgroundColor)
            {
                IsPresented = false;
                return;
            }
            FillOutFormStack.BackgroundColor = Color.Transparent;
            FillOutLabel.TextColor = Color.Black;
            FillOutImage.Source = ImageSource.FromFile("FillOutFormBlack.png");
            PendingDataStack.BackgroundColor = new Button().BackgroundColor;
            PendingDataImage.Source = ImageSource.FromFile("PendingIconWhite.png");
            PendingDataLabel.TextColor = Color.White;
            EditDataStack.BackgroundColor = Color.Transparent;
            EditDataLabel.TextColor = Color.Black;
            EditDataImage.Source = ImageSource.FromFile("EditDataBlack.png");
            CreateFormStack.BackgroundColor = Color.Transparent;
            CreateLabel.TextColor = Color.Black;
            CreateImage.Source = ImageSource.FromFile("CreateFormBlack.png");
            EditFormStack.BackgroundColor = Color.Transparent;
            EditLabel.TextColor = Color.Black;
            EditImage.Source = ImageSource.FromFile("EditFormBlack.png");
            ExportDataStack.BackgroundColor = Color.Transparent;
            ExportLabel.TextColor = Color.Black;
            ExportImage.Source = ImageSource.FromFile("ExportBlack.png");
            UserManagementStack.BackgroundColor = Color.Transparent;
            UserLabel.TextColor = Color.Black;
            UserImage.Source = ImageSource.FromFile("UserBlack.png");

            IsPresented = false;
            Detail = new NavigationPage(new PendingSubmissionsPage());
        }
        private void EditData_Tapped(object sender, EventArgs e)
        {
            if (EditDataStack.BackgroundColor == new Button().BackgroundColor)
            {
                IsPresented = false;
                return;
            }
            FillOutFormStack.BackgroundColor = Color.Transparent;
            FillOutLabel.TextColor = Color.Black;
            FillOutImage.Source = ImageSource.FromFile("FillOutFormBlack.png");
            PendingDataStack.BackgroundColor = Color.Transparent;
            PendingDataImage.Source = ImageSource.FromFile("PendingIconBlack.png");
            PendingDataLabel.TextColor = Color.Black;
            EditDataStack.BackgroundColor = new Button().BackgroundColor;
            EditDataLabel.TextColor = Color.White;
            EditDataImage.Source = ImageSource.FromFile("EditDataWhite.png");
            CreateFormStack.BackgroundColor = Color.Transparent;
            CreateLabel.TextColor = Color.Black;
            CreateImage.Source = ImageSource.FromFile("CreateFormBlack.png");
            EditFormStack.BackgroundColor = Color.Transparent;
            EditLabel.TextColor = Color.Black;
            EditImage.Source = ImageSource.FromFile("EditFormBlack.png");
            ExportDataStack.BackgroundColor = Color.Transparent;
            ExportLabel.TextColor = Color.Black;
            ExportImage.Source = ImageSource.FromFile("ExportBlack.png");
            UserManagementStack.BackgroundColor = Color.Transparent;
            UserLabel.TextColor = Color.Black;
            UserImage.Source = ImageSource.FromFile("UserBlack.png");

            IsPresented = false;
            Detail = new NavigationPage(new EditSubmissionsPage());
        }
        private void CreateForm_Tapped(object sender, EventArgs e)
        {
            if (CreateFormStack.BackgroundColor == new Button().BackgroundColor)
            {
                IsPresented = false;
                return;
            }
            FillOutFormStack.BackgroundColor = Color.Transparent;
            FillOutLabel.TextColor = Color.Black;
            FillOutImage.Source = ImageSource.FromFile("FillOutFormBlack.png");
            PendingDataStack.BackgroundColor = Color.Transparent;
            PendingDataImage.Source = ImageSource.FromFile("PendingIconBlack.png");
            PendingDataLabel.TextColor = Color.Black;
            EditDataStack.BackgroundColor = Color.Transparent;
            EditDataLabel.TextColor = Color.Black;
            EditDataImage.Source = ImageSource.FromFile("EditDataBlack.png");
            CreateFormStack.BackgroundColor = new Button().BackgroundColor;
            CreateLabel.TextColor = Color.White;
            CreateImage.Source = ImageSource.FromFile("CreateFormWhite.png");
            EditFormStack.BackgroundColor = Color.Transparent;
            EditLabel.TextColor = Color.Black;
            EditImage.Source = ImageSource.FromFile("EditFormBlack.png");
            ExportDataStack.BackgroundColor = Color.Transparent;
            ExportLabel.TextColor = Color.Black;
            ExportImage.Source = ImageSource.FromFile("ExportBlack.png");
            UserManagementStack.BackgroundColor = Color.Transparent;
            UserLabel.TextColor = Color.Black;
            UserImage.Source = ImageSource.FromFile("UserBlack.png");

            IsPresented = false;
            Detail = new NavigationPage(new CreateEFFormPage());
        }
        private void EditForm_Tapped(object sender, EventArgs e)
        {
            if (EditFormStack.BackgroundColor == new Button().BackgroundColor)
            {
                IsPresented = false;
                return;
            }
            FillOutFormStack.BackgroundColor = Color.Transparent;
            FillOutLabel.TextColor = Color.Black;
            FillOutImage.Source = ImageSource.FromFile("FillOutFormBlack.png");
            PendingDataStack.BackgroundColor = Color.Transparent;
            PendingDataImage.Source = ImageSource.FromFile("PendingIconBlack.png");
            PendingDataLabel.TextColor = Color.Black;
            EditDataStack.BackgroundColor = Color.Transparent;
            EditDataLabel.TextColor = Color.Black;
            EditDataImage.Source = ImageSource.FromFile("EditDataBlack.png");
            CreateFormStack.BackgroundColor = Color.Transparent;
            CreateLabel.TextColor = Color.Black;
            CreateImage.Source = ImageSource.FromFile("CreateFormBlack.png");
            EditFormStack.BackgroundColor = new Button().BackgroundColor;
            EditLabel.TextColor = Color.White;
            EditImage.Source = ImageSource.FromFile("EditFormWhite.png");
            ExportDataStack.BackgroundColor = Color.Transparent;
            ExportLabel.TextColor = Color.Black;
            ExportImage.Source = ImageSource.FromFile("ExportBlack.png");
            UserManagementStack.BackgroundColor = Color.Transparent;
            UserLabel.TextColor = Color.Black;
            UserImage.Source = ImageSource.FromFile("UserBlack.png");
            EFMasterPage.MainPage.Detail = new NavigationPage(new EditFormPage());
            IsPresented = false;
        }
        private void ExportData_Tapped(object sender, EventArgs e)
        {
            if (ExportDataStack.BackgroundColor == new Button().BackgroundColor)
            {
                IsPresented = false;
                return;
            }
            FillOutFormStack.BackgroundColor = Color.Transparent;
            FillOutLabel.TextColor = Color.Black;
            FillOutImage.Source = ImageSource.FromFile("FillOutFormBlack.png");
            PendingDataStack.BackgroundColor = Color.Transparent;
            PendingDataImage.Source = ImageSource.FromFile("PendingIconBlack.png");
            PendingDataLabel.TextColor = Color.Black;
            EditDataStack.BackgroundColor = Color.Transparent;
            EditDataLabel.TextColor = Color.Black;
            EditDataImage.Source = ImageSource.FromFile("EditDataBlack.png");
            CreateFormStack.BackgroundColor = Color.Transparent;
            CreateLabel.TextColor = Color.Black;
            CreateImage.Source = ImageSource.FromFile("CreateFormBlack.png");
            EditFormStack.BackgroundColor = Color.Transparent;
            EditLabel.TextColor = Color.Black;
            EditImage.Source = ImageSource.FromFile("EditFormBlack.png");
            ExportDataStack.BackgroundColor = new Button().BackgroundColor;
            ExportLabel.TextColor = Color.White;
            ExportImage.Source = ImageSource.FromFile("ExportWhite.png");
            UserManagementStack.BackgroundColor = Color.Transparent;
            UserLabel.TextColor = Color.Black;
            UserImage.Source = ImageSource.FromFile("UserBlack.png");

            IsPresented = false;
            Detail = new NavigationPage(new ExportDataPage());
        }
        private void UserManagement_Tapped(object sender, EventArgs e)
        {
            if (UserManagementStack.BackgroundColor == new Button().BackgroundColor)
            {
                IsPresented = false;
                return;
            }
            FillOutFormStack.BackgroundColor = Color.Transparent;
            FillOutLabel.TextColor = Color.Black;
            FillOutImage.Source = ImageSource.FromFile("FillOutFormBlack.png");
            PendingDataStack.BackgroundColor = Color.Transparent;
            PendingDataImage.Source = ImageSource.FromFile("PendingIconBlack.png");
            PendingDataLabel.TextColor = Color.Black;
            EditDataStack.BackgroundColor = Color.Transparent;
            EditDataLabel.TextColor = Color.Black;
            EditDataImage.Source = ImageSource.FromFile("EditDataBlack.png");
            CreateFormStack.BackgroundColor = Color.Transparent;
            CreateLabel.TextColor = Color.Black;
            CreateImage.Source = ImageSource.FromFile("CreateFormBlack.png");
            EditFormStack.BackgroundColor = Color.Transparent;
            EditLabel.TextColor = Color.Black;
            EditImage.Source = ImageSource.FromFile("EditFormBlack.png");
            ExportDataStack.BackgroundColor = Color.Transparent;
            ExportLabel.TextColor = Color.Black;
            ExportImage.Source = ImageSource.FromFile("ExportBlack.png");
            UserManagementStack.BackgroundColor = new Button().BackgroundColor;
            UserLabel.TextColor = Color.White;
            UserImage.Source = ImageSource.FromFile("UserWhite.png");

            IsPresented = false;
            Detail = new NavigationPage(new ManageUsersPage());
        }

        async void LogoutClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No"))
                App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}