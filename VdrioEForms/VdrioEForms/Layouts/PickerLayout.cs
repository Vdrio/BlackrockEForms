using System;
using System.Collections.Generic;
using System.Text;
using VdrioEForms.EFForms;
using Xamarin.Forms;

namespace VdrioEForms.Layouts
{
    public class PickerLayout : StackLayout
    {
        public EFEntry NewEntry { get; set; }
        public Entry itemToAdd;
        public ContentPage page;
        public PickerLayout()
        {
            Orientation = StackOrientation.Vertical;
            StackLayout s2 = new StackLayout { Orientation = StackOrientation.Horizontal };
            Label l = new Label { Text = "Selectable Items: ", VerticalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)), HorizontalOptions = LayoutOptions.Start };
            itemToAdd = new Entry { Placeholder = "Selection", HorizontalOptions = LayoutOptions.FillAndExpand };
            Button b = new Button { Text = "Add", HorizontalOptions = LayoutOptions.End };
            b.Clicked += AddClicked;
            BoxView bv = new BoxView { HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1, BackgroundColor = Color.Black };
            s2.Children.Add(l); s2.Children.Add(itemToAdd); s2.Children.Add(b);
            Children.Add(s2); Children.Add(bv);
        }
        public PickerLayout(List<string> existingSelections)
        {
            Orientation = StackOrientation.Vertical;
            StackLayout s2 = new StackLayout { Orientation = StackOrientation.Horizontal };
            Label l = new Label
            {
                Text = "Selectable Items: ",
                VerticalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                HorizontalOptions = LayoutOptions.Start
            };
            itemToAdd = new Entry { Placeholder = "Selection", HorizontalOptions = LayoutOptions.FillAndExpand };
            Button b = new Button { Text = "Add", HorizontalOptions = LayoutOptions.End };
            b.Clicked += AddClicked;
            BoxView bv = new BoxView { HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 1, BackgroundColor = Color.Black };
            s2.Children.Add(l); s2.Children.Add(itemToAdd); s2.Children.Add(b);
            Children.Add(s2); Children.Add(bv);
            foreach(string s in existingSelections)
            {
                AddItemToLayout(s);
            }
        }

        public void AddClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(itemToAdd.Text) && !NewEntry.Selections.Contains(itemToAdd.Text))
            {
                NewEntry.Selections.Add(itemToAdd.Text);
                AddItemToLayout(itemToAdd.Text);
                itemToAdd.Text = "";
            }
            else
            {
                page.DisplayAlert("Item", "Item name must be unique and cannot be empty.", "Ok");
            }
        }

        public void AddItemToLayout(string text)
        {
            StackLayout s = new StackLayout { Orientation = StackOrientation.Horizontal };
            Label l = new Label { Text = text, HorizontalOptions = LayoutOptions.CenterAndExpand,
                VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center,
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label))
                };
            Button b = new Button { Text = "Remove", HorizontalOptions = LayoutOptions.End };
            b.Clicked += RemoveClicked;
            s.Children.Add(l); s.Children.Add(b);
            Children.Add(s);
        }

        public void RemoveClicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            StackLayout s = b.Parent as StackLayout;
            Label l = s.Children[0] as Label;
            NewEntry.Selections.Remove(l.Text);
            Children.Remove(s);
        }
    }
}
