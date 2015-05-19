using System.Collections.Generic;
using System.Data;

namespace Code-Snippets.ExcelService
{
    public class SheetInfo
    {
        public SheetInfo()
        {
            ColumnNames = new List<string>();
            BrokenRules = new BrokenRules();
        }
        public string SheetName { get; set; }
        public List<string> ColumnNames { get; set; }
        public DataTable Data { get; set; }
        public BrokenRules BrokenRules { get; set; }
    }
}