using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;
using TestRepositoryGeneration.Infrastructure;
using TestRepositoryGeneration.RepositoryInterfaces;

namespace TestRepositoryGeneration.CustomRepositories.BaseRepositories
{
    public partial class ElectronicCouponsTypedRepository : RepositoryBase, IElectronicCouponsTypedRepository
    {
        public async Task<ElectronicCouponsTyped> Get(int id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> InsertAsync(ElectronicCouponsTyped electronicCouponsTyped)
        {
            throw new NotImplementedException();

        }

        public async Task UpdateByElectronicCouponsIdAsync(ElectronicCouponsTyped electronicCouponsTyped)
        {
            throw new NotImplementedException();
        }


        public async Task RemoveByElectronicCouponsIdAsync(int electronicCouponsId)
        {
            throw new NotImplementedException();
        }

        public async Task RemoveList(IEnumerable<int> ids)
        {
            throw new NotImplementedException();
        }
    }
}
