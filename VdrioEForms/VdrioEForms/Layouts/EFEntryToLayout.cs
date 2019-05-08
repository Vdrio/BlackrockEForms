using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using VdrioEForms.EFForms;
using Xamarin.Forms;

namespace VdrioEForms.Layouts
{
    public class EFEntryToLayout
    {
        public static StackLayout CreateEntryLayout(EFForm form, bool showDeleted = false)
        {
            List<EFEntry> entries = form.Entries;
            List<EFEntry> dEntries = form.Entries;
            entries = entries.FindAll(x => !x.Deleted);
            dEntries = dEntries.FindAll(x => x.Deleted);
            
            Debug.WriteLine("converted entries");
            StackLayout layout = new StackLayout { Orientation = StackOrientation.Vertical };
            if (entries != null)
            {
                foreach (EFEntry e in entries)
                {
                    //e.DecryptEntry();
                    AddUserEntry(e, layout);
                }
            }
            if (dEntries != null && showDeleted)
            {
                foreach(EFEntry e in dEntries)
                {
                    e.EntryName += " (Deleted)";
                    AddUserEntry(e, layout);
                }
            }
            return layout;
        }

        public static void AddUserEntry(EFEntry entry, StackLayout parentLayout)
        {
            Debug.WriteLine("checking entry name null  " + entry.EntryName);
            if (!string.IsNullOrEmpty(entry.EntryName))
            {

                Debug.WriteLine("Adding entry: " + entry.EntryName);
                StackLayout childLayout = new EFEntryItem(entry);
                parentLayout.Children.Add(childLayout);
            }

        }

        public static StackLayout CreateEditEntryLayout(EFForm form, EFForm baseForm, bool includeDeleted = false)
        {
            List<EFEntry> entries = baseForm.Entries.FindAll(x => !x.Deleted);
            List<EFEntry> dEntries = baseForm.Entries.FindAll(x=>x.Deleted);
            Debug.WriteLine("converted entries");
            StackLayout layout = new StackLayout { Orientation = StackOrientation.Vertical };
            if (entries != null)
            {
                int count = 0;
                foreach (EFEntry e in entries)
                {
                    //e.DecryptEntry();
                    //baseForm.Entries[count].DecryptEntry();
                    EFEntry entryToUse = form.Entries.Find(x => x.EntryID == e.EntryID);
                    if (entryToUse == null)
                    {
                        entryToUse = new EFEntry { EntryID = e.EntryID,EntryType= e.EntryType, EntryName = e.EntryName, EntryData = "" };
                        entryToUse.EntryID = e.EntryID;
                        Debug.WriteLine("Proper ID: " + e.EntryID + "My ID: " + entryToUse.EntryID);
                    }
                    AddEditEntry(entryToUse, baseForm, layout);
                    //AddEditEntry(form.Entries.Find(x=>x.EntryID == e.EntryID),baseForm,layout);

                    count++;
                }
                if (includeDeleted)
                {
                    foreach (EFEntry e in dEntries)
                    {
                        //e.DecryptEntry();
                        //baseForm.Entries[count].DecryptEntry();
                        
                        EFEntry entryToUse = form.Entries.Find(x => x.EntryID == e.EntryID);
                        if (entryToUse != null)
                        {
                            entryToUse.EntryName += "(Deleted)";
                        }
                        else
                        {
                            entryToUse = new EFEntry { EntryID = e.EntryID, EntryType = e.EntryType, EntryName = e.EntryName, EntryData = "" };
                            entryToUse.EntryID = e.EntryID;
                        }
                        AddEditEntry(entryToUse, baseForm, layout);

                        count++;
                    }
                }
            }
            return layout;
        }

        public static void AddEditEntry(EFEntry entry,EFForm baseForm ,StackLayout parentLayout)
        {
            Debug.WriteLine("checking entry name null  " + entry.EntryName);
            if (!string.IsNullOrEmpty(entry.EntryName))
            {

                Debug.WriteLine("Adding entry: " + entry.EntryName);
                StackLayout childLayout = new EFEntryItem(entry, entry.EntryData);
                parentLayout.Children.Add(childLayout);
            }

        }


        public static StackLayout CreateEditButtonStack(List<EFForm> submissions,EFForm baseForm, bool includeDeleted = false)
        {
            StackLayout buttonStack = new StackLayout { VerticalOptions = LayoutOptions.FillAndExpand, Spacing = 0, BackgroundColor = Color.FromHex("#bdbdbd") };
            StackLayout horizontalTitleStack = new StackLayout {Orientation = StackOrientation.Horizontal, Spacing = 0,HeightRequest = 52, WidthRequest = 75, BackgroundColor = Color.FromHex("#e5514c")
            };
            StackLayout titleStack = new StackLayout { Spacing = 0, HeightRequest = 52, WidthRequest = 75, BackgroundColor = Color.FromHex("#e5514c")};
            titleStack.Children.Add(new BoxView { BackgroundColor = Color.Black, HeightRequest = 1 });
            titleStack.Children.Add(new Label
            {
                Text = "Edit",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Label)),
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                TextColor = Color.White
                    ,
                FontAttributes = FontAttributes.Bold,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                LineBreakMode = LineBreakMode.WordWrap,
                HeightRequest = 50
            });
            titleStack.Children.Add(new BoxView { BackgroundColor = Color.Black, HeightRequest = 1 });
            horizontalTitleStack.Children.Add(new BoxView { BackgroundColor = Color.Black, WidthRequest = 1 });
            horizontalTitleStack.Children.Add(titleStack);
            horizontalTitleStack.Children.Add(new BoxView { BackgroundColor = Color.Black, WidthRequest = 1 });
            buttonStack.Children.Add(horizontalTitleStack);
            List<EFEntry> EntriesToUse = baseForm.Entries;
            int count = 0;
            if (!includeDeleted)
            {
                
                /*foreach(EFEntry e in baseForm.Entries)
                {
                    e.DecryptEntry();
                }*/
                EntriesToUse = baseForm.Entries.FindAll(e => !e.Deleted);
            }
            foreach (EFForm f in submissions)
            {
                if (count == 0)
                {
                    BoxView bView = new BoxView { HeightRequest = 1, BackgroundColor = Color.Transparent };
                    buttonStack.Children.Add(bView);
                }
                StackLayout s = new StackLayout { Orientation = StackOrientation.Horizontal, WidthRequest = 75, Spacing = 0 };
                s.Children.Add(new BoxView { WidthRequest = 1, BackgroundColor = Color.Black });
                EditFormSubButton b = new EditFormSubButton(f, baseForm);
                s.Children.Add(b);
                s.Children.Add(new BoxView { WidthRequest = 1, BackgroundColor = Color.Black });
                buttonStack.Children.Add(s);
                buttonStack.Children.Add(new BoxView { HeightRequest = 1, BackgroundColor = Color.Black });
                count++;
            }
            return buttonStack;
        }
    }
}
