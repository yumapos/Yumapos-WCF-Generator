using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface IElectronicCouponsTypedRepository : IRepository<ElectronicCouponsTyped>
    {
        IEnumerable<ElectronicCouponsTyped> GetAll(bool? isDeleted = false);

        Task<IEnumerable<ElectronicCouponsTyped>> GetAllAsync(bool? isDeleted = false);

        Task<ElectronicCouponsTyped> Get(int id);

        Task<int> InsertAsync(ElectronicCouponsTyped electronicCouponsTyped);

        Task InsertManyAsync(IEnumerable<ElectronicCouponsTyped> electronicCouponsTypedList);
        
        Task UpdateByElectronicCouponsIdAsync(ElectronicCouponsTyped electronicCouponsTyped);

        Task RemoveByElectronicCouponsIdAsync(int id);

        Task RemoveList(IEnumerable<int> ids);
    }
}
