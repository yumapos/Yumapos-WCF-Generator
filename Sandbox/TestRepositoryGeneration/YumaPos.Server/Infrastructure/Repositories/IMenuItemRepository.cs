using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IMenuItemRepository : IRepository<MenuItem>
    {
        Guid Insert(MenuItem menuItem);

        void UpdateByMenuItemId(MenuItem itemId);

        void RemoveByMenuItemId(MenuItem itemId);

        MenuItem GetByMenuItemId(Guid itemId, DateTimeOffset sliceDate, bool? isDeleted);

        MenuItem GetByMenuItemVersionId(Guid itemVersionId, bool? isDeleted);

        IEnumerable<MenuItem> GetAll(bool? isDeleted);

        IEnumerable<MenuItem> GetByMenuCategoryId(Guid menuCategoryId, DateTimeOffset sliceDate, bool? isDeleted);
    }
}
