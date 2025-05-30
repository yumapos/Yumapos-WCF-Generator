using WCFGenerator.RepositoriesGeneration.Core.SQL;

namespace WCFGenerator.RepositoriesGeneration.Helpers
{
    public static class CustomSqlModelHelper
    {
        internal static bool IsVersionTableJoined(this SqlInfo sqlInfo)
        {
            return !string.IsNullOrEmpty(sqlInfo.VersionKeyName) && !string.IsNullOrEmpty(sqlInfo.JoinVersionKeyName);
        }
    }
}
