using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Models.Attributes;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;

namespace YumaPos.Server.Infrastructure.DataObjects
{
    [DataAccess(TableName = "RecipieItems", FilterKey1 = "CategoryId", TableVersion = "RecipieItemVersions", IsDeleted = true)]
    public class RecipieItem
    {
        [Key]
        public Guid ItemId { get; set; }

        [VersionKey]
        public Guid ItemVersionId { get; set; }

        public bool IsDeleted { get; set; }

        public string CategoryId { get; set; }

    }
}
