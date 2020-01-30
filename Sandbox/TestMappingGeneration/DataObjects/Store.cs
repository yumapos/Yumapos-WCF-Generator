using System;
using System.Collections.Generic;
using System.Text;
using TestMappingGeneration.Enums;
using TestMappingGeneration.Infrastructure;

namespace TestMappingGeneration.DataObjects
{
    [Map]
    public class Store
    {
        public Guid StoreId { get; set; }
        public string Name { get; set; }
        public string ContactPerson { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> EmailsForSendZReport { get; set; }
        public string Phone { get; set; }
        public string Url { get; set; }
        public string Ip { get; set; }
        public string Logo { get; set; }
        public bool IsActive { get; set; }
        public int BusinessDayStartInSeconds { get; set; }
        public string Description { get; set; }
        public Guid? ImageId { get; set; }
        public Guid? AddressId { get; set; }
        public Guid? LegalEntityId { get; set; }
        public decimal MinAverageReceipt { get; set; }
        public decimal MaxAverageReceipt { get; set; }
        [MapIgnore]
        public DateTime? MenuUpdated { get; set; }
        [Map(Name = "AddressDto")]
        public Address StoreAddress { get; set; }
        public Guid PriceListId { get; set; }
        [MapIgnore]
        public bool IsDeleted { get; set; }
        public int ServiceTypes { get; set; }
        public int PaymentTypes { get; set; }
        public Guid? CustomTenderId { get; set; }
        public Guid? AggregatedTenderId { get; set; }
        public StoreSettings BitSettings { get; set; }
    }
}
