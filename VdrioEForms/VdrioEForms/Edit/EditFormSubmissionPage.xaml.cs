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

        public EditFormSubmissionPage (EFForm form, EFForm baseForm, bool pending)
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
            string[] displayAlert = new string[2];
            displayAlert[0] = "Update";
            displayAlert[1] = "Are you sure you want to update this data?";
            if ((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is PendingSubmissionsPage)
            {
                displayAlert[0] = "Submit";
                displayAlert[1] = "Are you sure you want to add this pending data?";
            }
            if (await DisplayAlert(displayAlert[0], displayAlert[1], "Yes", "No"))
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
                SelectedForm.Entries = entries;
                SelectedForm.LastModifiedUser = LoginPage.CurrentUser;
                EFForm f = null;
                if ((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is PendingSubmissionsPage)
                {
                    f = await AzureTableManager.AddFormSubmission(SelectedForm);
                    LoginPage.SavedInfo.PendingForms.Remove(LoginPage.SavedInfo.PendingForms.Find(x => x.RowKey == SelectedForm.RowKey));
                    LoginPage.Storage.UpdateSettings(LoginPage.SavedInfo);
                }
                else
                {
                    f = await AzureTableManager.UpdateFormSubmission(SelectedForm);
                }
                if (f != null)
                {
                    if ((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is PendingSubmissionsPage)
                        await DisplayAlert("Submitted", "Pending data submitted", "Ok");
                    else
                        await DisplayAlert("Updated", "Update successful", "Ok");
                    await EFMasterPage.MainPage.Detail.Navigation.PopToRootAsync();
                }
            }
            SubmitButton.IsEnabled = true;
            DeleteRestoreButton.IsEnabled = true;
        }

        public async void DeleteDataClicked(object sender, EventArgs e)
        {
            if ((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is PendingSubmissionsPage)
            {
                if (await DisplayAlert("Delete", "Are you sure you want to delete this pending data? This is permanent.", "Yes", "No"))
                {
                    LoginPage.SavedInfo.PendingForms.Remove(LoginPage.SavedInfo.PendingForms.Find(x => x.RowKey == SelectedForm.RowKey));
                    LoginPage.Storage.UpdateSettings(LoginPage.SavedInfo);
                    await EFMasterPage.MainPage.Detail.Navigation.PopToRootAsync();
                }
                return;
            }
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

        public async void SaveDataClicked(object sender, EventArgs e)
        {
            if (await DisplayAlert("Save", "Are you sure you want to save changes to this pending data? This will not save file entries.", "Yes", "No"))
            {
                SubmitButton.IsEnabled = false;
                DeleteRestoreButton.IsEnabled = false;
                List<EFEntry> entries = new List<EFEntry>();
                foreach (EFEntryItem d in (MainStack.Children[0] as StackLayout).Children)
                {
                    //Debug.WriteLine(d.dataEntry.EntryData);
                    if (d.dataEntry.EntryType == 9 || d.dataEntry.EntryType == 10 || d.dataEntry.EntryType == 11)
                    {
                        d.dataEntry.EntryData = "";
                        Debug.WriteLine("blob type");
                    }
                    d.dataEntry.EncryptEntry();
                    entries.Add(d.dataEntry);
                }
                SelectedForm.Entries = entries;
                SelectedForm.LastModifiedUser = LoginPage.CurrentUser;
                LoginPage.SavedInfo.PendingForms.Remove(LoginPage.SavedInfo.PendingForms.Find(x => x.RowKey == SelectedForm.RowKey));
                LoginPage.SavedInfo.PendingForms.Add(SelectedForm);
                LoginPage.Storage.UpdateSettings(LoginPage.SavedInfo);
                await DisplayAlert("Saved", "Changes saved successfully", "Ok");
                await EFMasterPage.MainPage.Detail.Navigation.PopToRootAsync();
            }
            SubmitButton.IsEnabled = true;
            DeleteRestoreButton.IsEnabled = true;
        }

        void ShowDeletedToggled(object sender, EventArgs e)
        {
            SelectedFormChanged();
        }

        void SelectedFormChanged()
        {
            MainStack.Children.Clear();
            EFForm formToFill = SelectedForm;
            if (formToFill != null)
            {
                SelectedForm = formToFill;
                MainStack.Children.Add(EFEntryToLayout.CreateEditEntryLayout(formToFill, MainForm, ShowDeletedSwitch.IsToggled));
            }

        }
    }
}