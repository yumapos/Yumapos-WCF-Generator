using System.Collections.Generic;
using System.Linq;

namespace VersionedRepositoryGeneration.Generator.Heplers
{
    internal static class SystemToSqlTypeMapper
    {
        private static readonly Dictionary<string, string> SystemTypesToSqlTypes = new Dictionary<string, string>
        {
            {"System.Guid","uniqueidentifier"},
            {"Guid","uniqueidentifier"},
            {"System.Int32","int"},
            {"Int32","int"},
            {"int","int"},
        };

        public static string GetSystemType(string sqlType)
        {
            return SystemTypesToSqlTypes.FirstOrDefault(x => x.Value == sqlType).Key;
        }

        public static string GetSqlType(string systemType)
        {
            return SystemTypesToSqlTypes.FirstOrDefault(x => x.Key == systemType).Value;
        }
    }
}