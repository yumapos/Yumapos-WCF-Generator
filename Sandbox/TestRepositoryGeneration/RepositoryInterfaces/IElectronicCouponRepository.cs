using System;
using System.Collections.Generic;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface IElectronicCouponRepository : IRepository<ElectronicCoupon>
    {
        IEnumerable<ElectronicCoupon> Get(DateTime now);
        IEnumerable<ElectronicCoupon> GetAll(bool? isDeletd);
    }
}