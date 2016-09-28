using System.ComponentModel.DataAnnotations;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.DataInterfaces;

namespace YumaPos.Server.Infrastructure.DataObjects
{
    [DataAccess(TableName = "dbo.Addresses2", FilterKey1 = "ZipCode", FilterKey2 = "Building")]
    public class Address2 : ITenantUnrelated
    {
        [Key]
        public int Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string ZipCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
    }
}