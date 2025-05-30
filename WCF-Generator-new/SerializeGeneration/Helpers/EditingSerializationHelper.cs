namespace WCFGenerator.SerializeGeneration.Helpers
{
    public class EditingSerializationHelper
    {
        public static string FirstSymbolToLower(string parms)
        {
            if (string.IsNullOrEmpty(parms))
            {
                return string.Empty;
            }
            return char.ToLower(parms[0]) + parms.Substring(1);
        }

        private static string FirstSymbolToUpper(string parms)
        {
            if (string.IsNullOrEmpty(parms))
            {
                return string.Empty;
            }
            return char.ToUpper(parms[0]) + parms.Substring(1);
        }

        private static string DeleteFirstSymbol(string parms)
        {
            parms = parms.Substring(1);
            return parms;
        }

        public static string GetPropertyName(string variableName)
        {
            if (variableName.StartsWith("_"))
            {
                variableName = DeleteFirstSymbol(variableName);
            }
            if (char.IsLower(variableName[0]))
            {
                variableName = FirstSymbolToUpper(variableName);
            }
            return variableName;
        }
    }
}
