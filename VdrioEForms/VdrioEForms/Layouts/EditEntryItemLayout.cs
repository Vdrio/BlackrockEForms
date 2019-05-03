using System;
using System.Collections.Generic;
using System.Text;
using VdrioEForms.Edit;
using VdrioEForms.EFForms;
using Xamarin.Forms;

namespace VdrioEForms.Layouts
{
    public class EditEntryItemLayout : StackLayout
    {
        public EFEntry Entry;
        public EFForm Form;

        public EditEntryItemLayout()
        {

        }

        public EditEntryItemLayout(EFEntry entry, EFForm form, bool deleted = false)
        {
            Entry = entry;
            Form = form;
            Orientation = StackOrientation.Horizontal;
            Button e = new Button { Text = "Edit", HorizontalOptions = LayoutOptions.Start };
            e.Clicked += EditEntryClicked;
            Children.Add(e);
            Children.Add(new Label
            {
                Text = entry.EntryName,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                HorizontalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
            });
            Button r = new Button { Text = "Remove", HorizontalOptions = LayoutOptions.End };

            if (deleted)
            {
                r.Text = "Restore";
                r.Clicked += RestoreEntryClicked;
            }
            else
            {
                r.Clicked += RemoveEntryClicked;
            }

            Children.Add(r);
        }

        public void EditEntryClicked(object sender, EventArgs e)
        {
            EFMasterPage.MainPage.Detail.Navigation.PushAsync(new EditEntryPage(Form, Entry));
        }

        public void RemoveEntryClicked(object sender, EventArgs e)
        {
            Form.Entries.Find(x=>x.EntryID == Entry.EntryID).Deleted = true;
            EditFormPage.EditEntryStack.Children.Remove(this);
            EditFormPage.EditEntryStack.Children.Add(new EditEntryItemLayout(Entry, Form, true));
        }

        public void RestoreEntryClicked(object sender, EventArgs e)
        {
            Form.Entries.Find(x => x.EntryID == Entry.EntryID).Deleted = false;
            int index = Form.Entries.FindIndex(x => x.EntryID == Entry.EntryID);
            EditFormPage.EditEntryStack.Children.Remove(this);
            EditFormPage.EditEntryStack.Children.Insert(Math.Max(0, index), new EditEntryItemLayout(Entry, Form));
        }
    }
}
