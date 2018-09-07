using System;
using System.Collections.Generic;
using System.Text;
using TestMappingGeneration.Enums;
using TestMappingGeneration.Infrastructure;

namespace TestMappingGeneration.DataTransferObjects
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
        [Map(Name = "BitSettings",
            FunctionFrom = "item.BitSettings = (itemDto.IsSendZReportEnabled ? StoreSettings.SendZReportEnabled : 0) | (itemDto.DeliveryMoneyChangeEnabled ? StoreSettings.DeliveryMoneyChangeEnabled : 0) | (itemDto.PartySizeEnabled ? StoreSettings.PartySizeEnabled : 0) | (itemDto.MobileFutureMenuShowing ? StoreSettings.MobileFutureMenuShowing : 0) | (itemDto.MobilePromoCampaignsShow ? StoreSettings.MobilePromoCampaignsShow : 0) | (itemDto.IsDeliveryRoundTheClock ? StoreSettings.IsDeliveryRoundTheClock : 0) | (itemDto.TipEnabled ? StoreSettings.TipEnabled : 0)",
            FunctionTo = "itemDto.IsSendZReportEnabled = item.BitSettings.HasFlag(StoreSettings.SendZReportEnabled); itemDto.DeliveryMoneyChangeEnabled = item.BitSettings.HasFlag(StoreSettings.DeliveryMoneyChangeEnabled); itemDto.PartySizeEnabled = item.BitSettings.HasFlag(StoreSettings.PartySizeEnabled); itemDto.MobileFutureMenuShowing = item.BitSettings.HasFlag(StoreSettings.MobileFutureMenuShowing); itemDto.MobilePromoCampaignsShow = item.BitSettings.HasFlag(StoreSettings.MobilePromoCampaignsShow); itemDto.IsDeliveryRoundTheClock = item.BitSettings.HasFlag(StoreSettings.IsDeliveryRoundTheClock); itemDto.TipEnabled = item.BitSettings.HasFlag(StoreSettings.TipEnabled); itemDto.BitSettings = (StoreSettings)item.BitSettings;")]
        public StoreSettings BitSettings { get; set; }
        public bool IsMain { get; set; }
        public bool TipEnabled { get; set; }
        public bool IsDeliveryRoundTheClock { get; set; }
        public bool MobilePromoCampaignsShow { get; set; }
        public bool MobileFutureMenuShowing { get; set; }
        public bool PartySizeEnabled { get; set; }
        public bool DeliveryMoneyChangeEnabled { get; set; }
        public bool IsSendZReportEnabled { get; set; }
    }
}
