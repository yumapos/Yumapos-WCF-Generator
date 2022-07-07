using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface IStoresRepository : IRepository<Store>
    {
        Task InsertAsync(Store store);
        Task InsertManyAsync(IEnumerable<Store> stores);
        Task UpdateByStoreIdAsync(Store store);
        Task UpdateManyByStoreIdSplitByTransactionsAsync(IEnumerable<Store> stores);
        Task RemoveByStoreIdAsync(Guid storeId);
    }

}
