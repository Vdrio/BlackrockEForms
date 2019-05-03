using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Azure;
using VdrioEForms.EFForms;
using VdrioEForms.Filters;
using VdrioEForms.Layouts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VdrioEForms.Export
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ExportDataPage : ContentPage
	{
        public static List<EFForm> Forms;
        public static List<EFForm> FilteredForms;
        public static Sorter CurrentSorter;
        public static EFForm SelectedForm;
        public static List<Filter> CurrentFilters;
        public DateTime fromTime = DateTime.Today;
        public DateTime toTime = DateTime.Today + TimeSpan.FromDays(1) - TimeSpan.FromTicks(1);
        public static int lastSelectedIndex = 0;

        bool pageLoading = false, itemsLoading = false;
        public ExportDataPage()
        {
            CurrentSorter = null;
            pageLoading = true;
            SelectedForm = null;
            InitializeComponent();
            FilteredForms = new List<EFForm>();
            FiltersPage.currentFilters = null;
            CurrentFilters = new List<Filter>();
            SetupFormPicker();
            FromPicker.Date = DateTime.Today;
            ToPicker.Date = DateTime.Today;
            FromPicker.PropertyChanged += FromDateChanged;
            ToPicker.PropertyChanged += ToDateChanged;
            pageLoading = false;
        }

        public void FromDateChanged(object sender, EventArgs e)
        {
            Debug.WriteLine(e.GetType().ToString());
            if (e as PropertyChangedEventArgs != null)
            {
                Debug.WriteLine((e as PropertyChangedEventArgs).PropertyName);
                if ((e as PropertyChangedEventArgs).PropertyName == "Date")
                {
                    fromTime = FromPicker.Date;
                    LoadPage();
                }
            }
        }
        public void ToDateChanged(object sender, EventArgs e)
        {
            if (e as PropertyChangedEventArgs != null)
            {
                if ((e as PropertyChangedEventArgs).PropertyName == "Date")
                {
                    toTime = ToPicker.Date + TimeSpan.FromDays(1) - TimeSpan.FromTicks(1);
                    LoadPage();
                }
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!pageLoading && !itemsLoading)
            {
                if (SelectedForm != null)
                {
                    if (FiltersPage.currentFilters?.filters?.Count > 0)
                    {
                        CurrentFilters = FiltersPage.currentFilters.filters;
                        FilterBtn.filtersLabel.Text = CurrentFilters.Count.ToString();
                        LoadPage();
                    }
                    else
                    {
                        FilterBtn.filtersLabel.Text = "0";
                        CurrentFilters.Clear();
                        LoadPage();
                    }
                }
            }

        }

        async void LoadPage()
        {
            itemsLoading = true;
            ExportButton.IsEnabled = false;
            if (SelectedForm != null)
            {
                FormPicker.IsEnabled = false;
                HorizontalContent.Children.Clear();
                ButtonStack.Children.Clear();
                HorizontalContent.Children.Add(new Label
                {
                    Text = "Loading Items...",
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalOptions = LayoutOptions.Start,
                    VerticalTextAlignment = TextAlignment.Center,
                    FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                });
                List<EFForm> submissions = await AzureTableManager.GetAllFormSubmissions(SelectedForm.TableName, fromTime, toTime);
                /*if (LoginPage.CurrentUser.UserType == 0 || LoginPage.CurrentUser.UserType == 3)
                {
                    submissions = submissions.FindAll(x => x.OriginalUser.RowKey == LoginPage.CurrentUser.RowKey);
                }*/
                if (CurrentFilters?.Count > 0)
                    submissions = submissions.FindAll(s => Filter.CheckFilters(s, CurrentFilters));
                if (!ShowDeletedDataSwitch.IsToggled)
                {
                    submissions = submissions.FindAll(x => !x.Deleted);
                }

                if (CurrentSorter != null)
                {
                    submissions = Sorter.SortSubmissions(submissions, CurrentSorter);
                }
                else
                {
                    submissions = submissions.OrderByDescending(x => x.TimeInTicks).ToList();
                }
                FilteredForms = submissions;
                FormSubmissionLayout layout = FormSubmissionLayout.CreateTitleLayout(SelectedForm, ShowDeletedSwitch.IsToggled);
                StackLayout itemLayout = FormSubmissionLayout.CreateItemsLayout(submissions, SelectedForm, ShowDeletedSwitch.IsToggled);
                HorizontalContent.Children.Clear();
                HorizontalContent.Children.Add(layout);
                HorizontalContent.Children.Add(itemLayout);
                ButtonStack.Children.Clear();
                ButtonStack.Children.Add(EFEntryToLayout.CreateEditButtonStack(submissions, SelectedForm, ShowDeletedSwitch.IsToggled));
                FormPicker.IsEnabled = true;
            }
            itemsLoading = false;
            ExportButton.IsEnabled = true;

        }

        void SortByClicked(object sender, EventArgs e)
        {
            EFMasterPage.MainPage.Detail.Navigation.PushAsync(new SorterPage(SelectedForm));
        }

        void ExportToExcelClicked(object sender, EventArgs e)
        {
            ExportToExcel.CreateAndOpenExcelDoc(SelectedForm, FilteredForms,CurrentFilters,CurrentSorter,fromTime, toTime, ShowDeletedSwitch.IsToggled);
        }

        public void ShowDeletedToggled(object sender, EventArgs e)
        {
            LoadPage();
        }

        async void SetupFormPicker()
        {
            Forms = await AzureTableManager.GetAllForms();
            Forms = Forms.FindAll(x => !x.Deleted);
            if (FormPicker.Items.Count != 0)
                FormPicker.Items.Clear();
            foreach (EFForm f in Forms)
            {
                FormPicker.Items.Add(f.FormName);
            }
            FormPicker.SelectedIndexChanged += SelectedFormChanged;
            FormPicker.SelectedIndex = 0;
        }

        void SelectedFormChanged(object sender, EventArgs e)
        {
            CurrentSorter = null;
            if (Forms != null)
            {
                HorizontalContent.Children.Clear();
                ButtonStack.Children.Clear();
                EFForm formToFill = Forms.Find(x => x.FormName == FormPicker.SelectedItem as string);
                if (formToFill != null)
                {
                    if (SelectedForm != formToFill)
                    {
                        FilterBtn.filtersLabel.Text = "0";
                        CurrentFilters.Clear();
                    }
                    SelectedForm = formToFill;
                    lastSelectedIndex = FormPicker.SelectedIndex;
                    LoadPage();
                }

            }
        }
    }
}