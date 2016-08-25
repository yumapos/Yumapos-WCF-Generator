using System;
using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;
using VersionedRepositoryGeneration.Interfaces.Repositories;
using VersionedRepositoryGeneration.Models;
using VersionedRepositoryGeneration.Repositories.Tax;

namespace VersionedRepositoryGeneration.Repositories
{
    partial class MenuItemRepository : RepositoryBase, IMenuItemRepository
    {
        private TaxCashRepository _taxRepository;

        MenuItemCashRepository _menuItemCashRepository;
        MenuItemVersionRepository _menuItemVersionRepository;

        MenuItemsToTaxesCashRepository _menuItemsToTaxesCashRepository;
        MenuItemsToTaxesVersionRepository _menuItemsToTaxesVersionRepository;

        public MenuItemRepository(IDataAccessService dataAccessService) : base(dataAccessService)
        {
            _taxRepository = new TaxCashRepository(dataAccessService);
            _menuItemCashRepository = new MenuItemCashRepository(dataAccessService);
            _menuItemVersionRepository = new MenuItemVersionRepository(dataAccessService);

            _menuItemsToTaxesCashRepository = new MenuItemsToTaxesCashRepository(dataAccessService);
            _menuItemsToTaxesVersionRepository = new MenuItemsToTaxesVersionRepository(dataAccessService);
        }

        private void UpdateMenuItemsToTaxes(MenuItem menuItem, IEnumerable<Guid> taxIds)
        {
            if (taxIds == null)
                taxIds = _menuItemsToTaxesCashRepository.GetTaxeIdsByMenuItemId(menuItem.ItemId);

            var listOfMenuItemsToTaxes = taxIds.Select(taxId => new MenuItemToTax()
            {
                TaxId = taxId,
                TaxVersionId = _taxRepository.GetByTaxId(taxId).TaxVersionId,
                ItemId = menuItem.ItemId,
                ItemVersionId = menuItem.ItemVersionId,
            }).ToList();

            _menuItemsToTaxesCashRepository.RemoveByMenuItemId(menuItem.ItemId);

            foreach (var mt in listOfMenuItemsToTaxes)
            {
                mt.Modified = DateTimeOffset.Now;
                mt.ModifiedBy = menuItem.ModifiedBy;

                _menuItemsToTaxesVersionRepository.Insert(mt);
                _menuItemsToTaxesCashRepository.Insert(mt);
            }
        }

        public Guid Insert(Models.MenuItem menuItem)
        {
            menuItem.Modified = DateTimeOffset.Now;

            menuItem.ItemVersionId = _menuItemVersionRepository.Insert(menuItem);
            _menuItemCashRepository.Insert(menuItem);

            UpdateMenuItemsToTaxes(menuItem, menuItem.TaxesId);

            return menuItem.ItemVersionId;
        }

        public Guid Update(Models.MenuItem menuItem)
        {
            menuItem.Modified = DateTimeOffset.Now;

            menuItem.ItemVersionId = _menuItemVersionRepository.Insert(menuItem);
            _menuItemCashRepository.Update(menuItem);

            UpdateMenuItemsToTaxes(menuItem, menuItem.TaxesId);

            return menuItem.ItemVersionId;
        }

        public Guid Remove(Models.MenuItem menuItem)
        {
            menuItem.IsDeleted = true;
            
            return Update(menuItem);
        }

        public Models.MenuItem GetByMenuItemId(System.Guid menuItemId, bool? isDeleted = false)
        {
            return _menuItemCashRepository.GetByMenuItemId(menuItemId);
        }

        public IEnumerable<Models.MenuItem> GetAll(bool? isDeleted = false)
        {
            return _menuItemCashRepository.GetAll();
        }

    }
}
