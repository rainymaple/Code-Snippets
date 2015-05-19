using System;
using System.Collections.Generic;

namespace Code-Snippets.ExcelService
{
    [Serializable()]
    public class InvalidMessage
    {
        public int? RowNumber { get; set; }
        public string ColumnName { get; set; }
        public string InvalidData { get; set; }
        public string Message { get; set; }
    }
}