using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Create;
using VdrioEForms.EFForms;
using VdrioEForms.Layouts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VdrioEForms.Edit
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class EditEntryPage : ContentPage
	{

        public static EFEntry editEntry;
        public static int pickerIndex = 0;

        public EditEntryPage(EFForm formToEdit, EFEntry entryToEdit)
        {
            InitializeComponent();
            LoadInputTypePicker();
            editEntry = entryToEdit;
            NameEntry.Text = entryToEdit.EntryName;
            inputPicker.SelectedIndex = entryToEdit.EntryType;

        }

        private async void UpdateEntryClicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(NameEntry.Text))
            {
                await DisplayAlert("Name", "Name cannot be empty.", "Ok");
                return;
            }
            if (string.IsNullOrEmpty(editEntry.Units))
            {
                editEntry.Units = "N/A";
            }
            if (inputPicker.SelectedIndex == 3 && editEntry.Selections.ToList().Count == 0)
            {
                await DisplayAlert("Selections", "Must add selections.", "Ok");
                return;
            }
            StackLayout layout = new StackLayout { Orientation = StackOrientation.Horizontal };
            layout.Children.Add(new Label { Text = NameEntry.Text, HorizontalOptions = LayoutOptions.FillAndExpand });
            layout.Children.Add(new Button { Text = "Edit", HorizontalOptions = LayoutOptions.End });
            layout.Children.Add(new Button { Text = "Remove", HorizontalOptions = LayoutOptions.End });
            editEntry.EntryName = NameEntry.Text;
            editEntry.EncryptEntry();
            if ((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is CreateEFFormPage)
                CreateEFFormPage.CurrentCreation.AddEditEntryToList(editEntry);
            else
            {
                EditFormPage.CurrentEdit.AddEditEntryToList(editEntry);
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
            editEntry.EntryType = pickerIndex;
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
            Entry e = new Entry { Text = editEntry.Units, HorizontalOptions = LayoutOptions.FillAndExpand };
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
            PickerLayout l = new PickerLayout(editEntry.Selections) { NewEntry = editEntry, page = this };
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
            editEntry.EntryName = ((Entry)sender).Text;
        }

        public void UnitsEntryTextChanged(object sender, EventArgs e)
        {
            editEntry.Units = ((Entry)sender).Text;
        }
    }
}