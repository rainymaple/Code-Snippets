using System.Collections.Generic;

namespace Code-Snippets.ExcelService
{
    public class ColumnRules
    {
        public string Name { get; set; }
        public bool AllowNull { get; set; }
        public string RegExPattern { get; set; }
        public bool IsDate { get; set; }
        public bool IsDecimal { get; set; }
        public List<string> AllowedValue { get; set; }
    }
}