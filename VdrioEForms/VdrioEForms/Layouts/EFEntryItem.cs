using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using VdrioEForms.EFForms;
using Xamarin.Forms;

namespace VdrioEForms.Layouts
{
    public class EFEntryItem:StackLayout
    {
        public EFEntry dataEntry;

        public DatePicker datePicker;
        public TimePicker timePicker;

        public List<string> fileNames;

        public EFEntryItem()
        {

        }

        public EFEntryItem(EFEntry entry, string input = "", bool enabled = true)
        {
            fileNames = new List<string>();
            dataEntry = entry;
            if (entry.EntryType == 0)
            {
                Debug.WriteLine("Adding alphanumeric entry");
                AlphanumericInput(entry, input, enabled);
            }
            else if (entry.EntryType == 1)
            {
                Debug.WriteLine("Adding numeric entry");
                NumericEntry(entry, input, enabled);
            }
            else if (entry.EntryType == 2)
            {
                Debug.WriteLine("Adding toggle entry");
                ToggleEntry(entry, input, enabled);
            }
            else if (entry.EntryType == 3)
            {
                Debug.WriteLine("Adding picker entry");
                PickerEntry(entry, input, enabled);
            }
            else if (entry.EntryType == 4)
            {
                Debug.WriteLine("Adding date entry");
                DateEntry(entry, input, enabled);
            }
            else if (entry.EntryType == 5)
            {
                Debug.WriteLine("Adding time entry");
                TimeEntry(entry, input, enabled);
            }
            else if (entry.EntryType == 6)
            {
                Debug.WriteLine("Adding date/time entry");
                DateTimeEntry(entry, input, enabled);
            }
        }

        //0
        public void AlphanumericInput(EFEntry entry, string input = "", bool enabled = true)
        {
            this.Orientation = StackOrientation.Horizontal;

            Children.Add(new Label { VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, Text = entry.EntryName + ": ", HorizontalOptions = LayoutOptions.StartAndExpand });
            Entry e = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, Text = input, IsEnabled = enabled, WidthRequest = 125 };
            e.TextChanged += EntryTextChanged;
            Children.Add(e);
            Debug.WriteLine("Checking unit null");
            if (!string.IsNullOrEmpty(entry.Units))
            {
                Debug.WriteLine("Checking unit value: " + entry.Units);
                if (entry.Units != "N/A" && entry.Units != "")
                {
                    Debug.WriteLine("Adding units");
                    Children.Add(new Label { VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, Text = entry.Units, HorizontalOptions = LayoutOptions.StartAndExpand });
                }
            }
        }

        //1
        public void NumericEntry(EFEntry entry, string input = "", bool enabled = true)
        {
            this.Orientation = StackOrientation.Horizontal;
            Children.Add(new Label { VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, Text = entry.EntryName + ": ", HorizontalOptions=LayoutOptions.StartAndExpand });
            Entry e = new Entry { HorizontalOptions = LayoutOptions.FillAndExpand, Text = input, IsEnabled = enabled, WidthRequest = 125 };
            e.TextChanged += EntryTextChanged;
            Children.Add(e);
            if (entry.Units != "N/A" && entry.Units != "")
            {
                Children.Add(new Label
                {
                    VerticalOptions = LayoutOptions.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = entry.Units
                    ,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                });
            }
        }

        //2
        public void ToggleEntry(EFEntry entry, string input = "", bool enabled = true)
        {
            this.Orientation = StackOrientation.Horizontal;
            entry.EntryData = "false";
            if (input == "true")
            {
                entry.EntryData = "true";
            }
            
            Children.Add(new Label { VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, Text = entry.EntryName + ": ", HorizontalOptions = LayoutOptions.StartAndExpand });
            Xamarin.Forms.Switch s = new Xamarin.Forms.Switch { HorizontalOptions = LayoutOptions.CenterAndExpand, IsEnabled = enabled };
            if (input == "true")
            {
                entry.EntryData = "true";
                s.IsToggled = true;
            }
            s.Toggled += ToggleChanged;
            Children.Add(s);
        }

        //3
        public void PickerEntry(EFEntry entry, string input = "", bool enabled = true)
        {
            this.Orientation = StackOrientation.Horizontal;
            Children.Add(new Label { VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, Text = entry.EntryName + ": ", HorizontalOptions = LayoutOptions.StartAndExpand });
            Picker p = new Picker { HorizontalOptions = LayoutOptions.FillAndExpand, IsEnabled = enabled };
            //entry.DecryptEntry();
            foreach (string s in entry.Selections)
            {
                p.Items.Add(s);
            }
            if (p.Items.Contains(input))
            {
                p.SelectedItem = input;
                entry.EntryData = input;
            }
            else
            {
                p.SelectedItem = entry.Selections[0];
                entry.EntryData = entry.Selections[0];
            }
            
            p.SelectedIndexChanged += PickerChanged;
            Children.Add(p);
        }

