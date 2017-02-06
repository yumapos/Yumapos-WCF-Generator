using System;
using System.Collections.Generic;
using TestRepositoryGeneration.DataObjects;
using TestRepositoryGeneration.DataObjects.VersionsRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    interface IMenuItems2TaxesRepository : IRepository<MenuItems2Taxes>
    {
        void Insert(MenuItems2Taxes menuItem);

        void UpdateByMenuItemId(MenuItems2Taxes itemId);

        void RemoveByMenuItemId(MenuItems2Taxes itemId);

        IEnumerable<MenuItems2Taxes> GetByTaxId(int TaxId, bool? isDeleted);

        void UpdateByTaxId(MenuItems2Taxes itemId);

        void RemoveByTaxId(MenuItems2Taxes itemId);

        IEnumerable<MenuItems2Taxes> GetByMenuItemId(Guid itemId, bool? isDeleted);

        IEnumerable<MenuItems2Taxes> GetAll(bool? isDeleted);
    }
}
