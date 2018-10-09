using System;
using System.Collections.Generic;
using TestRepositoryGeneration.DataObjects;
using TestRepositoryGeneration.DataObjects.VersionsRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    interface IMenuItems2TaxesRepository : IRepository<MenuItems2Taxes>
    {
        void Insert(MenuItems2Taxes menuItems2Taxes);

        void UpdateByMenuItemId(MenuItems2Taxes menuItems2Taxes);

        void RemoveByMenuItemId(MenuItems2Taxes menuItems2Taxes);

        IEnumerable<MenuItems2Taxes> GetByTaxId(int TaxId, bool? isDeleted);

        void UpdateByTaxId(MenuItems2Taxes menuItems2Taxes);

        void RemoveByTaxId(MenuItems2Taxes menuItems2Taxes);

        IEnumerable<MenuItems2Taxes> GetByMenuItemId(Guid menuItemId, bool? isDeleted);

        IEnumerable<MenuItems2Taxes> GetAll(bool? isDeleted);
    }
}