        //4
        public void DateEntry(EFEntry entry, string input = "", bool enabled = true)
        {
            this.Orientation = StackOrientation.Horizontal;
            Children.Add(new Label { VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, Text = entry.EntryName + ": ", HorizontalOptions = LayoutOptions.StartAndExpand });
            DatePicker p = new DatePicker { HorizontalOptions = LayoutOptions.FillAndExpand, IsEnabled = enabled };
            if (!string.IsNullOrEmpty(input))
            {
                DateTime.TryParseExact(input, "MM/dd/yyyy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt);
                p.Date = dt;
            }
            else
            {
                p.Date = DateTime.Now;
            }
            entry.EntryData = p.Date.ToString("MM/dd/yyyy");
            p.DateSelected += DateChanged;
            Children.Add(p);
        }

        //5
        public void TimeEntry(EFEntry entry, string input = "", bool enabled = true)
        {
            this.Orientation = StackOrientation.Horizontal;
            Children.Add(new Label { VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, Text = entry.EntryName + ": ", HorizontalOptions = LayoutOptions.StartAndExpand });
            TimePicker p = new TimePicker { HorizontalOptions = LayoutOptions.FillAndExpand, IsEnabled = enabled };
            p.Time = DateTime.Now.TimeOfDay;
            if (!string.IsNullOrEmpty(input))
            {
                DateTime.TryParseExact(input, "hh:mm tt", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt);
                p.Time = dt.TimeOfDay;
            }
            DateTime td = DateTime.Today;

            td += p.Time;

            entry.EntryData = td.ToString("hh:mm tt");
            p.PropertyChanged += TimeChanged;
            Children.Add(p);
        }

        //6
        public void DateTimeEntry(EFEntry entry, string input = "", bool enabled = true)
        {
            this.Orientation = StackOrientation.Horizontal;
            Children.Add(new Label { VerticalOptions = LayoutOptions.Center, VerticalTextAlignment = TextAlignment.Center, Text = entry.EntryName + ": ", HorizontalOptions = LayoutOptions.StartAndExpand });
            DatePicker d = new DatePicker { HorizontalOptions = LayoutOptions.FillAndExpand, IsEnabled = enabled };
            TimePicker t = new TimePicker { HorizontalOptions = LayoutOptions.FillAndExpand, IsEnabled = enabled };
            datePicker = d;
            timePicker = t;
            d.Date = DateTime.Today;
            t.Time = DateTime.Now.TimeOfDay;
            DateTime dt1 = DateTime.Now;
            if (!string.IsNullOrEmpty(input))
            {
                DateTime.TryParseExact(input, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime dt);
                d.Date = dt;
                t.Time = dt.TimeOfDay;
            }

            
            entry.EntryData = dt1.ToString("MM/dd/yyyy hh:mm tt");
            t.PropertyChanged += DateTimeChanged;
            d.PropertyChanged += DateTimeChanged;
            Children.Add(d);
            Children.Add(t);
        }

        bool justBackspaced = false;

        public void EntryTextChanged(object sender, EventArgs e)
        {
            if (justBackspaced) { justBackspaced = false; return; }

            Debug.WriteLine("text changed in entry");
            Entry entry = (Entry)sender;
            EFEntryItem item = entry.Parent as EFEntryItem;
            if (item.dataEntry.EntryType == 0)
            {
                item.dataEntry.EntryData = entry.Text;
            }
            else if (item.dataEntry.EntryType == 1)
            {
                Debug.WriteLine("checking num entry");
                if (TestNumericInput(entry))
                {
                    item.dataEntry.EntryData = entry.Text;
                }
            }
        }

        public void ToggleChanged(object sender, EventArgs e)
        {
            Xamarin.Forms.Switch swtch = (Xamarin.Forms.Switch)sender;
            EFEntryItem item = swtch.Parent as EFEntryItem;
            if (swtch.IsToggled)
            {
                item.dataEntry.EntryData = "true";
            }
            else
            {
                item.dataEntry.EntryData = "false";
            }
        }

        public void PickerChanged(object sender, EventArgs e)
        {
            Picker p = (Picker)sender;
            EFEntryItem item = p.Parent as EFEntryItem;
            item.dataEntry.EntryData = p.SelectedItem.ToString();
        }

        public void DateChanged(object sender, EventArgs e)
        {
            DatePicker p = (DatePicker)sender;
            EFEntryItem item = p.Parent as EFEntryItem;
            item.dataEntry.EntryData = p.Date.ToString("MM/dd/yyyy");
        }

        public void DateTimeChanged(object sender, EventArgs e)
        {
            EFEntryItem item = ((View)sender).Parent as EFEntryItem;
            DatePicker p = item.datePicker;
            TimePicker t = item.timePicker;
           // p.Date += t.Time;
            DateTime dt = p.Date + t.Time;
            item.dataEntry.EntryData = dt.ToString("MM/dd/yyyy hh:mm tt");
        }

        public void TimeChanged(object sender, EventArgs e)
        {
            TimePicker p = (TimePicker)sender;
            EFEntryItem item = p.Parent as EFEntryItem;
            DateTime t = DateTime.Today + p.Time;
            item.dataEntry.EntryData = t.ToString("hh:mm tt");
        }

        public bool TestNumericInput(Entry entry)
        {
            if (float.TryParse(entry.Text, out float result))
            {
                Debug.WriteLine("num entry good");
                return true;
            }
            else if (entry.Text.Length != 0)
            {
                Debug.WriteLine("num entry bad");
                entry.Text = entry.Text.Substring(0, entry.Text.Length - 1);
                justBackspaced = true;
            }

            return false;
        }
    }
}
