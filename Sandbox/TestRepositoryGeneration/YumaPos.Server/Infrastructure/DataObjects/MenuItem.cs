using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Models.Attributes;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.DataInterfaces;
using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes;

namespace YumaPos.Server.Infrastructure.DataObjects
{
    [DataAccess(TableVersion = "MenuItemVersions", FilterKey1 = "MenuCategoryId", IsDeleted = false)]
    public class MenuItem : RecipieItem
    {
        [Key]
        public Guid MenuItemId { get; set; }

        [VersionKey]
        public Guid MenuItemVersionId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; } 

        [DataMany2Many(EntityType = "YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax", ManyToManyEntityType = "YumaPos.Server.Infrastructure.DataObjects.MenuItems2Taxes")]
        public IEnumerable<int> TaxIds { get; set; }

        [DataOne2Many(OneToManyEntityType = "YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage", EntityKey = "ItemId" )]
        [DbIgnore]
        public IEnumerable<string> RecipeItemsLanguageIds { get; set; }

        public Guid MenuCategoryId { get; set; }
    }
}
