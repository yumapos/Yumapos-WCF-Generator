using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Infrastructure;

namespace TestRepositoryGeneration.DataObjects.VersionsRepositories
{
    [DataAccess (TableVersion = "RecipieItemVersions")]
    public class RecipieItem
    {
        [Key]
        public Guid ItemId { get; set; }

        [VersionKey]
        public Guid ItemVersionId { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; }

        public string CategoryId { get; set; }

    }
}
