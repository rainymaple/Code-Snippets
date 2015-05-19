using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Model;
using PCG.Skovision.DataAccess.Interfaces;
using PCG.Skovision.DataAccess.SqlServer;

namespace Code-Snippets.Connector
{
    public class ConnectorBll
    {
        #region Fields and Properties

        public const string OrganizationHeader = "Organization";
        public const string IndicatorHeader = "Indicator";
        public const string ValueHeader = "Value";
        public const string DateHeader = "Date";
        public const string TargetHeader = "Target";
        public const string InitialValueHeader = "Initial Value";
        public const string TargetValueHeader = "Target Value";
        public const string OnTargetRangeHeader = "On Target Range";
        public const string OffTargetRangeHeader = "Off Target Range";
        public const string RangeTypeHeader = "Range Type";

        private readonly ExcelService _excelService;
        private ExcelImportResult _excelImportResult;
        private const int FirstRow = 2;
        private readonly IIndicatorImport _dataService;
        public Dictionary<string, string> GlobalizationColumnNames { get; set; }
        public Dictionary<string, string> Mappings { get; set; }
        public FileTemplate FileTemplate { get; set; }
        public OrganizationNameType OrganizationNameType { get; set; }


        #endregion

        #region Constructor
        public ConnectorBll()
        {
            GetGlobalizationColumnNames();
            _excelService = new ExcelService();
            _dataService = new IndicatorImportUpdate();
        }


        #endregion

        #region Import Excel Data and Validate Excel Data Format
        public ExcelImportResult ImportAndValidateExcel(string excelFileName, byte[] excelDataBytes)
        {
            Mappings = new Dictionary<string, string>
            {
                {OrganizationHeader, Localize("COLUMN_ORGANIZATION")},
                {IndicatorHeader, Localize("COLUMN_INDICATOR")}
            };
            if (FileTemplate == FileTemplate.Import)
            {
                Mappings.Add(ValueHeader, Localize("COLUMN_VALUE"));
                Mappings.Add(DateHeader, Localize("COLUMN_DATE"));
                Mappings.Add(TargetHeader, Localize("COLUMN_TARGET"));
            }else
            {
                Mappings.Add(InitialValueHeader, Localize("COLUMN_INITIAL_VALUE"));
                Mappings.Add(TargetValueHeader, Localize("COLUMN_TARGET_VALUE"));
                Mappings.Add(OnTargetRangeHeader, Localize("COLUMN_ON_TARGET_RANGE"));
                Mappings.Add(OffTargetRangeHeader, Localize("COLUMN_OFF_TARGET_RANGE"));
                Mappings.Add(RangeTypeHeader, Localize("COLUMN_RANGE_TYPE"));
            }
            Mappings.Add("RowNo", "RowNo");

            // get ExcelSheets
            _excelImportResult = _excelService.ImportData(excelFileName, excelDataBytes);
            _excelImportResult.ExcelSheets[0] = AdjustColumnDataTypesWithClone(_excelImportResult.ExcelSheets[0]);

            // validate the generated DataTable
            if (!ValidateSheetDataTable())
            {
                return _excelImportResult;
            }

            // add data format validation rules to ExcelSheets
            AddExcelDataRules();

            // validate against data format rules
            _excelImportResult = _excelService.ValidateRules(_excelImportResult.ExcelSheets);

            return _excelImportResult;
        }

        private bool ValidateSheetDataTable()
        {
            _excelImportResult.BusinessValidateMessage.Clear();

            var sheet = _excelImportResult.ExcelSheets[0];
            if (sheet.Data.Rows.Count == 0)
            {
                _excelImportResult.BusinessValidateMessage.Add(new InvalidMessage
                {
                    Message = Localize("MSG_CANNOT_IMPORT"),
                    InvalidData = null
                });
                return false;
            }
            return true;
        }

        private void AddExcelDataRules()
        {
            var sheet = _excelImportResult.ExcelSheets.First();
            if (sheet != null)
            {
                var fields = sheet.BrokenRules.RequiredFields;

                fields.Add(new ColumnRules { Name = GlobalizationColumnNames[IndicatorHeader], RegExPattern = "" });
                fields.Add(new ColumnRules { Name = GlobalizationColumnNames[OrganizationHeader], RegExPattern = "" });

                if (FileTemplate == FileTemplate.Import)
                {
                    fields.Add(new ColumnRules { Name = GlobalizationColumnNames[ValueHeader], IsDecimal = true });
                    fields.Add(new ColumnRules { Name = GlobalizationColumnNames[DateHeader], IsDate = true });
                    fields.Add(new ColumnRules { Name = GlobalizationColumnNames[TargetHeader], AllowNull = true, IsDecimal = true });
                }
                else
                {
                    fields.Add(new ColumnRules { Name = GlobalizationColumnNames[InitialValueHeader], AllowNull = true, IsDecimal = true });
                    fields.Add(new ColumnRules { Name = GlobalizationColumnNames[TargetValueHeader], IsDecimal = true });
                    fields.Add(new ColumnRules { Name = GlobalizationColumnNames[OnTargetRangeHeader], IsDecimal = true });
                    fields.Add(new ColumnRules { Name = GlobalizationColumnNames[OffTargetRangeHeader], IsDecimal = true });
                    fields.Add(new ColumnRules { Name = GlobalizationColumnNames[RangeTypeHeader], AllowedValue = new List<string> { Localize("RANGE_TYPE_VALUE"), Localize("RANGE_TYPE_PERCENTAGE") } });
                }
            }
        }
        #endregion

