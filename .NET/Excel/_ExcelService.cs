using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Code-Snippets.DataAccess;

namespace Code-Snippets.ExcelService
{
    public class ExcelService
    {
        protected IWorkbook WorkBook { get; set; }
        public ExcelImportResult ExcelImportResult { get; set; }

        protected ExcelWorkBookInfo ExcelWorkBook { get; set; }

        public ExcelService()
        {
            ExcelImportResult = new ExcelImportResult
            {
                ExcelValidateMessage = new List<InvalidMessage>(),
                BusinessValidateMessage = new List<InvalidMessage>(),
                ExcelSheets = new List<SheetInfo>()
            };
        }


        public ExcelImportResult ValidateRules(List<SheetInfo> sheets)
        {

            if (sheets != null && sheets.Count > 0)
            {
                foreach (var sheet in sheets)
                {
                    if (sheet.BrokenRules != null && sheet.BrokenRules.RequiredFields != null && sheet.BrokenRules.RequiredFields.Count > 0)
                    {
                        var colNames = sheet.BrokenRules.RequiredFields.Select(c => c.Name.Trim().ToLower()).ToList();

                        var missingColumns = colNames.Except(sheet.ColumnNames.Select(c => c.Trim().ToLower())).ToList();
                        if (missingColumns.Any())
                        {
                            var invalidFields = missingColumns.Select(
                                    c => new FieldValue { RowNumber = null, FieldData = "", ColumnName = c.ToString() }).ToList();
                            ExcelImportResult.ExcelValidateMessage.AddRange(
                                        GetInvalidMessage(invalidFields, string.Format(Localize("EXCEL_MISSING_COLUMN"), sheet.SheetName)));
                            return ExcelImportResult;
                        }
                        foreach (var reqCol in sheet.BrokenRules.RequiredFields)
                        {
                            var i = 2;
                            List<FieldValue> values = (from DataRow row in sheet.Data.Rows
                                                       select new FieldValue
                                                       {
                                                           RowNumber = i++,
                                                           FieldData = row[reqCol.Name],
                                                           ColumnName = reqCol.Name
                                                       }).ToList();

                            if (!reqCol.AllowNull)
                            {
                                var invalidFields = values.Where(v => v.FieldData == DBNull.Value).ToList();
                                if (invalidFields.Any())
                                {
                                    ExcelImportResult.ExcelValidateMessage.AddRange(
                                        GetInvalidMessage(invalidFields, Localize("EXCEL_INVALID_EMPTY_VALUE")));
                                }
                            }

                            if (!string.IsNullOrEmpty(reqCol.RegExPattern))
                            {
                                //Check RegEx
                                var invalidFields = values.Where(v => v.FieldData != DBNull.Value
                                     && !Regex.IsMatch(v.FieldData.ToString(), reqCol.RegExPattern)).ToList();
                                if (invalidFields.Any())
                                {
                                    ExcelImportResult.ExcelValidateMessage.AddRange(
                                        GetInvalidMessage(invalidFields, Localize("EXCEL_INVALID_DATA_FORMAT")));
                                }
                            }
                            if (reqCol.IsDate)
                            {
                                DateTime dateTime;
                                var invalidFields = values.Where(v => v.FieldData != DBNull.Value
                                     && !DateTime.TryParse(v.FieldData.ToString(), out dateTime)).ToList();
                                if (invalidFields.Any())
                                {
                                    ExcelImportResult.ExcelValidateMessage.AddRange(
                                        GetInvalidMessage(invalidFields, Localize("EXCEL_INVALID_DATE_VALUE")));
                                }
                            }
                            if (reqCol.IsDecimal)
                            {
                                var invalidFields = values.Where(v => v.FieldData != DBNull.Value
                                     && !Regex.IsMatch(v.FieldData.ToString(), @"^\-{0,1}\d+(.\d+){0,1}$")).ToList();
                                if (invalidFields.Any())
                                {
                                    ExcelImportResult.ExcelValidateMessage.AddRange(
                                        GetInvalidMessage(invalidFields, Localize("EXCEL_NOT_DECIMAL")));
                                }
                            }
                            if (reqCol.AllowedValue != null && reqCol.AllowedValue.Any())
                            {
                                var col = reqCol;
                                var invalidFields = values.Where(v => v.FieldData != DBNull.Value
                                                                      && col.AllowedValue.All(c => c != v.FieldData.ToString())).ToList();
                                if (invalidFields.Any())
                                {
                                    ExcelImportResult.ExcelValidateMessage.AddRange(
                                        GetInvalidMessage(invalidFields, Localize("EXCEL_VAULE_NOT_ALLOWED")));
                                }
                            }


                        }
                    }
                }
                ExcelImportResult.ExcelValidateMessage =
                    ExcelImportResult.ExcelValidateMessage.OrderBy(c => c.RowNumber).ToList();
                return ExcelImportResult;
            }
            throw new Exception(Localize("EXCEL_INVALID_WORKBOOK"));
        }

