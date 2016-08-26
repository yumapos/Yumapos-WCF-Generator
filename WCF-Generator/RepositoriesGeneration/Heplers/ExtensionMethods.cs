using System;

namespace VersionedRepositoryGeneration.Generator.Heplers
{
    internal static class ExtensionMethods
    {
        public static string GetName(this Enum en)
        {
            var t = en.GetType();
            var n = Enum.GetName(t, en);
            return n;
        }

        public static string FirstSymbolToLower(this string str)
        {
            return Char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        public static string SurroundWithComments(this string codeText)
        {
            return "/*\r\n" + codeText + "\r\n*/";
        }

        public static string SurroundWithRegion(this string codeText, string regionName)
        {
            return "#region " + regionName + "\r\n\r\n" + codeText + "\r\n\r\n#endregion";
        }

        public static string SurroundWithQuotes(this string codeText)
        {
            return "\"" + codeText + "\"";
        }

    }
}
