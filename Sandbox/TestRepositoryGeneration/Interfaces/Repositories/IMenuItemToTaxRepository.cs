using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IMenuItemToTaxRepository : IRepository<MenuItemToTax>
    {
        void Insert(MenuItemToTax menuItem);

        void UpdateByItemId(MenuItemToTax itemId);

        void RemoveByItemId(MenuItemToTax itemId);

        IEnumerable<MenuItemToTax> GetByTaxId(Guid TaxId, bool? isDeleted);

        void UpdateByTaxId(MenuItemToTax itemId);

        void RemoveByTaxId(MenuItemToTax itemId);

        IEnumerable<MenuItemToTax> GetByItemId(Guid itemId, bool? isDeleted);

        IEnumerable<MenuItemToTax> GetAll(bool? isDeleted);
    }
}
