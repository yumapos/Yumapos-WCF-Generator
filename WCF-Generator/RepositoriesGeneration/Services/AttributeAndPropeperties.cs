using System;
using System.Collections.Generic;
using System.Linq;
using WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes;

namespace WCFGenerator.RepositoriesGeneration.Services
{
    internal class AttributeAndPropeperties
    {
        public string Name { get; set; }
        public string OwnerElementName { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public string GetParameterByKeyName(string key)
        {
            return Parameters.FirstOrDefault(x => x.Key.ToString().Trim() == key).Value;
        }

        public static explicit operator DataAccessAttribute(AttributeAndPropeperties obj)
        {
            var isDeleted = obj.GetParameterByKeyName("IsDeleted");

            var attr = new DataAccessAttribute
            {
                TableName = obj.GetParameterByKeyName("TableName"),
                TableVersion = obj.GetParameterByKeyName("TableVersion"),
                IsDeleted = !string.IsNullOrEmpty(isDeleted) && Convert.ToBoolean(isDeleted),
                FilterKey1 = obj.GetParameterByKeyName("FilterKey1"),
                FilterKey2 = obj.GetParameterByKeyName("FilterKey2"),
                FilterKey3 = obj.GetParameterByKeyName("FilterKey3"),
            };

            return attr;
        }

        public static explicit operator DataMany2ManyAttribute(AttributeAndPropeperties obj)
        {
            var attr = new DataMany2ManyAttribute
            {
                ManyToManyEntytyType = obj.GetParameterByKeyName("ManyToManyEntytyType"),
                EntityType = obj.GetParameterByKeyName("EntityType"),
            };
            return attr;
        }
    }
}
