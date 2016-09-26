using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    public interface ICustomerSubscriptionRepository : IRepository<CustomerSubscription>
    {
        Task InsertAsync(CustomerSubscription customerSubscription);
        Task<CustomerSubscription> GetCustomerSubscription(string customerId, int customerNotificationsType);
        Task UpdateCustomerSubscription(CustomerSubscription customerSubscription);
        Task<IEnumerable<CustomerSubscription>> GetSuperSubscriptions();
        Task<CustomerSubscription> GetSuperSubscription(int customerNotificationsType);
    }
}
