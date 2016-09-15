using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes;
using YumaPos.FrontEnd.Infrastructure.Repositories;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface ITaxRepository : IRepository<Tax>
    {
        int Insert(Tax tax);

        void UpdateByTaxId(Tax tax);

        void RemoveByTaxId(Tax tax);

        Tax GetByTaxId(int taxId, bool? isDeleted = false);
    }
}
