using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using VdrioEForms.Azure;
using VdrioEForms.EFForms;
using Xamarin.Forms;

namespace VdrioEForms.Layouts
{
    public class FormSubmissionLayout:Grid
    {
        public EFForm formSubmission;

        public FormSubmissionLayout()
        {
            
        }

        public FormSubmissionLayout(EFForm form)
        {
            formSubmission = form;
        }

        public static FormSubmissionLayout CreateTitleLayout(EFForm form, bool showDeletedColumns = false)
        {
            FormSubmissionLayout layout = new FormSubmissionLayout();
            List<EFEntry> entriesToLoad;
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = 0 });
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = 1 });
            layout.RowDefinitions.Add(new RowDefinition { Height = 1 });
            layout.RowDefinitions.Add(new RowDefinition { Height = 50 });
            layout.RowDefinitions.Add(new RowDefinition { Height = 1 });
            layout.RowDefinitions.Add(new RowDefinition { Height = GridLength.Star });
            layout.ColumnSpacing = 0;
            layout.RowSpacing = 0;
            layout.Padding = 0;
            /*foreach(EFEntry e in form.Entries)
            {
                e.DecryptEntry();
            }*/
            if (showDeletedColumns)
            {
                entriesToLoad = form.Entries;
            }
            else
            {
                entriesToLoad = form.Entries.FindAll(x=>!x.Deleted);
            }

            int count = 0;
            layout.ColumnDefinitions.Add(new ColumnDefinition { Width = 151 * entriesToLoad.Count });
            ScrollView TitleScroll = CreateTitleStack(entriesToLoad, showDeletedColumns);
            Grid.SetRow(TitleScroll, 1);
            Grid.SetColumn(TitleScroll, 2);
            layout.Children.Add(TitleScroll);
            BoxView leftBox = new BoxView { WidthRequest = 1, BackgroundColor = Color.Black };
            BoxView topBox = new BoxView { HeightRequest = 1, BackgroundColor = Color.Black };
            BoxView bottomBox = new BoxView { HeightRequest = 1, BackgroundColor = Color.Black };
            Grid.SetRow(leftBox, 0);
            Grid.SetRowSpan(leftBox, 3);
            Grid.SetColumn(leftBox, 1);

            Grid.SetRow(topBox, 0);
            Grid.SetColumn(topBox, 1);
            Grid.SetColumnSpan(topBox, 2);
            Grid.SetRow(bottomBox, 2);
            Grid.SetColumn(bottomBox, 1);
            Grid.SetColumnSpan(bottomBox, 2);
            layout.Children.Add(leftBox);
            layout.Children.Add(topBox);
            layout.Children.Add(bottomBox);


            return layout;
            
        }

        public static ScrollView CreateTitleStack(List<EFEntry> entries, bool showDeletedColumns = false)
        {
            ScrollView scrollView = new ScrollView();
            scrollView.Orientation = ScrollOrientation.Horizontal;
            scrollView.Padding = 0;
            StackLayout layout = new StackLayout { Orientation = StackOrientation.Horizontal };
            layout.Spacing = 0;
            scrollView.BackgroundColor = Color.FromHex("#e5514c");
            foreach (EFEntry e in entries.FindAll(x=>!x.Deleted))
            {
                //e.DecryptEntry();

                Label l = new Label { Text = e.EntryName, /*FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),*/
                    VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, TextColor = Color.White
                    , FontAttributes = FontAttributes.Bold, HorizontalOptions = LayoutOptions.FillAndExpand, HorizontalTextAlignment = TextAlignment.Center,
                    LineBreakMode = LineBreakMode.WordWrap};
                l.WidthRequest = 150;
                if ((e.EntryType == 1 || e.EntryType == 8) && !string.IsNullOrEmpty(e.Units) && e.Units != "N/A")
                {
                    l.Text += " (" + e.Units + ")";
                }
                layout.Children.Add(l);
                layout.Children.Add(new BoxView { WidthRequest = 1, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Black });
            }
            if (showDeletedColumns)
            {
                foreach (EFEntry e in entries.FindAll(x => x.Deleted))
                {
                    //e.DecryptEntry();

                    Label l = new Label
                    {
                        Text = e.EntryName + " (Deleted)", /*FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),*/
                        VerticalOptions = LayoutOptions.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Color.White
                        ,
                        FontAttributes = FontAttributes.Bold,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        LineBreakMode = LineBreakMode.WordWrap
                    };
                    l.WidthRequest = 150;
                    if (e.EntryType == 1 && !string.IsNullOrEmpty(e.Units))
                    {
                        l.Text += " (" + e.Units + ")";
                    }
                    layout.Children.Add(l);
                    layout.Children.Add(new BoxView { WidthRequest = 1, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Black });
                }
            }
            scrollView.Content = layout;
            return scrollView;
            
        }

        public static StackLayout CreateItemsLayout(List<EFForm> submissions,EFForm baseForm, bool showDeletedColumns = false)
        {
            StackLayout bigLayout = new StackLayout
            {
                Orientation = StackOrientation.Vertical,
                BackgroundColor = Color.FromHex("#bdbdbd"),
                Spacing = 0,
                HorizontalOptions = LayoutOptions.Start,

                VerticalOptions = LayoutOptions.FillAndExpand,
                Padding = 0
            };
            int sCount = 0;
            List<EFEntry> entriesToUse = new List<EFEntry>();
            if (showDeletedColumns)
            {
                entriesToUse.AddRange(baseForm.Entries.FindAll(x=>!x.Deleted));
                entriesToUse.AddRange(baseForm.Entries.FindAll(x => x.Deleted));
            }
            else
            {
                entriesToUse = baseForm.Entries.FindAll(e => !e.Deleted);
            }
            foreach (EFForm s in submissions)
            {             
                StackLayout layout = new StackLayout { Orientation = StackOrientation.Vertical, BackgroundColor = Color.FromHex("#bdbdbd"), Spacing = 0,
                    HorizontalOptions = LayoutOptions.FillAndExpand, HeightRequest = 56,
                    Padding = 0
                };
                StackLayout itemLayout = new StackLayout { Orientation = StackOrientation.Horizontal, Spacing = 0, HorizontalOptions = LayoutOptions.FillAndExpand,
                    Padding = 0
                };
                /*if(sCount == 0)
                    layout.Children.Add(new BoxView { HeightRequest = 1, HorizontalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Black });*/
                int count = 0;
                foreach (EFEntry e in s.Entries)
                {
                    //e.DecryptEntry();
                    if (string.IsNullOrEmpty(e.EntryData))
                    {
                        e.EntryData = "";
                    }
                }
                List<EFEntry> EntriesToLoad;
                //if (showDeletedColumns)
                //{
                    EntriesToLoad = s.Entries;
                //}
               // else
               // {
                    //EntriesToLoad = s.Entries.FindAll(e => !baseForm.Entries.Find(x=>x.EntryID == e.EntryID).Deleted);
                //}
                


                for (int i = 0; i < entriesToUse.Count; i++)
                {
                    //EFEntry e in EntriesToLoad

                    EFEntry loadingEntry = EntriesToLoad.Find(e => e.EntryID == entriesToUse[i].EntryID);
                    if (loadingEntry == null)
                    {
                        loadingEntry = new EFEntry { EntryData = "N/A" };
                    }
                    if (count == 0)
                    {
                        itemLayout.Children.Add(new BoxView { WidthRequest = 1, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Black });
                    }
                    //Debug.WriteLine(loadingEntry.EntryData);
                    Label l = new Label
                    {
                        Text = loadingEntry.EntryData,
                        //FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                        VerticalOptions = LayoutOptions.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        HorizontalTextAlignment = TextAlignment.Center,
                        WidthRequest = 150,
                        HeightRequest = 55
                    };
                    itemLayout.Children.Add(l);
                    itemLayout.Children.Add(new BoxView { WidthRequest = 1, VerticalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Black });
                    count++;
                }
                count = 0;
                layout.Children.Add(itemLayout);
                layout.Children.Add(new BoxView { HeightRequest = 1, HorizontalOptions = LayoutOptions.FillAndExpand, BackgroundColor = Color.Black });
                bigLayout.Children.Add(layout);
                sCount++;           
            }
            return bigLayout;
        }

    }
}