        private List<InvalidMessage> GetInvalidMessage(List<FieldValue> invalidFields, string msg)
        {
            return invalidFields.Select(invalidField => new InvalidMessage
            {
                Message = msg,
                ColumnName = invalidField.ColumnName,
                InvalidData = invalidField.FieldData.ToString(),
                RowNumber = invalidField.RowNumber
            }).ToList();
        }

        public ExcelImportResult ImportData(string fileName, byte[] bytes = null)
        {
            try
            {
                if (bytes == null)
                {
                    if (string.IsNullOrEmpty(fileName) || !File.Exists(fileName))
                    {
                        throw new FileNotFoundException();
                    }
                    using (FileStream file = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            WorkBook = new HSSFWorkbook(file);
                        }
                        catch (Exception)
                        {
                            WorkBook = new XSSFWorkbook(file);
                        }

                        ExcelWorkBook = new ExcelWorkBookInfo(WorkBook);
                    }

                }
                else
                {
                    MemoryStream mStream;

                    try
                    {
                        using (mStream = new MemoryStream(bytes))
                        {
                            WorkBook = new HSSFWorkbook(mStream);
                        }

                    }
                    catch (Exception)
                    {
                        using (mStream = new MemoryStream(bytes))
                        {
                            WorkBook = new XSSFWorkbook(mStream);
                        }
                    }
                    ExcelWorkBook = new ExcelWorkBookInfo(WorkBook);

                }
            }
            catch (Exception)
            {
                throw new ApplicationException(Localize("EXCEL_FAILED_TO_IMPORT"));
            }

            if (ExcelWorkBook.Sheets == null || ExcelWorkBook.Sheets.Count == 0)
            {
                throw new ApplicationException(Localize("EXCEL_FAILED_TO_IMPORT"));
            }

            ExcelImportResult.ExcelSheets = ExcelWorkBook.Sheets;
            return ExcelImportResult;
        }

        protected virtual string DBProcess()
        {
            return string.Empty;
        }

        public MemoryStream ToExcelStream(string storedProcedure, List<SqlParameter> parameters, string sheetName = "sheet1")
        {
            var dataService = new DataService();

            using (var reader = dataService.GetReader(storedProcedure, parameters))
            {
                var stream = new MemoryStream();
                var workbook = new HSSFWorkbook();
                var sheet1 = workbook.CreateSheet(sheetName);
                int offset = sheet1.LastRowNum + 1;
                //header
                IRow row = sheet1.CreateRow(offset);

                var numberFields = reader.FieldCount;
                for (int i = 0; i < numberFields; i++)
                {
                    row.CreateCell(i).SetCellValue(reader.GetName(i));
                }

                if (reader.HasRows)
                {

                    while (reader.Read())
                    {
                        offset = sheet1.LastRowNum + 1;
                        row = sheet1.CreateRow(offset);

                        for (int i = 0; i < numberFields; i++)
                            row.CreateCell(i).SetCellValue(reader[i] is DBNull ? string.Empty : reader[i].ToString());
                    }
                }
                workbook.Write(stream);
                return stream;
            }
            return null;
        }
        public MemoryStream ToExcelStream<T>(List<T> dataList, List<string> headerList = null, string sheetName = "sheet1") where T : new()
        {

            var stream = new MemoryStream();
            var workbook = new HSSFWorkbook();
            var sheet = workbook.CreateSheet(sheetName);
            // Add header labels
            var colIndex = 0;
            var rowIndex = 0;
            var row = sheet.CreateRow(colIndex);
            if (headerList == null)
            {
                var t = new T();
                var properties = t.GetType().GetProperties();
                headerList = properties.Select(c => c.Name).ToList();
            }
            foreach (var header in headerList)
            {
                row.CreateCell(colIndex++).SetCellValue(header);
            }
            rowIndex++;

            // Add data rows
            foreach (T data in dataList)
            {
                row = sheet.CreateRow(rowIndex);
                colIndex = 0;

                foreach (var header in headerList)
                {
                    var value = data.GetType().GetProperty(header).GetValue(data, null);
                    row.CreateCell(colIndex++).SetCellValue(value==null?string.Empty:value.ToString());
                }
                rowIndex++;
            }
            workbook.Write(stream);
            return stream;

        }

        private string Localize(string name)
        {
            return Globalization.GetLocalization(name);
        }
    }
}
