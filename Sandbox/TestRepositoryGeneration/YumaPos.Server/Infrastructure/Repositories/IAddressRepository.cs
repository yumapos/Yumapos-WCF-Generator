﻿using System;
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
        Task<Address> GetByIdAndZipCodeAsync(Guid id, string zipCode);
        Task<IEnumerable<Address>> GetByZipCodeAsync(string zipCode);
        Task<IEnumerable<Address>> GetAllAsync();
        Task InsertAsync(Address address);
        Task UpdateByIdAndZipCodeAsync(Address address);
        Task RemoveByIdAndZipCodeAsync(Address address);
    }


    public interface IAddress2Repository : IRepository<Address2>
    {
        Task<Address2> GetByIdAsync(int id);
        Task<IEnumerable<Address2>> GetAllAsync();
        Task<int> InsertAsync(Address2 address);
        Task<Address2> GetByZipCodeAsync(string code);
        Task<IEnumerable<Address2>> GetByBuildingAsync(string building);
    }
}
