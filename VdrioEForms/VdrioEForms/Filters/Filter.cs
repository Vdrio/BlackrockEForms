using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using VdrioEForms.EFForms;

namespace VdrioEForms.Filters
{
    public class Filter
    {
        public EFEntry Entry { get; set; }
        public enum ComparisonType
        {
            Equals = 0, GreaterOrEqual = 1, LessOrEqual = 2
        }
        public ComparisonType Comparison { get; set; }
        public EFForm FormType { get; set; }


        public static bool CheckFilters(EFForm value, List<Filter> filters)
        {
            bool checks = true;
            foreach (Filter f in filters)
            {
                //value.EntryType = JsonConvert.DeserializeObject<DataType>(value.DataTypeType);
                //f.Entry.DataType = JsonConvert.DeserializeObject<DataType>(f.Entry.DataTypeType);
                //Debug.WriteLine(value.EntryType.Name + " : " + f.Entry.DataType.Name);
                if (value.TableName == f.FormType.TableName)
                {
                    EFEntry e = f.Entry;
                    //value.Entries = JsonConvert.DeserializeObject<List<EFEntry>>(value.ListDataEntries);
                    EFEntry e2 = value.Entries.Find(x => x.EntryID == e.EntryID);

                    if (e.EntryType == 7)
                    {
                        e2 = new EFEntry { EntryData = value.OriginalUser.FirstName + " " + value.OriginalUser.LastName };
                    }
                    else if (e.EntryType == 8)
                    {
                        e2 = new EFEntry { EntryData = value.LastModifiedUser.FirstName + " " + value.LastModifiedUser.LastName };
                    }
                    //Debug.WriteLine("Checking " + f.Entry.Name);
                    if (e.EntryType == 0)
                    {
                        checks = CheckAlphanumeric(e, e2, f.Comparison);
                    }
                    else if (e.EntryType == 1)
                    {
                        checks = CheckNumeric(e, e2, f.Comparison);
                    }
                    else if (e.EntryType == 2)
                    {
                        checks = CheckToggle(e, e2, f.Comparison);
                    }
                    else if (e.EntryType == 3)
                    {
                        checks = CheckSelection(e, e2, f.Comparison);
                        Debug.WriteLine("Checking " + e.EntryData + " : " + e2.EntryData);
                    }
                    else if (e.EntryType == 4)
                    {
                        checks = CheckDate(e, e2, f.Comparison);
                    }
                    else if (e.EntryType == 5)
                    {
                        checks = CheckTime(e, e2, f.Comparison);
                    }
                    else if (e.EntryType == 6)
                    {
                        checks = CheckDateTime(e, e2, f.Comparison);
                    }
                    else if (e.EntryType == 7)
                    {
                        checks = CheckUser(e, e2, f.Comparison);
                    }
                    else if (e.EntryType == 8)
                    {
                        checks = CheckUser(e, e2, f.Comparison);
                    }
                    if (!checks)
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            return checks;
        }

        public static bool CheckAlphanumeric(EFEntry e1, EFEntry e2, ComparisonType comparison)
        {
            if (comparison == ComparisonType.Equals)
            {
                if (e1.EntryData == e2.EntryData)
                {
                    return true;
                }
            }
            return false;
        }

        public static bool CheckNumeric(EFEntry ef, EFEntry e, ComparisonType comparison)
        {
            if (float.TryParse(ef.EntryData, out float efv))
            {
                if (float.TryParse(e.EntryData, out float ev))
                {
                    if (comparison == ComparisonType.Equals)
                    {
                        return ev == efv;
                    }
                    else if (comparison == ComparisonType.GreaterOrEqual)
                    {
                        return ev >= efv;
                    }
                    else if (comparison == ComparisonType.LessOrEqual)
                    {
                        return ev <= efv;
                    }
                    else
                    {
                        return false;
                    }
                }
                else { return false; }
            }
            else
            {
                return false;
            }
        }

        public static bool CheckToggle(EFEntry ef, EFEntry e, ComparisonType comparison)
        {
            Debug.WriteLine(ef.EntryData + ", "+ e.EntryData);
            try
            {
                return ef.EntryData == e.EntryData;
            }
            catch
            {
                return false;
            }

        }

        public static bool CheckSelection(EFEntry ef, EFEntry e, ComparisonType comparison)
        {
            try
            {
                return ef.EntryData == e.EntryData;
            }
            catch { return false; }

        }

        public static bool CheckDate(EFEntry ef, EFEntry e, ComparisonType comparison)
        {
            
            if (DateTime.TryParseExact(ef.EntryData,"MM/dd/yyyy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None
                , out DateTime eft) && DateTime.TryParseExact(e.EntryData, "MM/dd/yyyy", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime et))
            {
                if (comparison == ComparisonType.Equals)
                {
                    return eft.Month == et.Month && eft.Day == et.Day && eft.Year == et.Year;
                }
                else if (comparison == ComparisonType.GreaterOrEqual)
                {
                    return et.Year > eft.Year || et.Year == eft.Year && et.Month > eft.Month || et.Year == eft.Year && et.Month == eft.Month && et.Day >= eft.Day;
                }
                else if (comparison == ComparisonType.LessOrEqual)
                {
                    return et.Year < eft.Year || et.Year == eft.Year && et.Month < eft.Month || et.Year == eft.Year && et.Month == eft.Month && et.Day <= eft.Day;
                }
            }
            return false;
        }

        public static bool CheckDateTime(EFEntry ef, EFEntry e, ComparisonType comparison)
        {
            if (DateTime.TryParseExact(ef.EntryData, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None
                , out DateTime eft) && DateTime.TryParseExact(e.EntryData, "MM/dd/yyyy hh:mm tt", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime et))
            {
                if (comparison == ComparisonType.Equals)
                {
                    return eft.Month == et.Month && eft.Day == et.Day && eft.Year == et.Year && eft.Hour == et.Hour && eft.Minute == et.Minute;
                }
                else if (comparison == ComparisonType.GreaterOrEqual)
                {
                    return et.Year > eft.Year || et.Year == eft.Year && et.Month > eft.Month || et.Year == eft.Year && et.Month == eft.Month && et.Day > eft.Day ||
                        et.Year == eft.Year && et.Month == eft.Month && et.Day == eft.Day && et.Hour > eft.Hour ||
                        et.Year == eft.Year && et.Month == eft.Month && et.Day == eft.Day && eft.Hour == et.Hour && et.Minute >= eft.Minute;
                }
                else if (comparison == ComparisonType.LessOrEqual)
                {
                    return et.Year < eft.Year || et.Year == eft.Year && et.Month < eft.Month || et.Year == eft.Year && et.Month == eft.Month && et.Day < eft.Day ||
                        et.Year == eft.Year && et.Month == eft.Month && et.Day == eft.Day && et.Hour < eft.Hour ||
                        et.Year == eft.Year && et.Month == eft.Month && et.Day == eft.Day && eft.Hour == et.Hour && et.Minute <= eft.Minute;
                }
            }
            return false;
        }

        public static bool CheckTime(EFEntry ef, EFEntry e, ComparisonType comparison)
        {
            if (DateTime.TryParseExact(ef.EntryData, "hh:mm tt", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None
                , out DateTime eft) && DateTime.TryParseExact(e.EntryData, "hh:mm tt", CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime et))
            {
                if (comparison == ComparisonType.Equals)
                {
                    return eft.Hour == et.Hour && eft.Minute == et.Minute;
                }
                else if (comparison == ComparisonType.GreaterOrEqual)
                {
                    return et.Hour > eft.Hour ||
                        (eft.Hour == et.Hour && et.Minute >= eft.Minute);
                }
                else if (comparison == ComparisonType.LessOrEqual)
                {
                    return et.Hour < eft.Hour ||(eft.Hour == et.Hour && et.Minute <= eft.Minute);
                }
            }
            return false;
        }

        public static bool CheckUser(EFEntry ef, EFEntry e, ComparisonType comparison)
        {
            return ef.EntryData == e.EntryData;
        }

        public static ComparisonType GetComparisonType(int index)
        {
            if (index == 0)
            {
                return ComparisonType.Equals;
            }
            else if (index == 1)
            {
                return ComparisonType.GreaterOrEqual;
            }
            else if (index == 2)
            {
                return ComparisonType.LessOrEqual;
            }
            else
            {
                return ComparisonType.Equals;

            }
        }

        public static string ComparisonTypeToString(ComparisonType t)
        {
            if (t == ComparisonType.Equals)
            {
                return "=";
            }
            else if (t == ComparisonType.GreaterOrEqual)
            {
                return ">=";
            }
            else if (t == ComparisonType.LessOrEqual)
            {
                return "=";
            }
            return "=";
        }
    }
}
