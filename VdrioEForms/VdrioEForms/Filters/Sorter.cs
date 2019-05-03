using System;
using System.Collections.Generic;
using System.Text;
using VdrioEForms.EFForms;
using System.Linq;
using System.Globalization;

namespace VdrioEForms.Filters
{
    public class Sorter
    {
        public EFEntry Entry { get; set; }
        public enum ComparisonType
        {
            Descending = 0, Ascending = 1,
        }
        public ComparisonType Comparison { get; set; }
        public EFForm FormType { get; set; }

        public static List<EFForm> SortSubmissions(List<EFForm> submissions, Sorter sorter)
        {
            List<EFForm> sortedSubs = new List<EFForm>();
             
            if (sorter.Entry.EntryType == 1)
            {
                Dictionary<string, float> values = new Dictionary<string, float>();
                foreach (EFForm f in submissions)
                {
                    float.TryParse(f.Entries.Find(x => x.EntryID == sorter.Entry.EntryID).EntryData, out float val);
                    values.Add(f.RowKey, val);
                }
                List<KeyValuePair<string, float>> sortedValues = values.OrderByDescending(v => v.Value).ToList();
                for (int i = 0; i < submissions.Count; i++)
                {
                    string key = sortedValues[i].Key;
                    sortedSubs.Insert(i, submissions.Find(x => x.RowKey == key));
                }
                if (sorter.Comparison == ComparisonType.Ascending)
                {
                    sortedSubs.Reverse();
                }
            }
            else if (sorter.Entry.EntryType == 4 || sorter.Entry.EntryType == 6 || sorter.Entry.EntryType == 5)
            {
                Dictionary<string, DateTime> values = new Dictionary<string, DateTime>();
                foreach (EFForm f in submissions)
                {
                    if (sorter.Entry.EntryType == 4)
                    {
                        DateTime.TryParseExact(f.Entries.Find(x => x.EntryID == sorter.Entry.EntryID).EntryData, "MM/dd/yyyy",
                            CultureInfo.InvariantCulture,System.Globalization.DateTimeStyles.None ,out DateTime val);
                        values.Add(f.RowKey, val);
                    }
                    else if (sorter.Entry.EntryType == 5)
                    {
                        DateTime.TryParseExact(f.Entries.Find(x => x.EntryID == sorter.Entry.EntryID).EntryData, "hh:mm tt",
                            CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime val);
                        values.Add(f.RowKey, val);
                    }
                    else if (sorter.Entry.EntryType == 6)
                    {
                        DateTime.TryParseExact(f.Entries.Find(x => x.EntryID == sorter.Entry.EntryID).EntryData, "MM/dd/yyyy hh:mm tt",
                            CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime val);
                        values.Add(f.RowKey, val);
                    }
                }
                List<KeyValuePair<string, DateTime>> sortedValues = values.OrderByDescending(v => v.Value).ToList();

                for (int i = 0; i < submissions.Count; i++)
                {
                    string key = sortedValues[i].Key;
                    sortedSubs.Insert(i, submissions.Find(x => x.RowKey == key));
                }
                if (sorter.Comparison == ComparisonType.Ascending)
                {
                    sortedSubs.Reverse();
                }
            }
            return sortedSubs;
        }

        
    }
}
