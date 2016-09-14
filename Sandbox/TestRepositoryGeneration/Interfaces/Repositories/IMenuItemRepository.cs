using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IMenuItemRepository : IRepository<MenuItem>
    {
        Guid Insert(MenuItem menuItem);

        void UpdateByItemId(MenuItem itemId);

        void RemoveByItemId(MenuItem itemId);

        MenuItem GetByItemId(Guid itemId, bool? isDeleted);

        MenuItem GetByItemVersionId(Guid itemVersionId, bool? isDeleted);

        IEnumerable<MenuItem> GetAll(bool? isDeleted);

        IEnumerable<MenuItem> GetByMenuCategoryId(Guid menuCategoryId, bool? isDeleted);
    }
}
