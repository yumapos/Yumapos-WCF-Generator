using System;
using System.Collections.Generic;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IRecipeItemsLanguageRepository : IRepository<RecipeItemsLanguage>
    {
        void Insert(RecipeItemsLanguage recipeItemsLanguage);
        RecipeItemsLanguage GetByItemIdAndLanguage(Guid id, string lang, bool? isDeleted = false);
        IEnumerable<RecipeItemsLanguage> GetByItemId(Guid id, bool? isDeleted = false);
        RecipeItemsLanguage GetByItemIdVersionId(System.Guid itemIdVersionId, bool? isDeleted = false);
        void RemoveByItemId(Guid itemId);
        void UpdateByItemId(RecipeItemsLanguage recipeItemsLanguage);
    }
}
