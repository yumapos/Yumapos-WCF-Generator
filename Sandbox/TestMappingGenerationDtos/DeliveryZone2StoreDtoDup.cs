using System;
using TestMappingGeneration.Infrastructure;

namespace TestMappingGenerationDtos
{
    [Map(Name = "DeliveryZone2Store")]
    public class DeliveryZone2StoreDtoDup
    {
        public Guid DeliveryZone2StoreId { get; set; }
        public Guid DeliveryZoneId { get; set; }
        public Guid StoreId { get; set; }
        public string Name { get; set; }
        public decimal? MinimumSumForFreeDelivery { get; set; }
        public decimal? DeliveryPrice { get; set; }
        public decimal? MinimumSumForDelivery { get; set; }
        public int? SecondsTimeCookingLimit { get; set; }
        public int? SecondsTimeDeliveryLimit { get; set; }
        public decimal? PriceFixed { get; set; }
        public int? PricePercentOfOrder { get; set; }
        public int? PricePercentOfCostDelivery { get; set; }
    }
}
