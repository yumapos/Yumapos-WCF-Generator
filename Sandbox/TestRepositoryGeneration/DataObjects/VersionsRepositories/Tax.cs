using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Infrastructure;

namespace TestRepositoryGeneration.DataObjects.VersionsRepositories
{
    [DataAccess(TableVersion = "TaxVersions", IsDeleted = false)]
    public class Tax
    {
        [Key]
        public int TaxId { get; set; }

        [VersionKey]
        public Guid TaxVersionId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; } //EmployeeId 

        public bool IsDeleted { get; set; }

        [DataMany2Many(EntityType = "TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem", ManyToManyEntytyType = "TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes")]
        [DbIgnore]
        public IEnumerable<Guid> MenuItemIds { get; set; }
    }
}
