using System;
using YumaPos.FrontEnd.Infrastructure.DataInterfaces;

namespace YumaPos.FrontEnd.Infrastructure.DataObjects
{
    public class Tenant : ITenantUnrelated
    {
        public Guid TenantId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ContactPerson { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string Url { get; set; }
        public string Email { get; set; }
        public int TimeZone { get; set; }
        public string Ip { get; set; }
        public bool IsActive { get; set; }
        public int? OpeningTime { get; set; }
        public int? Points { get; set; }
        public DateTime Registered { get; set; }
        public int SubscriptionPlanId { get; set; }
        public string Alias { get; set; }
        public Guid? AddressId { get; set; }
        public int Currency { get; set; }
        public Guid? ImageId { get; set; }
        public int BusinessDayStart { get; set; }
    }
}
