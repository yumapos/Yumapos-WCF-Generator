using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface ICustomerSubscriptionArchiveRepository : IArchiveRepository<CustomerSubscription>
    {
        IEnumerable<CustomerSubscription> GetByCustomerIdAndCustomerNotificationsType(string customerId, int customerNotificationsType);
        Task InsertOrUpdateAsync(CustomerSubscription item);
    }
}