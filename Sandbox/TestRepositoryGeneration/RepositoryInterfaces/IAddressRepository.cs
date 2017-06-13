using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task InsertAsync(Address address);
        Task<IEnumerable<Address>> GetAllAsync(bool? isDeleted);
        Task<Address> GetByIdAsync(Guid id, bool? isDeleted);
        IEnumerable<Address> GetByModified(DateTimeOffset startModified, DateTimeOffset endModified, bool? isDeleted = false);
        IEnumerable<Address> GetByCountryAndCityAndZipCode(string country, string city, string zipCode, bool? isDeleted = false);
        IEnumerable<Address> GetByModifiedAndCountryAndCity(string country, string city, DateTimeOffset startModified, DateTimeOffset endModified, bool? isDeleted = false);
        Task UpdateByIdAsync(Address address);
        Task RemoveByIdAsync(Address address);
        Task InsertOrUpdateAsync(Address address);
    }

}
