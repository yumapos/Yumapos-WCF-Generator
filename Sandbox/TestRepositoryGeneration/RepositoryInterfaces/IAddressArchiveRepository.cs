using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface IAddressArchiveRepository : IArchiveRepository<Address>
    {
        Task InsertAsync(Address address);
        Task<IEnumerable<Address>> GetAllAsync(bool? isDeleted);
        Task<Address> GetByIdAsync(Guid id, bool? isDeleted);
        Task<Address> GetByCountryAndCityAsync(string country, string city, bool? isDeleted);
        Task<Address> GetByCountryAndCityAndZipCodeAsync(string country, string city, string zipCode, bool? isDeleted);
        Task UpdateByIdAsync(Address address);
        Task RemoveByIdAsync(Address address);
        IEnumerable<Address> GetByModifiedUtc(DateTime startModifiedUtc, DateTime endModifiedUtc, bool? isDeleted = false);
    }
}