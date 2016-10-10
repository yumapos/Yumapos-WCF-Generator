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
        [DbIgnore]
        public string Name { get; set; }

        [DataMany2Many(EntityType = "YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax", ManyToManyEntytyType = "YumaPos.Server.Infrastructure.DataObjects.MenuItems2Taxes")]
        [DbIgnore]
        public IEnumerable<int> TaxIds { get; set; }

        public Guid MenuCategoryId { get; set; }
    }
}
