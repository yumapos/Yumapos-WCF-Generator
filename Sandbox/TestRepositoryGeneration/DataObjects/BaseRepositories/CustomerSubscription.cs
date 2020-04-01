using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.CustomRepositories.BaseRepositories;
using TestRepositoryGeneration.Infrastructure;
using TestRepositoryGeneration.RepositoryInterfaces;

namespace TestRepositoryGeneration.DataObjects.BaseRepositories
{
    /// <summary>
    ///     Repository interface <see cref="ICustomerSubscriptionRepository"/>
    ///     Repository <see cref="CustomerSubscriptionRepository"/>
    /// </summary>
    [DataAccess(TableName = "CustomerSubscriptions")]
    [DataArchive(TableName = "archive.customer_subscriptions", FilterKey1 = "CustomerId")]
    public class CustomerSubscription
    {
        [Key]
        public string CustomerId { get; set; }
        [Key]
        public int? CustomerNotificationsType { get; set; }
        public bool Email { get; set; }
        public bool SMS { get; set; }
        public bool Push { get; set; }
        public bool? IsCustomizable { get; set; }
        public int? ResendPeriod { get; set; }
        public bool IsDeleted { get; set; }
    }
}