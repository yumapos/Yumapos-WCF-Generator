using System.Collections.Generic;
using System.Linq;

namespace WCFGenerator.RepositoriesGeneration.Heplers
{
    internal static class SystemToPostgreSqlTypeMapper
    {
        private static readonly Dictionary<string, string> SystemTypesToPostgresTypes = new Dictionary<string, string>
        {
            {"System.Guid","uuid"},
            {"Guid","uuid"},
            {"System.Int32","integer"},
            {"Int32","integer"},
            {"int","integer"},
        };

        public static string GetSystemType(string sqlType)
        {
            return SystemTypesToPostgresTypes.FirstOrDefault(x => x.Value == sqlType).Key;
        }

        public static string GetPostgreSqlType(string systemType)
        {
            return SystemTypesToPostgresTypes.FirstOrDefault(x => x.Key == systemType).Value;
        }
    }
}