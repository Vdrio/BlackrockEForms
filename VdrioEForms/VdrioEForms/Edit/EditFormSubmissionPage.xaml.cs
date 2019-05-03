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

namespace VdrioEForms.Edit
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditFormSubmissionPage : ContentPage
	{
        public static EFForm SelectedForm;
        public static EFForm MainForm;

        public EditFormSubmissionPage (EFForm form, EFForm baseForm)
		{
			InitializeComponent ();
            SelectedForm = form;
            MainForm = baseForm;
            SelectedFormChanged();
            if (SelectedForm.Deleted)
            {
                DeleteRestoreButton.Text = "Restore Data";
            }
		}

        public async void UpdateDataClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Update", "Are you sure you want to update this data?", "Yes", "No"))
            {
                SubmitButton.IsEnabled = false;
                DeleteRestoreButton.IsEnabled = false;
                List<EFEntry> entries = new List<EFEntry>();
                foreach (EFEntryItem d in (MainStack.Children[0] as StackLayout).Children)
                {
                    //Debug.WriteLine(d.dataEntry.EntryData);
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
                await AzureTableManager.UpdateFormSubmission(formToSubmit);
                if (await AzureTableManager.UpdateFormSubmission(SelectedForm) != null)
                {
                    await DisplayAlert("Updated", "Update successful", "Ok");
                    await EFMasterPage.MainPage.Detail.Navigation.PopAsync();
                }
            }
            SubmitButton.IsEnabled = true;
            DeleteRestoreButton.IsEnabled = true;
        }

        public async void DeleteDataClicked(object sender, EventArgs e)
        {
            if (!SelectedForm.Deleted)
            {
                if (await DisplayAlert("Delete", "Are you sure you want to delete this data?", "Yes", "No"))
                {
                    SelectedForm.Deleted = true;
                    SubmitButton.IsEnabled = false;
                    DeleteRestoreButton.IsEnabled = false;
                    if (await AzureTableManager.UpdateFormSubmission(SelectedForm) != null)
                    {
                        await DisplayAlert("Deleted", "Delete successful", "Ok");
                        await EFMasterPage.MainPage.Detail.Navigation.PopAsync();
                    }
                }
            }
            else
            {
                if (await DisplayAlert("Restore", "Are you sure you want to restore this data?", "Yes", "No"))
                {
                    SubmitButton.IsEnabled = false;
                    DeleteRestoreButton.IsEnabled = false;
                    SelectedForm.Deleted = false;
                    if (await AzureTableManager.UpdateFormSubmission(SelectedForm) != null)
                    {
                        await DisplayAlert("Restored", "Restore successful", "Ok");
                        await EFMasterPage.MainPage.Detail.Navigation.PopAsync();
                    }
                }
            }
            SubmitButton.IsEnabled = true;
            DeleteRestoreButton.IsEnabled = true;
        }

        void SelectedFormChanged()
        {
            MainStack.Children.Clear();
            EFForm formToFill = SelectedForm;
            if (formToFill != null)
            {
                SelectedForm = formToFill;
                MainStack.Children.Add(EFEntryToLayout.CreateEditEntryLayout(formToFill, MainForm));
            }

        }
    }
}