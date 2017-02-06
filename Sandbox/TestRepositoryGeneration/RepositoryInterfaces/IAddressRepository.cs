using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<Guid> InsertAsync(Address address);
        Task<IEnumerable<Address>> GetAllAsync();
        Task<Address> GetByIdAsync(Guid id);
        Task<Address> GetByCountryAsync(string country);
        Task<Address> GetByCountryAndCityAsync(string country, string city);
        Task<Address> GetByCountryAndCityAndZipCodeAsync(string country, string city, string zipCode);
        Task UpdateByIdAsync(Address address);
        Task RemoveByIdAsync(Address address);
    }

}
