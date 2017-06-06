using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.CustomRepositories.BaseRepositories;
using TestRepositoryGeneration.Infrastructure;
using TestRepositoryGeneration.RepositoryInterfaces;

namespace TestRepositoryGeneration.DataObjects.BaseRepositories
{
    /// <summary>
    ///     Repository interface <see cref="IAddressRepository"/>
    ///     Repository <see cref="AddressRepository"/>
    /// </summary>
    [DataAccess(TableName = "dbo.Addresses", FilterKey1 = "Country", FilterKey2 = "Country,City", FilterKey3 = "Country,City,ZipCode", IsDeleted = false)]
    [DataArchive(TableName = "archive.addresses", FilterKey1 = "Country", FilterKey2 = "Country,City", FilterKey3 = "Country,City,ZipCode", IsDeleted = false)]
    public class Address : ITenantUnrelated
    {
        [Key]
        public Guid Id { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string ZipCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        [DbIgnore]
        public string AditionalInfo { get; set; }
    }
}
