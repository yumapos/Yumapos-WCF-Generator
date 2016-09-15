using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Models.Attributes;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;


namespace YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes
{
    [DataAccess(TableVersion = "TaxVersions")]
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
    }
}
