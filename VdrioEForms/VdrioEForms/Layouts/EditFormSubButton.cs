using System;
using System.Collections.Generic;
using System.Text;
using VdrioEForms.Edit;
using VdrioEForms.EFForms;
using Xamarin.Forms;

namespace VdrioEForms.Layouts
{
    public class EditFormSubButton: Button
    {
        public EditFormSubButton(EFForm form, EFForm baseForm)
        {
            FormToEdit = form;
            BaseForm = baseForm;
            WidthRequest = 73;
            HeightRequest = 55;
            Padding = 0;
            BackgroundColor = Color.Transparent;
            TextColor = Color.FromHex("#b5211c");
            FontAttributes = FontAttributes.Bold;
            FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button));
            Text = "Edit";
            if (LoginPage.CurrentUser.UserType == 0 || LoginPage.CurrentUser.UserType == 3)
            {
                if (form.OriginalUser.RowKey != LoginPage.CurrentUser.RowKey)
                {
                    IsEnabled = false;
                }
            }
            //TapGestureRecognizer g = new TapGestureRecognizer();
            //g.Tapped += OnClicked;
            //GestureRecognizers.Add(g);
            Clicked += OnClicked;
        }
        public EFForm FormToEdit;
        public EFForm BaseForm;
        public async void OnClicked(object sender, EventArgs e)
        {
            await EFMasterPage.MainPage.Detail.Navigation.PushAsync(new EditFormSubmissionPage(FormToEdit, BaseForm, (EFMasterPage.MainPage.Detail as NavigationPage).RootPage is PendingSubmissionsPage));
        }
    }
}
