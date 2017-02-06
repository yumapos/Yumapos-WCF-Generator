using System;
using System.Collections.Generic;
using TestRepositoryGeneration.DataObjects.BaseRepositories;
using TestRepositoryGeneration.Infrastructure;
using TestRepositoryGeneration.RepositoryInterfaces;

namespace TestRepositoryGeneration.CustomRepositories.BaseRepositories
{
    public partial class ElectronicCouponRepository : RepositoryBase, IElectronicCouponRepository
    {
        public ElectronicCouponRepository(IDataAccessService dataAccessService)
            : base(dataAccessService)
        {
        }

        public IEnumerable<ElectronicCoupon> Get(DateTime now)
        {
            throw new NotImplementedException();
        }
    }
}
