using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Models.Attributes;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.DataInterfaces;

namespace YumaPos.Server.Infrastructure.DataObjects
{
    [DataAccess(TableName = "RecipieItemsLanguages", TableVersion = "RecipieItemsLanguagesVersions", FilterKey1 = "ItemId")]
    public class RecipeItemsLanguage
    {
        [Required]
        [Key]
        public Guid ItemId { get; set; }

        [Required]
        [StringLength(2)]
        [Key]
        public string Language { get; set; }

        public string Description { get; set; }

        [Required]
        public string Name { get; set; }

        public bool IsDeleted { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; }

        [VersionKey]
        public Guid ItemIdVersionId { get; set; }
    }
}