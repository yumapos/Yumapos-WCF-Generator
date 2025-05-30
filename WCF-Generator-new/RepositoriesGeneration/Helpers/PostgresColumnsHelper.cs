using System.Text.RegularExpressions;

namespace WCFGenerator.RepositoriesGeneration.Helpers
{
    public static class PostgresColumnsHelper
    {
        public static string Convert(string column)
        {
           var replaced = Regex.Replace(column, "([A-Z])", "_$1").ToLowerInvariant();
           var excludeFirstUnderscope = replaced.Substring(1);
           return excludeFirstUnderscope.Replace("__", "_");
        }
    }
}