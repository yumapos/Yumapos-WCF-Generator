using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface ICustomerSubscriptionRepository : IRepository<CustomerSubscription>
    {
        Task<CustomerSubscription> GetByCustomerIdAndCustomerNotificationsTypeAsync(string customerId, int customerNotificationsType);
    }
}
