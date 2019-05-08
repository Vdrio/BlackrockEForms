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

namespace VdrioEForms.Edit
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PendingSubmissionsPage : ContentPage
    {
        public static List<EFForm> Forms;
        public static EFForm SelectedForm;
        public static int lastSelectedIndex = 0;

        bool pageLoading = false, itemsLoading = false;
        public PendingSubmissionsPage()
        {
            pageLoading = true;
            SelectedForm = null;
            InitializeComponent();
            SetupFormPicker();
            pageLoading = false;
        }


        protected override void OnAppearing()
        {
            base.OnAppearing();
            SetupFormPicker();

        }

        async void LoadPage()
        {
            itemsLoading = true;
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
                Debug.WriteLine("Getting submissions for : " + SelectedForm.TableName);
                List<EFForm> submissions = LoginPage.SavedInfo.PendingForms.FindAll(x => x.FormName == SelectedForm.FormName);
                if (submissions == null)
                {
                    HorizontalContent.Children.Clear();
                    HorizontalContent.Children.Add(new Label
                    {
                        Text = "No items to load",
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.Start,
                        VerticalTextAlignment = TextAlignment.Center,
                        FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                    });
                    return;
                }
                foreach (EFForm f in submissions)
                {
                    //f.DecryptForm();
                    Debug.WriteLine("Deserializing Entries");
                    if (!string.IsNullOrEmpty(f.EntriesJson))
                        f.Entries = JsonConvert.DeserializeObject<List<EFEntry>>(f.EntriesJson);
                    foreach (EFEntry e in f.Entries)
                    {
                        Debug.WriteLine(e.EntryName);
                        e.DecryptEntry();
                    }
                }
                if (submissions == null)
                {
                    return;
                }

                submissions = submissions.OrderByDescending(x => x.TimeInTicks).ToList();


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

        }

        public void ShowDeletedToggled(object sender, EventArgs e)
        {
            LoadPage();
        }

        async void SetupFormPicker()
        {
            Forms = await AzureTableManager.GetAllForms();
            if (Forms == null)
            {
                Forms = LoginPage.SavedInfo.SavedMainForms;
                if (Forms == null)
                {
                    await DisplayAlert("Forms", "Unable to load forms to pick", "Ok");
                    return;
                }
            }
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
            if (Forms != null)
            {
                HorizontalContent.Children.Clear();
                ButtonStack.Children.Clear();
                EFForm formToFill = Forms.Find(x => x.FormName == FormPicker.SelectedItem as string);
                if (formToFill != null)
                {
                    SelectedForm = formToFill;
                    lastSelectedIndex = FormPicker.SelectedIndex;
                    LoadPage();
                }

            }
        }
    }
}