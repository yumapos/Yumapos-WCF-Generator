using System;
using System.Collections.Generic;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IMenuCategoryRepository
    {
        Guid Insert(MenuCategory menuCategory);

        Guid Update(MenuCategory menuCategory);

        Guid Remove(MenuCategory menuCategory);

        MenuCategory GetByMenuCategoryId(System.Guid menuCategoryId, bool? isDeleted);

        IEnumerable<MenuCategory> GetAll(bool? isDeleted);
    }
}
