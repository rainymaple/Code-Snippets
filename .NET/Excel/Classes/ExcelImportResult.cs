using System.Collections.Generic;

namespace Code-Snippets.ExcelService
{
    public class ExcelImportResult
    {
        public List<InvalidMessage> ExcelValidateMessage { get; set; }
        public List<InvalidMessage> BusinessValidateMessage { get; set; }

        public List<SheetInfo> ExcelSheets { get; set; }

    }
}
