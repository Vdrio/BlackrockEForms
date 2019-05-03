using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Azure;
using VdrioEForms.Edit;
using VdrioEForms.EFForms;
using VdrioEForms.Export;
using VdrioEForms.UserManagement;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VdrioEForms.Filters
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FiltersPage : ContentPage
    {
        //VdrioLandfill vdrioLandfill;
        public EFForm selectedType;
        List<EFForm> formTypes;
        public EFEntry selectedEntry;
        public static FiltersPage currentFilters;
        public List<Filter> filters;
        public ContentPage Page;


        public Picker EntryPicker;
        public Entry valueEntry;
        public Picker valuePicker;
        public DatePicker datePicker;
        public TimePicker timePicker;

        public FiltersPage()
        {
            InitializeComponent();
            filters = new List<Filter>();
            currentFilters = this;

        }
        public FiltersPage(EditSubmissionsPage page)
        {
            InitializeComponent();
            filters = new List<Filter>();
            currentFilters = this;
            //vdrioLandfill = page.vdrioLandfill;
            LoadFiltersPage(page);
        }

        public FiltersPage(ExportDataPage page)
        {
            InitializeComponent();
            filters = new List<Filter>();
            currentFilters = this;
            //vdrioLandfill = page.vdrioLandfill;
            LoadFiltersPage(page);
        }

        List<EFUser> CurrentUsers;
        async void LoadFiltersPage(EditSubmissionsPage page)
        {
            CurrentUsers = await AzureTableManager.GetAllUsers();
            formTypes = EditSubmissionsPage.Forms;
            foreach (EFForm f in formTypes)
            {
                DataTypePicker.Items.Add(f.FormName);
            }
            DataTypePicker.SelectedItem = EditSubmissionsPage.SelectedForm.FormName;
            selectedType = EditSubmissionsPage.SelectedForm;
            if (EditSubmissionsPage.CurrentFilters?.Count > 0)
            {
                filters = EditSubmissionsPage.CurrentFilters;
                foreach(Filter f in filters)
                {
                    ContainerStack.Children.Add(CreateFilterLayout(f, f.Entry.EntryData));
                }
            }
            currentFilters = this;
        }

        async void LoadFiltersPage(ExportDataPage page)
        {
            CurrentUsers = await AzureTableManager.GetAllUsers();
            formTypes = ExportDataPage.Forms;
            foreach (EFForm f in formTypes)
            {
                DataTypePicker.Items.Add(f.FormName);
            }
            DataTypePicker.SelectedItem = ExportDataPage.SelectedForm.FormName;
            selectedType = ExportDataPage.SelectedForm;
            if (ExportDataPage.CurrentFilters?.Count > 0)
            {
                filters = ExportDataPage.CurrentFilters;
                foreach (Filter f in filters)
                {
                    ContainerStack.Children.Add(CreateFilterLayout(f, f.Entry.EntryData));
                }
            }
            currentFilters = this;
        }

        async void GetFormTypes()
        {
            formTypes = await AzureTableManager.GetAllForms();
        }

        /*public FiltersPage(LandfillZonePage page)
        {
            InitializeComponent();
            filters = new List<Filter>();
            vdrioLandfill = page.vdrioLandfill;
            currentFilters = this;
        }*/

       /* public FiltersPage(LandfillAnalyzePage page)
        {
            InitializeComponent();
            filters = new List<Filter>();
            //vdrioLandfill = page.vdrioLandfill;
            currentFilters = this;
        }*/


        public void DataTypeChanged(object sender, EventArgs e)
        {
            try
            {
                selectedType = formTypes.Find(t => t.FormName == ((Picker)sender).SelectedItem as string);
                if (EntryPicker != null)
                    FilterStack.Children.Remove(EntryPicker);
               

                EntryPicker = new Picker {
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                };
                EntryPicker.SelectedIndexChanged += DataEntrySelected;
                EntryPicker.Items.Add("User Created");
                EntryPicker.Items.Add("User Last Modified");
                foreach (EFEntry f in selectedType.Entries)
                {
                    EntryPicker.Items.Add(f.EntryName);
                }
                
                FilterStack.Children.Insert(0, EntryPicker);
                EntryPicker.SelectedIndex = 0;

                currentFilters = this;
            }
            catch { selectedType = null; }
        }

        public void DataEntrySelected(object sender, EventArgs e)
        {
            List<View> views = FilterStack.Children.ToList().GetRange(0, 4);
            FilterStack.Children.Clear();
            foreach (View v in views)
            {
                FilterStack.Children.Add(v);
            }
            //selectedType.Entries = JsonConvert.DeserializeObject<List<DataEntry>>(selectedType.DataEntryEntries);
            selectedEntry = selectedType.Entries.Find(x => x.EntryName == EntryPicker.SelectedItem.ToString());
            if (selectedEntry == null)
            {
                if (EntryPicker.SelectedItem.ToString() == "User Created")
                {
                    selectedEntry = new EFEntry { EntryType = 7, EntryName = "User Created" };
                }
                else if (EntryPicker.SelectedItem.ToString() == "User Last Modified")
                {
                    selectedEntry = new EFEntry { EntryType = 8, EntryName = "User Last Modified" };
                }
            }
            if (selectedEntry.EntryType == 0)
            {
                ComparisonPicker.Items.Clear();
                ComparisonPicker.Items.Add("Equal to");
                ComparisonPicker.SelectedItem = "Equal to";
                valueEntry = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, Placeholder = "Value" };
                FilterStack.Children.Add(valueEntry);
            }
            else if (selectedEntry.EntryType == 1)
            {
                ComparisonPicker.Items.Clear();
                ComparisonPicker.Items.Add("Equal to");
                ComparisonPicker.Items.Add("Greater than or equal to");
                ComparisonPicker.Items.Add("Less than or equal to");
                ComparisonPicker.SelectedItem = "Equal to";
                valueEntry = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, Placeholder = "Value" };
                valueEntry.TextChanged += CheckNumericValue;
                FilterStack.Children.Add(valueEntry);
            }
            else if (selectedEntry.EntryType == 2)
            {
                ComparisonPicker.Items.Clear();
                ComparisonPicker.Items.Add("Equal to");
                ComparisonPicker.SelectedItem = "Equal to";
                valuePicker = new Picker();
                valuePicker.Items.Add("true");
                valuePicker.Items.Add("false");
                valuePicker.SelectedItem = "false";
                FilterStack.Children.Add(valuePicker);
            }
            else if (selectedEntry.EntryType == 3)
            {
                ComparisonPicker.Items.Clear();
                ComparisonPicker.Items.Add("Equal to");
                ComparisonPicker.SelectedItem = "Equal to";
                valuePicker = new Picker();
                foreach (string s in selectedEntry.Selections)
                {
                    valuePicker.Items.Add(s);
                }
                valuePicker.SelectedItem = selectedEntry.Selections[0];
                FilterStack.Children.Add(valuePicker);
            }
            else if (selectedEntry.EntryType == 4)
            {
                ComparisonPicker.Items.Clear();
                ComparisonPicker.Items.Add("Equal to");
                ComparisonPicker.Items.Add("Greater than or equal to");
                ComparisonPicker.Items.Add("Less than or equal to");
                ComparisonPicker.SelectedItem = "Equal to";
                datePicker = new DatePicker { HorizontalOptions = LayoutOptions.FillAndExpand };
                datePicker.Date = DateTime.Now;
                FilterStack.Children.Add(datePicker);

            }
            else if (selectedEntry.EntryType == 5)
            {
                ComparisonPicker.Items.Clear();
                ComparisonPicker.Items.Add("Equal to");
                ComparisonPicker.Items.Add("Greater than or equal to");
                ComparisonPicker.Items.Add("Less than or equal to");
                ComparisonPicker.SelectedItem = "Equal to";
                timePicker = new TimePicker { HorizontalOptions = LayoutOptions.FillAndExpand };
                FilterStack.Children.Add(timePicker);
            }
            else if (selectedEntry.EntryType == 6)
            {
                ComparisonPicker.Items.Clear();
                ComparisonPicker.Items.Add("Equal to");
                ComparisonPicker.Items.Add("Greater than or equal to");
                ComparisonPicker.Items.Add("Less than or equal to");
                ComparisonPicker.SelectedItem = "Equal to";
                timePicker = new TimePicker { HorizontalOptions = LayoutOptions.FillAndExpand };
                datePicker = new DatePicker { HorizontalOptions = LayoutOptions.FillAndExpand };
                datePicker.Date = DateTime.Now;
                FilterStack.Children.Add(datePicker);
                FilterStack.Children.Add(timePicker);
            }
            else if (selectedEntry.EntryType == 7 || selectedEntry.EntryType == 8)
            {
                ComparisonPicker.Items.Clear();
                ComparisonPicker.Items.Add("Equal to");
                ComparisonPicker.SelectedItem = "Equal to";
                valuePicker = new Picker();
                Debug.WriteLine("We got " + CurrentUsers.Count + " users");
                foreach (EFUser u in CurrentUsers)
                {
                    valuePicker.Items.Add(u.FirstName + " " + u.LastName);
                }
                valuePicker.SelectedIndex = 0;
                FilterStack.Children.Add(valuePicker);
            }

            Button b = new Button { Text = "Add", HorizontalOptions = LayoutOptions.End };
            b.Clicked += AddClicked;
            FilterStack.Children.Add(b);
        }

        public bool justBackspaced = false;
        public void CheckNumericValue(object sender, EventArgs e)
        {
            if (!justBackspaced)
            {
                Entry entry = (Entry)sender;
                if (float.TryParse(entry.Text, out float result))
                {
                    Debug.WriteLine("num entry good");
                }
                else if (entry.Text.Length != 0)
                {
                    Debug.WriteLine("num entry bad");
                    entry.Text = entry.Text.Substring(0, entry.Text.Length - 1);
                    justBackspaced = true;
                }
            }
            else
            {
                justBackspaced = false;
            }

        }

        public void AddClicked(object sender, EventArgs e)
        {
            string data = "";
            if (selectedEntry.EntryType == 0)
            {
                data = valueEntry.Text;
            }
            else if (selectedEntry.EntryType == 1)
            {
                data = valueEntry.Text;
            }
            else if (selectedEntry.EntryType == 2)
            {
                data = valuePicker.SelectedItem.ToString();
            }
            else if (selectedEntry.EntryType == 3)
            {
                data = valuePicker.SelectedItem.ToString();
            }
            else if (selectedEntry.EntryType == 4)
            {
                data = datePicker.Date.ToString("MM/dd/yyyy");
            }
            else if (selectedEntry.EntryType == 5)
            {
                DateTime d = DateTime.Today;
                d += timePicker.Time;
                data = d.ToString("hh:mm tt");
            }
            else if (selectedEntry.EntryType == 6)
            {
                DateTime d = datePicker.Date + timePicker.Time;
                data = d.ToString("MM/dd/yyyy hh:mm tt");
            }
            else if (selectedEntry.EntryType == 7 || selectedEntry.EntryType == 8)
            {
                data = valuePicker.SelectedItem.ToString();
            }


            //Debug.WriteLine(selectedEntry.DataTypeType);
            Filter newFilter = new Filter
            {
                Entry = new EFEntry { EntryName = selectedEntry.EntryName, EntryData = data, EntryType = selectedEntry.EntryType, Selections = selectedEntry.Selections,
                EntryID = selectedEntry.EntryID}
            ,
                Comparison = Filter.GetComparisonType(ComparisonPicker.SelectedIndex),
                FormType = selectedType
            };
            ContainerStack.Children.Add(CreateFilterLayout(newFilter));
            filters.Add(newFilter);
            DataTypePicker.SelectedItem = selectedType.FormName;
            EntryPicker.SelectedIndex = 0;
            ComparisonPicker.SelectedItem = "Equal to";
            foreach (FilterButton filterButton in FilterButton.filterButtons)
            {
                filterButton.filtersLabel.Text = filters.Count.ToString();
            }
        }

        public StackLayout CreateFilterLayout(Filter f)
        {
            StackLayout s = new StackLayout { Orientation = StackOrientation.Vertical };
            BoxView b = new BoxView { HeightRequest = 1, BackgroundColor = Color.Black };
            StackLayout c = new StackLayout { Orientation = StackOrientation.Horizontal };
            Label l = new Label();
            if (f.Entry.EntryType == 0)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + valueEntry.Text, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 1)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + valueEntry.Text, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 2)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + valuePicker.SelectedItem.ToString(), HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 3)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + valuePicker.SelectedItem.ToString(), HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 4)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + datePicker.Date.ToString(@"MM/dd/yyyy"), HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 5)
            {
                DateTime d = DateTime.Today;
                d += timePicker.Time;
                l = new Label
                {
                    Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + d.ToString("hh:mm tt")
                    ,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    FontSize = 22
                };
            }
            else if (f.Entry.EntryType == 6)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + datePicker.Date.ToString(@"MM/dd/yyyy hh:mm tt"), HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 7)
            {
                l = new Label { Text = " " + "User Created" + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + valuePicker.SelectedItem as string, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 8)
            {
                l = new Label { Text = " " + "User Last Modified" + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + valuePicker.SelectedItem as string, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            RemoveFilterButton button = new RemoveFilterButton { Text = "Remove", HorizontalOptions = LayoutOptions.End };
            button.filter = f;
            button.Clicked += RemoveClicked;
            c.Children.Add(l); c.Children.Add(button);
            s.Children.Add(c); s.Children.Add(b);
            return s;
        }
        public StackLayout CreateFilterLayout(Filter f, string value)
        {
            StackLayout s = new StackLayout { Orientation = StackOrientation.Vertical };
            BoxView b = new BoxView { HeightRequest = 1, BackgroundColor = Color.Black };
            StackLayout c = new StackLayout { Orientation = StackOrientation.Horizontal };
            Label l = new Label();
            if (f.Entry.EntryType == 0)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + value, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 1)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + value, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 2)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + value, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 3)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + value, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 4)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + value, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 5)
            {
                DateTime d = DateTime.Today;
                d += timePicker.Time;
                l = new Label
                {
                    Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + value
                    ,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    FontSize = 22
                };
            }
            else if (f.Entry.EntryType == 6)
            {
                l = new Label { Text = " " + f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + value, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 7)
            {
                l = new Label { Text = " " + "User Created" + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + value, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            else if (f.Entry.EntryType == 8)
            {
                l = new Label { Text = " " + "User Last Modified" + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + value, HorizontalOptions = LayoutOptions.FillAndExpand, FontSize = 22 };
            }
            RemoveFilterButton button = new RemoveFilterButton { Text = "Remove", HorizontalOptions = LayoutOptions.End };
            button.filter = f;
            button.Clicked += RemoveClicked;
            c.Children.Add(l); c.Children.Add(button);
            s.Children.Add(c); s.Children.Add(b);
            return s;
        }

        void RemoveClicked(object sender , EventArgs e)
        {
            RemoveFilterButton b = (RemoveFilterButton)sender;
            int index = 0;
            index = filters.FindIndex(x => x == b.filter);
            filters.Remove(b.filter);
            ContainerStack.Children.RemoveAt(index);
        }

        public void ClearFiltersClicked(object sender, EventArgs e)
        {
            filters.Clear();
            ContainerStack.Children.Clear();
            foreach (FilterButton filterButton in FilterButton.filterButtons)
            {
                filterButton.filtersLabel.Text = "0";
            }
        }

        public void DoneClicked(object sender, EventArgs e)
        {
            EFMasterPage.MainPage.Detail.Navigation.PopAsync();
        }

        public void PickerChanged(object sender, EventArgs e)
        {

        }
    }
}