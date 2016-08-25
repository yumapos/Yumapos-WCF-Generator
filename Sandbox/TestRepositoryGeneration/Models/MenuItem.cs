using System;
using System.Collections.Generic;
using VersionedRepositoryGeneration.Models.Attributes;

namespace VersionedRepositoryGeneration.Models
{
    [DataAccess(TableVersion = "MenuItemVersions", FilterKey1 = "MenuCategoryId", IsDeleted = false)]
    public class MenuItem : RecipieItem
    {
        public string Name { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; } 

        [DataMany2Many(Type = typeof(Tax), Name = "MenuItemsToTaxes")]
        public IEnumerable<Guid> TaxesId { get; set; }

        public Guid MenuCategoryId { get; set; }
    }

}
