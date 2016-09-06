using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<Address> GetByIdAsync(Guid id);
        Task<IEnumerable<Address>> GetAllAsync();
        Task InsertAsync(Address address);
        Task UpdateByIdAsync(Address address);
        Task RemoveByIdAsync(Address address);
    }

    public partial class AddressRepository : IAddressRepository {
        
        #region Implementation of IAddressRepository

        //public Task<IEnumerable<Address>> GetAllAsync()
        //{
        //    throw new NotImplementedException();
        //}

        #endregion
    }
}
