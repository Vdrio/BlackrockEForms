using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VdrioEForms.Edit;
using VdrioEForms.EFForms;
using VdrioEForms.Export;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace VdrioEForms.Filters
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class SorterPage : ContentPage
	{
        public Sorter sorter;
        public EFForm MainForm;
        public List<EFEntry> entriesToUse;
		public SorterPage (EFForm baseForm)
		{
			InitializeComponent ();
            MainForm = baseForm;
            if ((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is EditSubmissionsPage)
            {
                sorter = EditSubmissionsPage.CurrentSorter;
            }
            else
            {
                sorter = ExportDataPage.CurrentSorter;
            }
            string selectedItem = "";
            int index = 0;
            if (sorter == null)
            {
                sorter = new Sorter();
            }
            else
            {
                 index = (int)sorter.Comparison;
                 selectedItem = sorter.Entry.EntryName;
            }
            OrderPicker.Items.Add("Descending");
            OrderPicker.Items.Add("Ascending");
            OrderPicker.SelectedIndex = 0;
            OrderPicker.SelectedIndexChanged += OrderChanged;
            entriesToUse = MainForm.Entries.FindAll(x => x.EntryType == 1 || x.EntryType == 4 || x.EntryType == 5 || x.EntryType == 6 || x.EntryType == 8);
            foreach (EFEntry e in entriesToUse)
            {
                EntryPicker.Items.Add(e.EntryName);
            }
            EntryPicker.SelectedIndexChanged += EntryChanged;
            EntryPicker.SelectedIndex = 0;
            if (index != 0)
            {
                OrderPicker.SelectedIndex = index;
            }
            if (!string.IsNullOrEmpty(selectedItem))
            {
                EntryPicker.SelectedItem = selectedItem;
            }

        }

        void EntryChanged(object sender, EventArgs e)
        {
            EFEntry selectedEntry = entriesToUse[EntryPicker.SelectedIndex];
        }

        void OrderChanged(object sender, EventArgs e)
        {
            if (OrderPicker.SelectedIndex == 0)
            {
                sorter.Comparison = Sorter.ComparisonType.Descending;
            }
            else if (OrderPicker.SelectedIndex == 1)
            {
                sorter.Comparison = Sorter.ComparisonType.Ascending;
            }
        }

        void SortClicked(object sender, EventArgs e)
        {
            sorter.Entry = entriesToUse[EntryPicker.SelectedIndex];
            if (OrderPicker.SelectedIndex == 0)
            {
                sorter.Comparison = Sorter.ComparisonType.Descending;
            }
            else
            {
                sorter.Comparison = Sorter.ComparisonType.Ascending;
            }
            if ((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is EditSubmissionsPage)
            {
                EditSubmissionsPage.CurrentSorter = sorter;
            }
            else
            {
                ExportDataPage.CurrentSorter = sorter;
            }
            EFMasterPage.MainPage.Detail.Navigation.PopAsync();
        }
        void DefaultClicked(object sender, EventArgs e)
        {

            if ((EFMasterPage.MainPage.Detail as NavigationPage).RootPage is EditSubmissionsPage)
            {
                EditSubmissionsPage.CurrentSorter = null;
            }
            else
            {
                ExportDataPage.CurrentSorter = null;
            }
            EFMasterPage.MainPage.Detail.Navigation.PopAsync();
        }
    }
}