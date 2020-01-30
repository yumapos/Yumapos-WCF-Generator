using System;
using System.Collections.Generic;
using TestMappingGeneration.Enums;
using TestMappingGeneration.Infrastructure;

namespace TestMappingGenerationDtos
{
    [Map]
    public class StoreDto
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
        public int BusinessDayStartInSeconds { get; set; }
        public bool IsActive { get; set; }
        public string Description { get; set; }
        public Guid? ImageId { get; set; }
        public AddressDto AddressDto { get; set; }
        public Guid? PriceListId { get; set; }
        public Guid? LegalEntityId { get; set; }
        public decimal MinAverageReceipt { get; set; }
        public decimal MaxAverageReceipt { get; set; }
        public IEnumerable<DeliveryZone2StoreDto> DeliveryZones { get; set; }
        public StorePaymentType PaymentTypes { get; set; }
       public bool IsMain { get; set; }
        public bool TipEnabled { get; set; }
        public bool IsDeliveryRoundTheClock { get; set; }
        public bool MobilePromoCampaignsShow { get; set; }
        public bool MobileFutureMenuShowing { get; set; }
        public bool PartySizeEnabled { get; set; }
        public bool DeliveryMoneyChangeEnabled { get; set; }
        public bool SendZReportEnabled { get; set; }
    }
}
