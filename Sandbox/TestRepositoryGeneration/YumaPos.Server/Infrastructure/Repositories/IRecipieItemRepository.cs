using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IRecipieItemRepository : IRepository<RecipieItem>
    {
        Guid Insert(RecipieItem item);

        IEnumerable<RecipieItem> GetByItemId(Guid itemId);

        Guid Update(RecipieItem item);

        Guid Remove(RecipieItem item);

        IEnumerable<RecipieItem> GetAll(bool? isDeleted);
    }
}
