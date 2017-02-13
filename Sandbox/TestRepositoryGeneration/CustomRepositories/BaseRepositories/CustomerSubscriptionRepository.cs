using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;
using TestRepositoryGeneration.Infrastructure;
using TestRepositoryGeneration.RepositoryInterfaces;

namespace TestRepositoryGeneration.CustomRepositories.BaseRepositories
{
    public partial class CustomerSubscriptionRepository : RepositoryBase, ICustomerSubscriptionRepository
    {
        #region Implementation of ICustomerSubscriptionRepository

        public Task<CustomerSubscription> GetCustomerSubscription(string customerId, int customerNotificationsType)
        {
            throw new System.NotImplementedException();
        }

        public Task UpdateCustomerSubscription(CustomerSubscription customerSubscription)
        {
            throw new System.NotImplementedException();
        }

        public Task<IEnumerable<CustomerSubscription>> GetSuperSubscriptions()
        {
            throw new System.NotImplementedException();
        }

        public Task<CustomerSubscription> GetSuperSubscription(int customerNotificationsType)
        {
            throw new System.NotImplementedException();
        }

        #endregion
    }
}