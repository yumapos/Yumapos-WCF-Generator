using System;
using System.ComponentModel.DataAnnotations;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.DataInterfaces;

namespace YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat
{
    [DataRepository(TableName = "dbo.GiftCards")]
    public class GiftCard : IModified
    {
        [Key]
        public string GiftCardId { get; set; }
        public decimal Balance { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? ExpDate { get; set; }
        public int? Active { get; set; }
        public DateTime? Modified { get; set; }
    }
}