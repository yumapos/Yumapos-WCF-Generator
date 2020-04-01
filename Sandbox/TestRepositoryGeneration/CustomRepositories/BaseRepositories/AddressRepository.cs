using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;
using TestRepositoryGeneration.RepositoryInterfaces;

namespace TestRepositoryGeneration.CustomRepositories.BaseRepositories
{
    public partial class AddressRepository : IAddressRepository
    {
        // TODO Full type name comparation need YWG-37
        public async void Insert(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
        {
            await DataAccessService.InsertObjectAsync(address, InsertQuery);
        }

        #region Implementation of IOfflineSerializeRepository<IEnumerable<Address>>

        public Task InsertAsync(IEnumerable<Address> obj)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}