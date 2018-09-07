using System;
using System.Collections.Generic;
using System.Text;
using TestMappingGeneration.Infrastructure;

namespace TestMappingGeneration.DataTransferObjects
{
    [Map]
    public class DeliveryZone2StoreDto
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
