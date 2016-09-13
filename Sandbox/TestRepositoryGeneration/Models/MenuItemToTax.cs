using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Models.Attributes;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.DataInterfaces;

namespace YumaPos.Server.Infrastructure.DataObjects
{
    /* On create interface for repository use template for Get[FilterKey1]By[FilterKey2]([FilterKey2])*/
    [DataAccess(TableVersion = "MenuItemToTaxVersions", FilterKey1 = "ItemId", FilterKey2 = "TaxId")]
    public class MenuItemToTax : ITenantUnrelated
    {
        [Key]
        public Guid ItemId { get; set; }

        public Guid ItemVersionId { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; }

        [Key]
        public Guid TaxId { get; set; }

        public Guid TaxVersionId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
