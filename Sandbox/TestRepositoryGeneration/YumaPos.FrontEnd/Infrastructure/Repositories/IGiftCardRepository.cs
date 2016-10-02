using System.Collections.Generic;
using System.Threading.Tasks;

using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat;

namespace YumaPos.FrontEnd.Infrastructure.Repositories
{
    public interface IGiftCardRepository : IRepository<GiftCard>
    {
        Task<IList<GiftCard>> Get(int from, int pageSize, string filter);

        GiftCard GetByGiftCardId(string cardId);

        void UpdateByGiftCardId(GiftCard giftCard);
    }
}