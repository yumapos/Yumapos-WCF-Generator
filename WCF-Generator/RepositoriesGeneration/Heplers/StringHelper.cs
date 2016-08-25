using System;

namespace VersionedRepositoryGeneration.Generator.Heplers
{
    internal  static class StringHelper
    {
        public static string FirstSymbolToLower(string parms)
        {
            char temp = ' ';
            temp = parms[0];
            temp = char.ToLower(temp);
            parms = parms.Substring(1, parms.Length - 1);
            parms = parms.Insert(0, Convert.ToString(temp));
            return parms;
        }

        public static string DeleteLastSymbol(string key)
        {
            return key = key.Substring(0, key.Length - 1);
        }
    }
}
