using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface IElectronicCouponRepository : IRepository<ElectronicCoupon>
    {
        Task InsertManyAsync(IEnumerable<ElectronicCoupon> now);
        IEnumerable<ElectronicCoupon> Get(DateTime now);
        IEnumerable<ElectronicCoupon> GetAll(bool? isDeletd);
    }
}