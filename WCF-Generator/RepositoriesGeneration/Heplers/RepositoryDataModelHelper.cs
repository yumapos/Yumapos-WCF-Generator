using System.ComponentModel.DataAnnotations;
using VersionedRepositoryGeneration.Models.Attributes;

namespace VersionedRepositoryGeneration.Generator.Heplers
{
    internal static class RepositoryDataModelHelper
    {
        #region KeyAttribute

        public static string KeyAttributeName
        {
            get { return typeof(KeyAttribute).Name; }
        }

        #endregion

        #region DbIgnore

        public static string DbIgnoreAttributeName
        {
            get { return typeof(DbIgnoreAttribute).Name; }
        }

        #endregion

        #region DataAccessAttribute

        public static string DataAccessAttributeName
        {
            get { return typeof(DataAccessAttribute).Name; }
        }

        #endregion

        #region DataFilterAttribute

        public static string DataFilterAttributeName
        {
            get { return typeof(DataFilterAttribute).Name; }
        }


        #endregion

        #region DataMany2ManyAttribute

        public static string DataMany2ManyAttributeName
        {
            get { return typeof(DataMany2ManyAttribute).Name; }
        }

        #endregion


    }
}
