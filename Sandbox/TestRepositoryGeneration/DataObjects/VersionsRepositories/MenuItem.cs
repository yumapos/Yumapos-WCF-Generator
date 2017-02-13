using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Infrastructure;

namespace TestRepositoryGeneration.DataObjects.VersionsRepositories
{
    [DataAccess(TableVersion = "MenuItemVersions", FilterKey1 = "MenuCategoryId", IsDeleted = false)]
    public class MenuItem : RecipieItem
    {
        [Key]
        public Guid MenuItemId { get; set; }

        [VersionKey]
        public Guid MenuItemVersionId { get; set; }
        [DbIgnore]
        public string Name { get; set; }

        [DataMany2Many(EntityType = "TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax", ManyToManyEntytyType = "TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes")]
        [DbIgnore]
        public IEnumerable<int> TaxIds { get; set; }

        public Guid MenuCategoryId { get; set; }
    }
}
