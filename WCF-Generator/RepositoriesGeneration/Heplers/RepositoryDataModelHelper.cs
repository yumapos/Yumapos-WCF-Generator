using System.ComponentModel.DataAnnotations;
using WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes;
using VersionKeyAttribute = WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes.VersionKeyAttribute;

namespace VersionedRepositoryGeneration.Generator.Heplers
{
    internal static class RepositoryDataModelHelper
    {
        #region KeyAttribute

        public static string KeyAttributeName
        {
            get { return typeof(KeyAttribute).Name.Replace("Attribute", ""); }
        }

        public static string VesionKeyAttributeName { get { return typeof(VersionKeyAttribute).Name; } }

        #endregion

        #region DbIgnore

        public static string DbIgnoreAttributeName
        {
            get { return typeof(DbIgnoreAttribute).Name.Replace("Attribute", ""); }
        }

        #endregion

        #region DataAccessAttribute

        public static string DataAccessAttributeName
        {
            get { return typeof(DataAccessAttribute).Name.Replace("Attribute", ""); }
        }

        #endregion

        #region DataMany2ManyAttribute

        public static string DataMany2ManyAttributeName
        {
            get { return typeof(DataMany2ManyAttribute).Name.Replace("Attribute", ""); }
        }


        #endregion


    }
}
