using System.ComponentModel.DataAnnotations;
using WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes;
using VersionKeyAttribute = WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes.VersionKeyAttribute;

namespace WCFGenerator.RepositoriesGeneration.Helpers
{
    internal static class RepositoryDataModelHelper
    {
        public static string KeyAttributeName
        {
            get { return typeof(KeyAttribute).Name.Replace("Attribute", ""); }
        }

        public static string VesionKeyAttributeName
        {
            get { return typeof(VersionKeyAttribute).Name.Replace("Attribute", ""); }
        }

        public static string DataAccessAttributeName
        {
            get { return typeof(DataAccessAttribute).Name.Replace("Attribute", ""); }
        }

        public static string DataMany2ManyAttributeName
        {
            get { return typeof(DataMany2ManyAttribute).Name.Replace("Attribute", ""); }
        }

    }
}
