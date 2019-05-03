using System;
using System.Collections.Generic;
using System.Text;
using VdrioEForms.Create;
using VdrioEForms.Edit;
using VdrioEForms.EFForms;
using Xamarin.Forms;

namespace VdrioEForms.Layouts
{
    public class NewEntryItemLayout:StackLayout
    {
        public EFEntry Entry;
        public EFForm Form;

        public NewEntryItemLayout()
        {

        }

        public NewEntryItemLayout(EFEntry entry, EFForm form, bool deleted = false)
        {
            Entry = entry;
            Form = form;
            Orientation = StackOrientation.Horizontal;
            Button e = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.Start };
            e.Clicked += EditEntryClicked;
            Children.Add(e);
            Children.Add(new Label { Text = entry.EntryName, HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center, FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))});
            Button r = new Button { Text = "Remove", HorizontalOptions = LayoutOptions.End };

                //r.Text = "Restore";
                //r.Clicked += ReplaceEntryClicked;
            
                r.Clicked += RemoveEntryClicked;
            
            Children.Add(r);
        }

        public void EditEntryClicked(object sender, EventArgs e)
        {
            EFMasterPage.MainPage.Detail.Navigation.PushAsync(new EditEntryPage(Form, Entry));
        }

        public void RemoveEntryClicked(object sender, EventArgs e)
        {
            Form.Entries.Remove(Entry);
            CreateEFFormPage.NewEntryStack.Children.Remove(this);
        }


    }
}
