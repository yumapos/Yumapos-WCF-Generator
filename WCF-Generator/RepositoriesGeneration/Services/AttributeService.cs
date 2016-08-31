using System;
using WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes;

namespace VersionedRepositoryGeneration.Generator.Services
{
    internal static class AttributeService
    {
        public static DataAccessAttribute GetDataAccessAttribute(AttributeAndPropeperties dataAccessAttr)
        {
            var isDeleted = dataAccessAttr.GetParameterByKeyName("IsDeleted");

            var attr = new DataAccessAttribute
            {
                TableName = dataAccessAttr.GetParameterByKeyName("TableName"),
                TableVersion = dataAccessAttr.GetParameterByKeyName("TableVersion"),
                IsDeleted = !string.IsNullOrEmpty(isDeleted) && Convert.ToBoolean(isDeleted),
                FilterKey1 = dataAccessAttr.GetParameterByKeyName("FilterKey1"),
                FilterKey2 = dataAccessAttr.GetParameterByKeyName("FilterKey2"),
                FilterKey3 = dataAccessAttr.GetParameterByKeyName("FilterKey3"),
            };

            return attr;
        }

        public static DataMany2ManyAttribute GetMenyToManyAttribute(AttributeAndPropeperties m2mAttr)
        {
            var attr = new DataMany2ManyAttribute
            {
                ManyToManyEntytyType = m2mAttr.GetParameterByKeyName("ManyToManyEntytyType"),
                EntityType = m2mAttr.GetParameterByKeyName("EntityType"),
            };

            return attr;
        }
    }
}
