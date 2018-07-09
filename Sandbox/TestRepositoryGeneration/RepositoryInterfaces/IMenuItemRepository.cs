using System;
using System.Collections.Generic;
using TestRepositoryGeneration.DataObjects;
using TestRepositoryGeneration.DataObjects.VersionsRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    interface IMenuItemRepository : IRepository<MenuItem>
    {
        Guid Insert(MenuItem menuItem);

        IEnumerable<MenuItem> InsertMany(IEnumerable<MenuItem> menuItem);
        void UpdateByMenuItemId(MenuItem itemId);

        void RemoveByMenuItemId(MenuItem itemId);

        MenuItem GetByMenuItemId(Guid itemId, DateTimeOffset sliceDate, bool? isDeleted);

        MenuItem GetByMenuItemVersionId(Guid itemVersionId, bool? isDeleted);

        IEnumerable<MenuItem> GetAll(bool? isDeleted);

        IEnumerable<MenuItem> GetByMenuCategoryId(Guid menuCategoryId, DateTimeOffset sliceDate, bool? isDeleted);
    }
}
