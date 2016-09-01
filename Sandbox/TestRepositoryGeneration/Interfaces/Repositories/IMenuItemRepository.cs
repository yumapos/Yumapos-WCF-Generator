using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IMenuItemRepository : IRepository<MenuItem>
    {
        void Insert(MenuItem menuItem);

        void UpdateByItemId(MenuItem menuItem);

        void RemoveByItemId(MenuItem menuItem);

        IEnumerable<MenuItem> GetByMenuItemId(Guid menuItemId, bool? isDeleted);

        IEnumerable<MenuItem> GetAll(bool? isDeleted);
    }
}
