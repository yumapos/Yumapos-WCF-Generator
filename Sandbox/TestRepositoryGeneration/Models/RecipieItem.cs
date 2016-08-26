using System;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;

namespace YumaPos.Server.Infrastructure.DataObjects
{
    [DataAccess]
    public class RecipieItem
    {
        public Guid ItemId { get; set; }

        public Guid ItemVersionId { get; set; }

        public bool IsDeleted { get; set; }

    }
}
