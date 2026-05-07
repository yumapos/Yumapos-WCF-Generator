using System.Collections.Frozen;

namespace WCFGenerator.RepositoriesGeneration.Helpers
{
    internal static class SystemToSqlTypeMapper
    {
        private static readonly FrozenDictionary<string, string> SystemTypesToSqlTypes = new Dictionary<string, string>(comparer: StringComparer.OrdinalIgnoreCase)
        {
            {"System.String","nvarchar(max)"},
            {"string","nvarchar(max)"},
            {"System.Int64","bigint"},
            {"Int64","bigint"},
            {"long","bigint"},
            {"System.Int32","int"},
            {"Int32","int"},
            {"int","int"},
            {"System.Int16","smallint"},
            {"Int16","smallint"},
            {"short","smallint"},
            {"System.Byte","tinyint"},
            {"byte","tinyint"},
            {"System.Boolean","bit"},
            {"bool","bit"},
            {"System.Decimal","decimal(19,5)"},
            {"decimal","decimal(19,5)"},
            {"System.Double","float"},
            {"double","float"},
            {"System.Single","real"},
            {"float","real"},
            {"System.DateTime","datetime2"},
            {"DateTime","datetime2"},
            {"System.DateTimeOffset","datetimeoffset"},
            {"DateTimeOffset","datetimeoffset"},
            {"System.TimeSpan","time"},
            {"TimeSpan","time"},
            {"System.Guid","uniqueidentifier"},
            {"Guid","uniqueidentifier"},
            {"System.Byte[]","varbinary(max)"},
            {"byte[]","varbinary(max)"},
        }.ToFrozenDictionary();
        
        public static string GetSqlType(string systemType)
        {
            if (SystemTypesToSqlTypes.TryGetValue(systemType, out var sqlType))
            {
                return sqlType;
            }

            return "";
        }
    }
}