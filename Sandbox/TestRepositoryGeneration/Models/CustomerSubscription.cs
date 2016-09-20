using System.ComponentModel.DataAnnotations;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;


namespace YumaPos.Server.Infrastructure.DataObjects
{
    [DataAccess(TableName = "CustomerSubscriptions")]
    public class CustomerSubscription
    {
        public string CustomerId { get; set; }
        public int CustomerNotificationsType { get; set; }
        public bool Email { get; set; }
        public bool SMS { get; set; }
        public bool Push { get; set; }
        public bool? IsCustomizable { get; set; }
        public int? ResendPeriod { get; set; }
    }
}