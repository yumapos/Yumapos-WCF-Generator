using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IMenuItemRepository : IRepository<MenuItem>
    {
        Guid Insert(MenuItem menuItem);

        Guid Update(MenuItem menuItem);

        Guid Remove(MenuItem menuItem);

        MenuItem GetByMenuItemId(System.Guid menuItemId, bool? isDeleted);

        IEnumerable<MenuItem> GetAll(bool? isDeleted);
    }
}
