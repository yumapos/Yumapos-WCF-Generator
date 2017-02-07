using System;
using System.Collections.Generic;
using TestRepositoryGeneration.DataObjects;
using TestRepositoryGeneration.DataObjects.VersionsRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    interface IRecipieItemRepository
    {
        Guid Insert(RecipieItem item);

        IEnumerable<RecipieItem> GetByItemId(Guid itemId);

        Guid Update(RecipieItem item);

        Guid Remove(RecipieItem item);

        IEnumerable<RecipieItem> GetAll(bool? isDeleted);
    }
}
