using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface ICustomerSubscriptionArchiveRepository : IArchiveRepository<CustomerSubscription>
    {
        CustomerSubscription GetByCustomerIdAndCustomerNotificationsType(string customerId, int customerNotificationsType);//  customerId + customerNotificationsType is PK
        Task<IEnumerable<CustomerSubscription>> GetByCustomerIdAsync(string customerId);
        Task InsertOrUpdateAsync(CustomerSubscription item);
    }
}