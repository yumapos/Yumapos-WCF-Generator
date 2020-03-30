using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.VersionsRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    interface IMenuItemRepository : IRepository<MenuItem>
    {
        Guid Insert(MenuItem menuItem);

        IEnumerable<MenuItem> InsertMany(IEnumerable<MenuItem> menuItem);

        Task<IEnumerable<MenuItem>> InsertManyAsync(IEnumerable<MenuItem> menuItem);

        void UpdateByMenuItemId(MenuItem menuItemId);

        Task UpdateManyByMenuItemIdAsync(IEnumerable<MenuItem> menuItem);

        void RemoveByMenuItemId(MenuItem menuItemId);

        MenuItem GetByMenuItemId(Guid menuItemId, DateTimeOffset modified, bool? isDeleted = false);

        MenuItem GetByMenuItemVersionId(Guid menuItemVersionId, bool? isDeleted = false);

        IEnumerable<MenuItem> GetAll(bool? isDeleted);

        IEnumerable<MenuItem> GetByMenuCategoryId(Guid menuCategoryId, DateTimeOffset sliceDate, bool? isDeleted);
    }
}
