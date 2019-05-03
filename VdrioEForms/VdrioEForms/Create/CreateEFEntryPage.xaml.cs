using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.EFForms;
using VdrioEForms.Layouts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using VdrioEForms.Edit;

namespace VdrioEForms.Create
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class CreateEFEntryPage : ContentPage
	{
        public static EFEntry newEntry;
        public static int pickerIndex = 0;

        public CreateEFEntryPage ()
		{
			InitializeComponent ();
            LoadInputTypePicker();
            newEntry = new EFEntry();
            inputPicker.SelectedIndex = 0;
        }

        private async void AddEntryClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameEntry.Text))
            {
                await DisplayAlert("Name", "Name cannot be empty.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(newEntry.Units))
            {
                newEntry.Units = "N/A";
            }
            if (inputPicker.SelectedIndex == 3 && newEntry.Selections.ToList().Count == 0)
            {
                await DisplayAlert("Selections", "Must add selections.", "Ok");
                return;
            }
            StackLayout layout = new StackLayout { Orientation = StackOrientation.Horizontal };
            layout.Children.Add(new Label { Text = NameEntry.Text, HorizontalOptions = LayoutOptions.FillAndExpand });
            layout.Children.Add(new Button { Text = "Edit", HorizontalOptions = LayoutOptions.End });
            layout.Children.Add(new Button { Text = "Remove", HorizontalOptions = LayoutOptions.End });
            newEntry.EntryName = NameEntry.Text;
            newEntry.EncryptEntry();
            if ((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is CreateEFFormPage)
                CreateEFFormPage.CurrentCreation.AddEntryToList(newEntry);
            else
            {
                EditFormPage.CurrentEdit.AddEntryToList(newEntry);
            }
            await EFMasterPage.MainPage.Detail.Navigation.PopAsync();
            //NewLandfillDataType.newLandfillDataType.dataEntries.Add(newEntry);
        }

        void LoadInputTypePicker()
        {
            inputPicker.Items.Add("Alphanumeric");//0
            inputPicker.Items.Add("Numeric");//1
            inputPicker.Items.Add("Toggle");//2
            inputPicker.Items.Add("Selection");//3
            inputPicker.Items.Add("Date");//4
            inputPicker.Items.Add("Time");//5
            inputPicker.Items.Add("Date and Time");//6
        }

        public void PickerSelectionChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            Debug.WriteLine(picker.SelectedIndex);
            pickerIndex = picker.SelectedIndex;
            newEntry.EntryType = pickerIndex;
            if (pickerIndex == 0)
            {
                AlphanumericEntry();
            }
            else if (pickerIndex == 1)
            {
                NumericEntry();
            }
            else if (pickerIndex == 2)
            {
                ToggleEntry();
            }
            else if (pickerIndex == 3)
            {
                PickerEntry();
            }
            else if (pickerIndex == 4)
            {
                DateEntry();
            }
            else if (pickerIndex == 5)
            {
                TimeEntry();
            }
            else if (pickerIndex == 6)
            {
                DateTimeEntry();
            }
        }

        public void AlphanumericEntry()
        {
            MainStack.Children.Clear();
        }
        public void NumericEntry()
        {
            MainStack.Children.Clear();
            StackLayout s = new StackLayout { Orientation = StackOrientation.Horizontal };
            Label l = new Label { Text = "Units: ", VerticalTextAlignment = TextAlignment.Center, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)) };
            Entry e = new Entry { Placeholder = "N/A", HorizontalOptions = LayoutOptions.FillAndExpand };
            e.TextChanged += UnitsEntryTextChanged;
            s.Children.Add(l); s.Children.Add(e);
            MainStack.Children.Add(s);
        }

        public void ToggleEntry()
        {
            MainStack.Children.Clear();
        }

        public void PickerEntry()
        {
            MainStack.Children.Clear();
            PickerLayout l = new PickerLayout { NewEntry = newEntry, page = this };
            MainStack.Children.Add(l);
        }

        public void DateEntry()
        {
            MainStack.Children.Clear();
        }

        public void TimeEntry()
        {
            MainStack.Children.Clear();
        }

        public void DateTimeEntry()
        {
            MainStack.Children.Clear();
        }

        public void NameEntryTextChanged(object sender, EventArgs e)
        {
            newEntry.EntryName = ((Entry)sender).Text;
        }

        public void UnitsEntryTextChanged(object sender, EventArgs e)
        {
            newEntry.Units = ((Entry)sender).Text;
        }
    }
}