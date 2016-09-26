using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using YumaPos.FrontEnd.Infrastructure.Configuration;
using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat;
using YumaPos.FrontEnd.Infrastructure.Repositories;

namespace YumaPos.FrontEnd.Data.Repositories.TransactionsHistory
{
    public partial class GiftCardRepository : RepositoryBase, IGiftCardRepository
    {
        public GiftCardRepository(IDataAccessService dataAccessService)
            : base(dataAccessService)
        {
        }

        public Task<IList<GiftCard>> Get(int from, int pageSize, string filter)
        {
            throw new NotImplementedException();
        }
    }
}

