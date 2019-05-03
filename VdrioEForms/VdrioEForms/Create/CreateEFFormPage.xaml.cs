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

namespace VdrioEForms.Create
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateEFFormPage : ContentPage
	{
        public static CreateEFFormPage CurrentCreation;
        public static EFForm NewForm;
        public static StackLayout NewEntryStack;

		public CreateEFFormPage ()
		{
			InitializeComponent ();
            CurrentCreation = this;
            NewEntryStack = EntryStack;
            NewForm = new EFForm();
		}

        

        private void AddDataEntryClicked(object sender, EventArgs e)
        {
            EFMasterPage.MainPage.Detail.Navigation.PushAsync(new CreateEFEntryPage());
        }

        public void AddEntryToList(EFEntry entry)
        {
            NewEntryItemLayout layout = new NewEntryItemLayout(entry, NewForm);
            NewForm.Entries.Add(entry);
            EntryStack.Children.Add(layout);
        }
        public void AddEditEntryToList(EFEntry entry)
        {
            NewEntryItemLayout layout = new NewEntryItemLayout(entry, NewForm);
            int index = 0;
            foreach (NewEntryItemLayout l in EntryStack.Children)
            {
                if (l.Entry.EntryID == entry.EntryID)
                {
                    EntryStack.Children.Remove(l);
                    index = NewForm.Entries.FindIndex(e => e.EntryID == entry.EntryID);
                    NewForm.Entries.Remove(NewForm.Entries.Find(e => e.EntryID == entry.EntryID));
                    break;
                }
            }
            NewForm.Entries.Insert(index, entry);
            EntryStack.Children.Insert(index, layout);
        }

        private async void AddFormClicked(object sender, EventArgs e)
        {
            if (FormNameAcceptable(NameEntry.Text))
            {
                if (NewForm.Entries.Count > 0)
                {
                    NewForm.FormName = NameEntry.Text;
                    NewForm.TableName = NameEntry.Text.Replace(" ", "") + "Table";
                    if (AzureTableManager.IsAppTester)
                    {
                        NewForm.TableName = "Test" + NewForm.TableName;
                    }
                    if (await AzureTableManager.AddForm(NewForm) != null)
                    {
                        await DisplayAlert("Success", "Form added successfully.", "Ok");
                        EFMasterPage.MainPage.Detail = new NavigationPage(new CreateEFFormPage());
                    }
                    else
                    {
                        await DisplayAlert("Failed", "Failed to add form.", "Ok");
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

        static readonly string allowedChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 ";
        static readonly string disallowedFirstChars = " 0123456789";
        bool FormNameAcceptable(string name)
        {
            if (name.Length < 4)
            {
                return false;
            }
            foreach(char c in name.ToCharArray())
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