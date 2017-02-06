using System;
using TestRepositoryGeneration.Infrastructure;

namespace TestRepositoryGeneration.DataObjects.VersionsRepositories
{
    /* On create interface for repository use template for Get[FilterKey1]By[FilterKey2]([FilterKey2])*/
    [DataAccess(TableVersion = "MenuItemToTaxVersions", FilterKey1 = "MenuItemId", FilterKey2 = "TaxId")]
    public class MenuItems2Taxes : ITenantUnrelated
    {
        public Guid MenuItemId { get; set; }

        public Guid MenuItemVersionId { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; }

        public int TaxId { get; set; }

        public Guid TaxVersionId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
