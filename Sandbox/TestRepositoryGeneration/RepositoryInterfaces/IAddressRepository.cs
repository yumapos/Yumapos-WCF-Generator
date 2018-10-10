using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task InsertAsync(Address address);
        Task InsertManyAsync(IEnumerable<Address> address);
        Task<IEnumerable<Address>> GetAllAsync(bool? isDeleted);
        Task<Address> GetByIdAsync(Guid id, bool? isDeleted);
        //IEnumerable<Address> GetByModified(DateTime startModified, DateTime endModified, bool? isDeleted = false);
        IEnumerable<Address> GetByExpireDate(DateTimeOffset? expireDate, bool? isDeleted = false);
        void RemoveByExpireDate(DateTimeOffset? expireDate);
        IEnumerable<Address> GetByModifiedAndCountryAndCity(string country, string city, DateTime startModified, DateTime endModified, bool? isDeleted = false);
        IEnumerable<Address> GetByLatitudeAndLongitude(decimal? latitude, decimal? longitude, bool? isDeleted = false);
        Task UpdateByIdAsync(Address address);
        Task RemoveByIdAsync(Address address);
        Task RemoveByIdAsync(Guid id);
    }

}
