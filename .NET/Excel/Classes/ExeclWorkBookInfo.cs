using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;

namespace Code-Snippets.ExcelService
{
    public class ExcelWorkBookInfo
    {
        public ExcelWorkBookInfo(IWorkbook workbook)
        {
            Sheets = new List<SheetInfo>();
            ReadData(workbook);
        }

        public List<SheetInfo> Sheets { get; set; }

        private void ReadData(IWorkbook workbook)
        {
            for (int index = 0; index < workbook.NumberOfSheets; index++)
            {
                var sheet = workbook.GetSheetAt(index);
                IEnumerator rows = sheet.GetRowEnumerator();
                if (sheet.FirstRowNum == 0 && sheet.LastRowNum == 0)
                    continue;
                var sheetInfo = new SheetInfo();
                sheetInfo.SheetName = sheet.SheetName;
                var dt = new DataTable();

                rows.MoveNext();

                IRow header;
                if (workbook is XSSFWorkbook)
                {
                    header = rows.Current as XSSFRow;
                }
                else
                {
                    header = rows.Current as HSSFRow;
                }
                for (int i = 0; i < header.LastCellNum; i++)
                {
                    var cell = header.GetCell(i);
                    if (cell == null)
                        throw new Exception("Empty column name");
                    dt.Columns.Add(cell.ToString());
                    sheetInfo.ColumnNames.Add(cell.ToString());
                }

                while (rows.MoveNext())
                {
                    IRow row;
                    if (workbook is XSSFWorkbook)
                    {
                        row = rows.Current as XSSFRow;
                    }
                    else
                    {
                        row = rows.Current as HSSFRow;
                    }
                    DataRow dr = dt.NewRow();

                    for (int i = 0; i < row.LastCellNum; i++)
                    {
                        var cell = row.GetCell(i);

                        if (cell == null)
                        {
                            dr[i] = null;
                        }
                        else
                        {
                            dr[i] = cell.ToString();
                        }
                    }
                    dt.Rows.Add(dr);
                }
                sheetInfo.Data = dt;
                Sheets.Add(sheetInfo);
            }
        }

        public SheetInfo GetWorkSheet(string name)
        {
            if (Sheets != null)
                return Sheets.FirstOrDefault(s => s.SheetName == name);
            return null;
        }


    }
}