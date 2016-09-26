using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YumaPos.Server.Infrastructure.DataObjects;
using YumaPos.Server.Infrastructure.Repositories;

namespace YumaPos.Server.Data.Sql.Settings
{
    public partial class CustomerSubscriptionRepository : RepositoryBase, ICustomerSubscriptionRepository
    {
        private const string WhereQueryByNotificationsType = @" AND CustomerId IS NULL AND CustomerNotificationsType = @customerNotificationsType";

        public async Task<CustomerSubscription> GetCustomerSubscription(string customerId, int customerNotificationsType)
        {
            if (customerId == null)
            {
                return (await
                    DataAccessService.GetAsync<CustomerSubscription>(SelectAllQuery + WhereQueryByNotificationsType, new { customerNotificationsType })).FirstOrDefault();
            }

            var where =
                "AND CustomerSubscriptions.[CustomerId] = @CustomerId AND CustomerSubscriptions.[CustomerNotificationsType] = @CustomerNotificationsType ";

            return (await DataAccessService
                .GetAsync<CustomerSubscription>(SelectAllQuery + where, new
                {
                    customerNotificationsType,
                    customerId
                }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<CustomerSubscription>> GetSuperSubscriptions()
        {
            const string sql = @"SELECT CustomerId, CustomerNotificationsType, Email, SMS, Push, IsCustomizable, ResendPeriod,
                                 TenantId FROM dbo.CustomerSubscriptions
                                 WHERE TenantId is NULL AND CustomerId is NULL";

            return await DataAccessService.GetAsync<CustomerSubscription>(sql, new { });
        }

        public async Task<CustomerSubscription> GetSuperSubscription(int customerNotificationsType)
        {
            const string sql = @"SELECT CustomerId, CustomerNotificationsType, Email, SMS, Push, IsCustomizable, ResendPeriod,
                                 TenantId FROM dbo.CustomerSubscriptions
                                 WHERE TenantId is NULL AND CustomerId is NULL AND CustomerNotificationsType = @customerNotificationsType";
            return (await DataAccessService.GetAsync<CustomerSubscription>(sql, new { customerNotificationsType })).FirstOrDefault();
        }


        public async Task UpdateCustomerSubscription(CustomerSubscription customerSubscription)
        {
            var whereQuery = customerSubscription.CustomerId == null
                ? WhereQueryByNotificationsType
                : WhereQueryByCustomerIdAndCustomerNotificationsType;
            await DataAccessService.PersistObjectAsync(customerSubscription, UpdateQueryBy + whereQuery);
        }

    }
}