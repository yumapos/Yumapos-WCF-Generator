using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IMenuItems2TaxesRepository : IRepository<MenuItems2Taxes>
    {
        void Insert(MenuItems2Taxes menuItem);

        void UpdateByMenuItemId(MenuItems2Taxes itemId);

        void RemoveByMenuItemId(MenuItems2Taxes itemId);

        IEnumerable<MenuItems2Taxes> GetByTaxId(int TaxId, bool? isDeleted);

        void UpdateByTaxId(MenuItems2Taxes itemId);

        void RemoveByTaxId(MenuItems2Taxes itemId);

        IEnumerable<MenuItems2Taxes> GetByMenuItemId(Guid itemId, bool? isDeleted);

        IEnumerable<MenuItems2Taxes> GetAll(bool? isDeleted);
    }
}
