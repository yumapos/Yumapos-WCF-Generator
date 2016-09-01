using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IMenuItemToTaxRepository : IRepository<MenuItemToTax>
    {
        GetTaxeIdsByMenuItemId
    }
}