        #region Validate Business Rules
        public ExcelImportResult ValidateBusinessRules(ExcelImportResult excelImportResult)
        {
            _excelImportResult.BusinessValidateMessage.Clear();

            var sheet = _excelImportResult.ExcelSheets[0];

            // run each business validation
            ValidateIndicator(sheet);
            ValidateOrganization(sheet);
            ValidateIndicatorInstances(sheet);

            _excelImportResult.BusinessValidateMessage =
                _excelImportResult.BusinessValidateMessage.OrderBy(c => c.RowNumber).ToList();
            return _excelImportResult;
        }


        private void ValidateIndicator(SheetInfo sheet)
        {
            var indicator = new Indicator();
            var indicators = indicator.GetAllIndicators(-1);
            List<FieldValue> indicatorsImport = (from DataRow row in sheet.Data.Rows
                                                 select new FieldValue
                                                 {
                                                     RowNumber = (int)row["RowNo"] + FirstRow,
                                                     FieldData = row[GlobalizationColumnNames[IndicatorHeader]].ToString().Trim()
                                                 }).ToList();

            var indicatorsInDb = indicators.Cast<IndicatorInfo>().Select(i => i.Label.Trim());
            var notMatchedList = indicatorsImport.Where(id => indicatorsInDb.All(i => i != id.FieldData.ToString())).ToList();
            AddBusinessValidateMessage(notMatchedList, GlobalizationColumnNames[IndicatorHeader], Localize("MSG_INDICATOR_NOT_DEFINED"));
        }


        private void ValidateOrganization(SheetInfo sheet)
        {
            var ou = new OU();
            var ous = ou.GetAllOUs();
            List<FieldValue> ousImport = (from DataRow row in sheet.Data.Rows
                                          select new FieldValue
                                          {
                                              RowNumber = (int)row["RowNo"] + FirstRow,
                                              FieldData = row[GlobalizationColumnNames[OrganizationHeader]].ToString().Trim()
                                          }).ToList();

            Func<OUInfo, string> selectOuFunc = (ouInfo) =>
            {
                var ouName = ouInfo.Label;
                switch (OrganizationNameType)
                {
                    case OrganizationNameType.ShortName:
                        ouName = ouInfo.ShortName;
                        break;
                    case OrganizationNameType.Code:
                        ouName = ouInfo.Number;
                        break;
                }
                return ouName;
            };

            var ousInDb = ous.Cast<OUInfo>().Select(selectOuFunc);

            var notMatchedList = ousImport.Where(importOu => ousInDb.All(o => o != importOu.FieldData.ToString())).ToList();
            AddBusinessValidateMessage(notMatchedList, GlobalizationColumnNames[OrganizationHeader], Localize("MSG_ORGANIZATION_NOT_DEFINED"));
        }

        private void ValidateIndicatorInstances(SheetInfo sheet)
        {
            // FULLNAME,SHORTNAME,CODE
            var orNameType = OrganizationNameType.ToString().ToUpper();

            var dataTable = _dataService.ValidateIndicatorInstances(sheet.Data, orNameType, FileTemplate.ToString(), Mappings);
            var notMatchedList = (from DataRow row in dataTable.Rows
                                  select new FieldValue
                                  {
                                      RowNumber = (int)row["RowNo"] + FirstRow,
                                      FieldData = row[GlobalizationColumnNames[IndicatorHeader]].ToString().Trim(),
                                      Message = string.Format(Localize("MSG_INDICATOR_NOT_DEPLOYED"), row[GlobalizationColumnNames[OrganizationHeader]])
                                  }).ToList();
            AddBusinessValidateMessage(notMatchedList, GlobalizationColumnNames[IndicatorHeader], Localize("MSG_INDICATOR_NOT_EXIST"));


        }

