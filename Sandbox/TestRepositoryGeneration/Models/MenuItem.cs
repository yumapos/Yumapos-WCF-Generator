using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes;

namespace YumaPos.Server.Infrastructure.DataObjects
{
    [DataAccessAttribute(TableVersion = "MenuItemVersions", FilterKey1 = "MenuCategoryId", IsDeleted = false)]
    public class MenuItem : RecipieItem
    {
        public string Name { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; } 

        [DataMany2Many(EntityType = "YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax", ManyToManyEntytyType = "YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax")]
        public IEnumerable<Guid> TaxesId { get; set; }

        public Guid MenuCategoryId { get; set; }
    }

}
