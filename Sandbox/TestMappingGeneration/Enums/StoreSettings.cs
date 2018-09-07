using System;
using System.Collections.Generic;
using System.Text;

namespace TestMappingGeneration.Enums
{
    [Flags]
    public enum StoreSettings
    {
        TipEnabled = 1,
        IsDeliveryRoundTheClock = 2,
        MobilePromoCampaignsShow = 4,
        MobileFutureMenuShowing = 8,
        PartySizeEnabled = 16,
        DeliveryMoneyChangeEnabled = 32,
        SendZReportEnabled = 64
    }
}
