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
    [DataAccess(TableName = "dbo.Addresses", FilterKey1 = "ExpireDate", FilterKey2 = "Modified,Country,City", FilterKey3 = "Latitude,Longitude", 
        IsDeleted = false, HasSyncState = true)]
    [DataArchive(TableName = "archive.addresses", FilterKey1 = "Modified", FilterKey2 = "Country,City", FilterKey3 = "Country,City,ZipCode", IsDeleted = false)]
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
        [DbIgnore(IgnoreOnUpdate = true)]
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTimeOffset? ExpireDate { get; set; }
        [DbIgnore]
        public string AditionalInfo { get; set; }
        public bool SyncState { get; set; }
    }
}
