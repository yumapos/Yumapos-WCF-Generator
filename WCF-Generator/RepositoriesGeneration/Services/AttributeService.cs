using System;
using VersionedRepositoryGeneration.Models.Attributes;

namespace VersionedRepositoryGeneration.Generator.Services
{
    internal static class AttributeService
    {
        public static DataAccessAttribute GetDataAccessAttribute(AttributeAndPropeperties dataAccessAttr)
        {
            var identity = dataAccessAttr.GetParameterByKeyName("Identity");

            var attr = new DataAccessAttribute
            {
                TableName = dataAccessAttr.GetParameterByKeyName("TableName"),
                TableVersion = dataAccessAttr.GetParameterByKeyName("TableVersion"),
                IsIdentity = !string.IsNullOrEmpty(identity) && Convert.ToBoolean(identity),
                FilterKey1 = dataAccessAttr.GetParameterByKeyName("FilterKey1"),
                FilterKey2 = dataAccessAttr.GetParameterByKeyName("FilterKey2"),
                FilterKey3 = dataAccessAttr.GetParameterByKeyName("FilterKey3"),
            };

            return attr;
        }
    }
}
