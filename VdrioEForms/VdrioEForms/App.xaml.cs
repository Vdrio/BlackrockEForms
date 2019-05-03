using System;
using System.Diagnostics;
using System.Text;
using VdrioEForms.Security;
using VdrioEForms.Storage;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace VdrioEForms
{
    public partial class App : Application
    {
        public static IOpenExcel openExcel;

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new LoginPage());
            
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
