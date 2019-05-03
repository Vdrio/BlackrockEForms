using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using VdrioEForms.EFForms;
using VdrioEForms.Filters;
using VdrioEForms.Storage;
using Xamarin.Forms;

namespace VdrioEForms.Export
{
    public class ExportToExcel
    {
        public static async void CreateAndOpenExcelDoc(EFForm baseForm, List<EFForm> formsToLoad,List<Filter> filters, DateTime from, DateTime to, bool showDeletedColumns = false)
        {
            string shortFileName = baseForm.FormName.Replace(" ", "") + DateTime.Now.ToString("MMddyyyHHmmss") + ".xlsx";
            string fileName = Path.Combine(EFStorage.appDataBasePath, "Files", shortFileName);
            LoginPage.Storage.CreateFile(fileName);
            if (Device.RuntimePlatform == "Android")
            {
                fileName = await OpenExcel.CreateAndroidExternalFile(shortFileName);
            }
            if (fileName == "")
            {
                return;
            }
            using (var fs = new FileStream(fileName, FileMode.Create))
            {
                IWorkbook workbook = new XSSFWorkbook();
                ICellStyle boldStyle = workbook.CreateCellStyle();
                ICellStyle boldLeftStyle = workbook.CreateCellStyle();
                ICellStyle boldRightStyle = workbook.CreateCellStyle();
                ICellStyle boldTopBotStyle = workbook.CreateCellStyle();
                ICellStyle medStyle = workbook.CreateCellStyle();
                boldStyle.BorderBottom = BorderStyle.Medium;
                boldStyle.BorderTop = BorderStyle.Medium;
                boldStyle.BorderLeft = BorderStyle.Medium;
                boldStyle.BorderRight = BorderStyle.Medium;
                boldLeftStyle.BorderBottom = BorderStyle.Medium;
                boldLeftStyle.BorderTop = BorderStyle.Medium;
                boldLeftStyle.BorderLeft = BorderStyle.Medium;
                boldLeftStyle.BorderRight = BorderStyle.Thin;
                boldRightStyle.BorderBottom = BorderStyle.Medium;
                boldRightStyle.BorderTop = BorderStyle.Medium;
                boldRightStyle.BorderLeft = BorderStyle.Thin;
                boldRightStyle.BorderRight = BorderStyle.Medium;
                boldTopBotStyle.BorderBottom = BorderStyle.Medium;
                boldTopBotStyle.BorderTop = BorderStyle.Medium;
                boldTopBotStyle.BorderLeft = BorderStyle.Thin;
                boldTopBotStyle.BorderRight = BorderStyle.Thin;
                medStyle.BorderBottom = BorderStyle.Thin;
                medStyle.BorderTop = BorderStyle.Thin;
                medStyle.BorderLeft = BorderStyle.Thin;
                medStyle.BorderRight = BorderStyle.Thin;

                boldStyle.Alignment = HorizontalAlignment.Center;
                boldStyle.VerticalAlignment = VerticalAlignment.Center;
                boldStyle.WrapText = true;
                boldLeftStyle.Alignment = HorizontalAlignment.Center;
                boldLeftStyle.VerticalAlignment = VerticalAlignment.Center;
                boldLeftStyle.WrapText = true;
                medStyle.Alignment = HorizontalAlignment.Center;
                medStyle.VerticalAlignment = VerticalAlignment.Center;
                medStyle.WrapText = true;
                boldTopBotStyle.Alignment = HorizontalAlignment.Center;
                boldTopBotStyle.VerticalAlignment = VerticalAlignment.Center;
                boldTopBotStyle.WrapText = true;
                boldRightStyle.Alignment = HorizontalAlignment.Center;
                boldRightStyle.VerticalAlignment = VerticalAlignment.Center;
                boldRightStyle.WrapText = true;
                IFont boldFont = workbook.CreateFont();
                boldFont.IsBold = true;
                boldTopBotStyle.SetFont(boldFont);
                boldLeftStyle.SetFont(boldFont);
                boldRightStyle.SetFont(boldFont);



                ISheet sheet = workbook.CreateSheet(baseForm.FormName + "_Data_" + DateTime.Now.ToString("MMddyyyyhhmmss"));
       
                    int rowCount = 3; int colCount = 2;
                IRow row1 = sheet.CreateRow(0);
                row1.Height = 750;
                IRow row2 = sheet.CreateRow(1);
                row2.Height = 750;
                sheet.CreateRow(2).Height = 750;
                row1.CreateCell(0);
                row2.CreateCell(1);
                IRow row = sheet.CreateRow(rowCount);
                row.Height = 750;
                    ICell cell;
                cell = row1.CreateCell(2);
                cell.CellStyle = boldStyle;
                cell.SetCellValue("From");
                cell = row1.CreateCell(3);
                cell.CellStyle = boldStyle;
                cell.SetCellValue(from.ToString("MM/dd/yyyy hh:mm tt"));
                cell = row1.CreateCell(4);
                cell.CellStyle = boldStyle;
                cell.SetCellValue("To");
                cell = row1.CreateCell(5);
                cell.CellStyle = boldStyle;
                cell.SetCellValue(to.ToString("MM/dd/yyyy hh:mm tt"));
                int fCount = colCount;
                foreach(Filter f in filters)
                {
                    cell = row2.CreateCell(fCount);
                    cell.CellStyle = boldStyle;
                    string filterString = "";
                    filterString += f.Entry.EntryName + " " + Filter.ComparisonTypeToString(f.Comparison) + " " + f.Entry.EntryData;
                    cell.SetCellValue(filterString);
                    fCount++;
                }
                
                int count = 0;
                List<EFEntry> entries = baseForm.Entries.FindAll(x => !x.Deleted);
                List<EFEntry> dEntries = baseForm.Entries.FindAll(x => x.Deleted);
                foreach (EFEntry d in entries)
                {
                    cell = row.CreateCell(colCount);
                    cell.CellStyle = boldTopBotStyle;
                    if (count == 0)
                    {
                        cell.CellStyle = boldLeftStyle;
                    }
                    if (!showDeletedColumns && count == entries.Count-1)
                    {
                        cell.CellStyle = boldRightStyle;
                    }
                    sheet.SetColumnWidth(colCount, 4500);
                    string name = d.EntryName;
                    if (d.EntryType == 1)
                    {
                        if (!string.IsNullOrEmpty(d.Units) && d.Units != "N/A")
                        {
                            name += " (" + d.Units + ")";
                        }
                    }
                    cell.SetCellValue(name);
                    colCount++;                 
                    count++;
                }
                count = 0;
                if (showDeletedColumns)
                {
                    foreach (EFEntry d in dEntries)
                    {
                        cell = row.CreateCell(colCount);
                        cell.CellStyle = boldTopBotStyle;
                        if (count == dEntries.Count - 1)
                        {
                            cell.CellStyle = boldRightStyle;
                        }
                        sheet.SetColumnWidth(colCount, 4500);
                        string name = d.EntryName;
                        if (d.EntryType == 1)
                        {
                            if (!string.IsNullOrEmpty(d.Units) && d.Units != "N/A")
                            {
                                name += " (" + d.Units + ")";
                            }
                        }
                        cell.SetCellValue(name + " (Deleted)");
                        colCount++;
                        count++;
                    }
                }
                List<EFEntry> entriesToUse = new List<EFEntry>();
                if (showDeletedColumns)
                {
                    entriesToUse.AddRange(baseForm.Entries.FindAll(x => !x.Deleted));
                    entriesToUse.AddRange(baseForm.Entries.FindAll(x => x.Deleted));
                }
                else
                {
                    entriesToUse = baseForm.Entries.FindAll(e => !e.Deleted);
                }
                int subCount = 0;
                foreach(EFForm v in formsToLoad)
                {

                    rowCount++;
                    colCount = 2;
                    IRow r = sheet.CreateRow(rowCount);
                    r.Height = 750;
                    r.CreateCell(0);
                    r.CreateCell(1);
                    for (int i = 0; i < entriesToUse.Count; i++)
                    {
                        EFEntry entryToUse = v.Entries.Find(x => x.EntryID == entriesToUse[i].EntryID);
                        if (entryToUse == null)
                        {
                            entryToUse = new EFEntry { EntryData = "N/A" };
                        }
                            ICell c = r.CreateCell(colCount);
                            c.SetCellValue(entryToUse.EntryData);
                            c.CellStyle = medStyle;
                            colCount++;

                    }
                    subCount++;
                }

                
                workbook.Write(fs);
            }

            if (Device.RuntimePlatform == "Android" || Device.RuntimePlatform == "iOS")
            {
                OpenExcel.OpenExcelDoc(fileName);
            }
            else
            {
                OpenExcel.OpenExcelDoc(shortFileName);
            }
            

        }
    }
}
