using System.Web;

namespace Code-Snippets.ExcelService
{
    public class Globalization
    {
        public const string VirtualPath = "~/Admin/Connector.aspx";
        public static string GetLocalization(string key)
        {
            //It should throw an exception if the intended key not defied in resource file
            return HttpContext.GetLocalResourceObject(VirtualPath, key).ToString();
        }
    }
}