        private SheetInfo AdjustColumnDataTypesWithClone(SheetInfo sheet)
        {
            sheet.Data.Columns.Add(new DataColumn("RowNo", typeof(int)));
            for (var i = 0; i < sheet.Data.Rows.Count; i++)
            {
                sheet.Data.Rows[i]["RowNo"] = i;
            }
            DataTable dtCloned = sheet.Data.Clone();
            var columns = sheet.Data.Columns;
            for (int colNum = 0; colNum < columns.Count; colNum++)
            {
                var arrayDecimal = new[] { GlobalizationColumnNames[ValueHeader], GlobalizationColumnNames[TargetHeader], GlobalizationColumnNames[InitialValueHeader],
                    GlobalizationColumnNames[TargetValueHeader], GlobalizationColumnNames[OnTargetRangeHeader], GlobalizationColumnNames[OffTargetRangeHeader] };
                var arrayDate = new[] { GlobalizationColumnNames[DateHeader] };
                var arrayInt = new[] { "RowNo" };
                if (arrayDecimal.Contains(columns[colNum].ColumnName))
                {
                    dtCloned.Columns[colNum].DataType = typeof(decimal);
                    continue;
                }
                if (arrayDate.Contains(columns[colNum].ColumnName))
                {
                    dtCloned.Columns[colNum].DataType = typeof(DateTime);
                    continue;

                }
                if (arrayInt.Contains(columns[colNum].ColumnName))
                {
                    dtCloned.Columns[colNum].DataType = typeof(Int32);
                }
            }

            foreach (DataRow row in sheet.Data.Rows)
            {
                dtCloned.ImportRow(row);
            }
            sheet.Data = dtCloned;
            return sheet;
        }

        private void AddBusinessValidateMessage(List<FieldValue> invalidFields, string colName, string msg)
        {
            _excelImportResult.BusinessValidateMessage.AddRange(invalidFields.Select(invalidField => new InvalidMessage
            {
                Message = !string.IsNullOrEmpty(invalidField.Message) ? invalidField.Message : msg,
                ColumnName = colName,
                InvalidData = invalidField.FieldData.ToString(),
                RowNumber = invalidField.RowNumber
            }).ToList());
        }
        #endregion

        # region Database Processing
        public string RunDbProcess(FileTemplate fileTemplate, ExcelImportResult importResult, string ouNameType, ActionForIndicator action, int userId)
        {
            var datatable = importResult.ExcelSheets[0].Data;
            if (fileTemplate == FileTemplate.Import)
            {
                var obj = _dataService.ImportValues(datatable, userId, ouNameType.ToUpper(), action.ToString().ToUpper(), Mappings);
                var ret = string.Format(Localize("TOTAL_RECORDS"), obj.TotalRecords);
                ret += string.Format(Localize(action == ActionForIndicator.Discard ? "TOTAL_DUPLICATED_DISCARDED" : "TOTAL_DUPLICATED_REPLACED"), obj.TotalDuplicated);
                ret += string.Format(Localize("TOTAL_INSERTED"), obj.TotalInserted);
                return ret;
            }
            else
            {
                var obj = _dataService.UpdateThresholds(datatable, userId, ouNameType.ToUpper(), Mappings);
                var ret = string.Format(Localize("TOTAL_RECORDS"), obj.TotalRecords);
                ret += string.Format(Localize("TOTAL_UPDATED"), obj.TotalUpdated);
                return ret;
            }
        }
        public string Rollback(FileTemplate fileTemplate)
        {
            // 'IMPORT' or 'UPDATE'
            var obj = _dataService.RollbackLastTransaction(fileTemplate.ToString().ToUpper());
            return string.Format(Localize("TOTAL_ROLLBACK"), obj.TotalRecords);
        }

        public bool CheckRollbackImportStatus()
        {
            return _dataService.CheckRollbackImportStatus();
        }
        public bool CheckRollbackUpdateStatus()
        {
            return _dataService.CheckRollbackUpdateStatus();
        }
        #endregion

        #region Export to Excel
        public MemoryStream ExportExcel<T>(List<T> dataList) where T : new()
        {
            return _excelService.ToExcelStream(dataList);
        }
        #endregion

        #region Helper Methods

        private string Localize(string name)
        {
            return Globalization.GetLocalization(name);
        }
        private void GetGlobalizationColumnNames()
        {
            GlobalizationColumnNames = new Dictionary<string, string>
            {
                {OrganizationHeader, Localize("COLUMN_ORGANIZATION")},
                {IndicatorHeader, Localize("COLUMN_INDICATOR")},
                {ValueHeader, Localize("COLUMN_VALUE")},
                {DateHeader, Localize("COLUMN_DATE")},
                {TargetHeader, Localize("COLUMN_TARGET")},
                {InitialValueHeader, Localize("COLUMN_INITIAL_VALUE")},
                {TargetValueHeader, Localize("COLUMN_TARGET_VALUE")},
                {OnTargetRangeHeader, Localize("COLUMN_ON_TARGET_RANGE")},
                {OffTargetRangeHeader, Localize("COLUMN_OFF_TARGET_RANGE")},
                {RangeTypeHeader, Localize("COLUMN_RANGE_TYPE")},
                {"RowNo", "RowNo"}
            };
        }
        #endregion
    }
}
