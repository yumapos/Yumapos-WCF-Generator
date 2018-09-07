using System;
using System.Collections.Generic;
using System.Text;

namespace TestMappingGeneration.Enums
{
    [Flags]
    public enum StorePaymentType
    {
        Cash = 1,
        Card = 2,
        GiftCard = 4,
        Online = 8
    }
}
