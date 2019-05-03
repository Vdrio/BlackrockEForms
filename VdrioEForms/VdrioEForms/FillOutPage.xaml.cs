using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Azure;
using VdrioEForms.EFForms;
using VdrioEForms.Layouts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VdrioEForms
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class FillOutPage : ContentPage
	{
        public static List<EFForm> Forms;
        public static EFForm SelectedForm;

		public FillOutPage ()
		{
			InitializeComponent ();
            Forms = new List<EFForm>();
            SetupFormPicker();
            
		}

        async void CheckEntries()
        {
            List<EFForm> forms = await AzureTableManager.GetAllFormSubmissions(SelectedForm.TableName,DateTime.UtcNow - TimeSpan.FromDays(1), DateTime.UtcNow);
            Debug.WriteLine("Number of form submissions found: " + forms.Count);
            foreach (EFForm f in forms)
            {
                //f.OriginalUser.DecryptUser();
                Debug.WriteLine("Found submssion by: " + f.OriginalUser.UserName);
            }
        }

        async void SetupFormPicker()
        {
            Forms = await AzureTableManager.GetAllForms();
            Forms = Forms.FindAll(x => !x.Deleted);
            if (FormPicker.Items.Count != 0)
                FormPicker.Items.Clear();
            if(Forms != null)
            foreach (EFForm f in Forms)
            {
                FormPicker.Items.Add(f.FormName);
            }
            FormPicker.SelectedIndexChanged += SelectedFormChanged;
            FormPicker.SelectedIndex = 0;
        }

        void ShowDeletedToggled(object sender, EventArgs e)
        {

            SelectedFormChanged(sender, e);
        }

        void SelectedFormChanged(object sender, EventArgs e)
        {
            if (Forms != null)
            {
                MainStack.Children.Clear();
                EFForm formToFill = Forms.Find(x => x.FormName == FormPicker.SelectedItem as string);
                if (formToFill != null)
                {
                    SelectedForm = formToFill;
                    MainStack.Children.Add(EFEntryToLayout.CreateEntryLayout(formToFill, ShowDeletedSwitch.IsToggled));
                }

            }
            // CheckEntries();
        }

        public async void AddDataClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Confirmation", "Are you sure you want to add this data?", "Yes", "No"))
            {
                SubmitButton.IsEnabled = false;
                List<EFEntry> entries = new List<EFEntry>();
                foreach (EFEntryItem d in (MainStack.Children[0] as StackLayout).Children)
                {
                    Debug.WriteLine(d.dataEntry.EntryData);
                    d.dataEntry.EncryptEntry();
                    entries.Add(d.dataEntry);
                }
                EFForm formToSubmit = new EFForm
                {
                    FormName = SelectedForm.FormName,
                    Created = DateTime.Now,
                    LastModified = DateTime.Now,
                    Entries = entries,
                    TableName = SelectedForm.TableName,
                    OriginalUser = LoginPage.CurrentUser,
                    LastModifiedUser = LoginPage.CurrentUser
                };
                if (await AzureTableManager.AddFormSubmission(formToSubmit) != null)
                {
                    await DisplayAlert("Submitted", "Form submitted succesfully", "Ok");
                    SelectedFormChanged(sender, e);
                }
                else
                {
                    await DisplayAlert("Failed", "Unable to submit form", "Ok");
                }
            }
            SubmitButton.IsEnabled = true;
        }
    }
}