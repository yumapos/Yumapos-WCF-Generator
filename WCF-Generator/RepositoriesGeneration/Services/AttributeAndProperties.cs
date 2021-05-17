using System;
using System.Collections.Generic;
using System.Linq;
using WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes;

namespace WCFGenerator.RepositoriesGeneration.Services
{
    public class AttributeAndProperties
    {
        public string Name { get; set; }
        public string OwnerElementName { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public string GetParameterByKeyName(string key)
        {
            return Parameters.FirstOrDefault(x => x.Key.ToString().Trim() == key).Value;
        }

        public static explicit operator DataAccessAttribute(AttributeAndProperties obj)
        {
            var isDeleted = obj.GetParameterByKeyName("IsDeleted");
            var identity = obj.GetParameterByKeyName("Identity");
            var hasSyncState = obj.GetParameterByKeyName("HasSyncState");

            
            var attr = new DataAccessAttribute
            {
                TableName = obj.GetParameterByKeyName("TableName"),
                TableVersion = obj.GetParameterByKeyName("TableVersion"),
                IsDeleted = isDeleted !=null ? Convert.ToBoolean(isDeleted) : (bool?) null,
                Identity = !string.IsNullOrEmpty(identity) && Convert.ToBoolean(identity),
                HasSyncState = hasSyncState != null ? Convert.ToBoolean(hasSyncState) : (bool?)null,
                FilterKey1 = obj.GetParameterByKeyName("FilterKey1"),
                FilterKey2 = obj.GetParameterByKeyName("FilterKey2"),
                FilterKey3 = obj.GetParameterByKeyName("FilterKey3")
            };

            return attr;
        }

        public static explicit operator DataMany2ManyAttribute(AttributeAndProperties obj)
        {
            var attr = new DataMany2ManyAttribute
            {
                ManyToManyEntityType = obj.GetParameterByKeyName("ManyToManyEntityType") ?? obj.GetParameterByKeyName("ManyToManyEntytyType"),// TODO REMOVE old name whit typo
                EntityType = obj.GetParameterByKeyName("EntityType"),
            };
            return attr;
        }  
  
    }
}
