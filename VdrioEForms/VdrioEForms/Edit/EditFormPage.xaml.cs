using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Azure;
using VdrioEForms.Create;
using VdrioEForms.EFForms;
using VdrioEForms.Layouts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VdrioEForms.Edit
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditFormPage : ContentPage
	{

        public static EditFormPage CurrentEdit;
        public static EFForm EditForm;
        public static StackLayout EditEntryStack;
        public List<EFForm> Forms;

        public EditFormPage()
        {
            InitializeComponent();
            CurrentEdit = this;
            EditEntryStack = EntryStack;
            SetupFormPicker();
            //EditForm = new EFForm();
        }

        async void SetupFormPicker()
        {
            SubmitButton.IsEnabled = false;
            DeleteRestoreButton.IsEnabled = false;
            Forms = await AzureTableManager.GetAllForms();
            List<EFForm> forms, deletedForms;
            forms = Forms.FindAll(x => !x.Deleted);
            deletedForms = Forms.FindAll(x => x.Deleted);
            if (FormPicker.Items.Count != 0)
                FormPicker.Items.Clear();
            if (forms != null)
                foreach (EFForm f in forms)
                {
                    FormPicker.Items.Add(f.FormName);
                }
            if (deletedForms != null)
                foreach (EFForm f in deletedForms)
                {
                    FormPicker.Items.Add(f.FormName);
                }
            FormPicker.SelectedIndexChanged += SelectedFormChanged;
            FormPicker.SelectedIndex = 0;
            SubmitButton.IsEnabled = true;
            DeleteRestoreButton.IsEnabled = true;
        }

        void SelectedFormChanged(object sender, EventArgs e)
        {
            if (Forms != null)
            {
                SubmitButton.IsEnabled = false;
                DeleteRestoreButton.IsEnabled = false;
                EFForm formToFill = Forms.Find(x => x.FormName == FormPicker.SelectedItem as string);
                if (formToFill != null)
                {
                    EditForm = formToFill;
                    NameEntry.Text = EditForm.FormName;
                    EntryStack.Children.Clear();
                    foreach(EFEntry ent in EditForm.Entries.FindAll(x => !x.Deleted))
                    {
                        EntryStack.Children.Add(new EditEntryItemLayout(ent, EditForm));
                    }
                    foreach (EFEntry ent in EditForm.Entries.FindAll(x => x.Deleted))
                    {
                        EntryStack.Children.Add(new EditEntryItemLayout(ent, EditForm, true));
                    }
                    if (EditForm.Deleted)
                    {
                        DeleteRestoreButton.Text = "Restore Form";
                    }
                    else
                    {
                        DeleteRestoreButton.Text = "Delete Form";
                    }
                }

            }
            SubmitButton.IsEnabled = true;
            DeleteRestoreButton.IsEnabled = true;
            // CheckEntries();
        }

        private void AddDataEntryClicked(object sender, EventArgs e)
        {
            EFMasterPage.MainPage.Detail.Navigation.PushAsync(new CreateEFEntryPage());
        }

        public void AddEditEntryToList(EFEntry entry)
        {
            EditEntryItemLayout layout = new EditEntryItemLayout(entry, EditForm);
            int index = 0;
            foreach (EditEntryItemLayout l in EntryStack.Children)
            {
                if (l.Entry.EntryID == entry.EntryID)
                {
                    EntryStack.Children.Remove(l);
                    index = EditForm.Entries.FindIndex(e => e.EntryID == entry.EntryID);
                    EditForm.Entries.Remove(EditForm.Entries.Find(e => e.EntryID == entry.EntryID));
                    break;
                }
            }
            EditForm.Entries.Insert(index,entry);
            EntryStack.Children.Insert(index,layout);
        }

        public void AddEntryToList(EFEntry entry)
        {
            EditEntryItemLayout layout = new EditEntryItemLayout(entry, EditForm);
            EditForm.Entries.Add(entry);
            EntryStack.Children.Add(layout);
        }

        private async void UpdateFormClicked(object sender, EventArgs e)
        {
            if (FormNameAcceptable(NameEntry.Text))
            {
                if (EditForm.Entries.Count > 0)
                {
                    EditForm.FormName = NameEntry.Text;
                    if (await DisplayAlert("Update Form", "Are you sure you want to submit these changes to this form?", "Yes", "No"))
                    {
                        if(await AzureTableManager.UpdateForm(EditForm) != null)
                        {
                            await DisplayAlert("Success", "Update successful.", "Ok");
                            EFMasterPage.MainPage.Detail = new NavigationPage(new EditFormPage());
                        }
                        else
                        {
                            await DisplayAlert("Failed", "Unable to update form.", "Ok");
                        }
                    }
                    
                }
                else
                {
                    await DisplayAlert("Invalid Form", "Form must contain at least 1 entry.", "Ok");
                }
            }
            else
            {
                await DisplayAlert("Invalid Form", "Form names must begin with a letter, contain at least 4 characters and can only contain letters, numbers and spaces.", "Ok");
            }
        }

        private async void DeleteFormClicked(object sender, EventArgs e)
        {
            if (!EditForm.Deleted)
            {
                if (await DisplayAlert("Delete Form", "Are you sure you want to delete this form?", "Yes", "No"))
                {
                    SubmitButton.IsEnabled = false;
                    DeleteRestoreButton.IsEnabled = false;
                    EditForm.Deleted = true;
                    if (await AzureTableManager.UpdateForm(EditForm) != null)
                    {
                        await DisplayAlert("Deleted", "Delete successful.", "Ok");
                        EFMasterPage.MainPage.Detail = new NavigationPage(new EditFormPage());
                    }
                    else
                    {
                        await DisplayAlert("Failed", "Unable to delete form.", "Ok");
                    }
                }
            }
            else
            {
                if (await DisplayAlert("Restore Form", "Are you sure you want to restore this form?", "Yes", "No"))
                {
                    SubmitButton.IsEnabled = false;
                    DeleteRestoreButton.IsEnabled = false;
                    EditForm.Deleted = false;
                    if (await AzureTableManager.UpdateForm(EditForm) != null)
                    {
                        await DisplayAlert("Restored", "Restore successful.", "Ok");
                        EFMasterPage.MainPage.Detail = new NavigationPage(new EditFormPage());
                    }
                    else
                    {
                        await DisplayAlert("Failed", "Unable to restore form.", "Ok");
                    }
                }
            }
            SubmitButton.IsEnabled = true;
            DeleteRestoreButton.IsEnabled = true;
        }

        static readonly string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
        static readonly string disallowedFirstChars = " 0123456789";
        bool FormNameAcceptable(string name)
        {
            if (name.Length < 4)
            {
                return false;
            }
            foreach (char c in name.ToCharArray())
            {
                if (disallowedFirstChars.Contains(name.ToCharArray()[0]))
                {
                    return false;
                }
                if (!allowedChars.ToCharArray().ToList().Contains(c))
                {
                    return false;
                }
            }
            return true;
        }
    }
}