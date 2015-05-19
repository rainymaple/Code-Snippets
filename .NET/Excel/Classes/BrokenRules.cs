using System.Collections.Generic;

namespace Code-Snippets.ExcelService
{
    public class BrokenRules
    {
        public BrokenRules()
        {
            RequiredFields = new List<ColumnRules>();
        }
        public List<ColumnRules> RequiredFields { get; set; }
    }
}