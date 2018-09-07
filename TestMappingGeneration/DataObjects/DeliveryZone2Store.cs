using System;
using System.Collections.Generic;
using System.Text;
using TestMappingGeneration.Infrastructure;

namespace TestMappingGeneration.DataObjects
{
    [Map]
    public class DeliveryZone2Store 
    {
        public Guid DeliveryZone2StoreId { get; set; }
        public Guid DeliveryZoneId { get; set; }
        public Guid StoreId { get; set; }
        public string Name { get; set; }
        public decimal? MinimumSumForFreeDelivery { get; set; }
        public decimal? DeliveryPrice { get; set; }
        public decimal? MinimumSumForDelivery { get; set; }
        public Guid DeliveryZone2StoreVersionId { get; set; }
        public DateTimeOffset Modified { get; set; }
        public Guid ModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
        public int? SecondsTimeCookingLimit { get; set; }
        public int? SecondsTimeDeliveryLimit { get; set; }
        public decimal? PriceFixed { get; set; }
        public int? PricePercentOfOrder { get; set; }
        public int? PricePercentOfCostDelivery { get; set; }
    }
}
