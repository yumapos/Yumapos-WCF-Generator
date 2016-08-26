using System;
using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface ITaxRepository
    {
        Guid Insert(Tax tax);

        Guid Update(Tax tax);

        Guid Remove(Tax tax);

        Tax GetByTaxId(System.Guid taxId, bool? isDeleted = false);
    }
}
