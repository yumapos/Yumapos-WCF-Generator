using System;
using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;
using VersionedRepositoryGeneration.Interfaces.Repositories;
using VersionedRepositoryGeneration.Models;

namespace VersionedRepositoryGeneration.Repositories.Tax
{
    class TaxRepository : RepositoryBase, ITaxRepository
    {
        private MenuItemCashRepository _menuItemRepository;

        TaxCashRepository _taxCashRepository;
        TaxVersionRepository _taxVersionRepository;

        MenuItemsToTaxesCashRepository _menuItemsToTaxesCashRepository;
        MenuItemsToTaxesVersionRepository _menuItemsToTaxesVersionRepository;

        public TaxRepository(IDataAccessService dataAccessService) : base(dataAccessService)
        {
            _taxCashRepository = new TaxCashRepository(dataAccessService);
            _taxVersionRepository = new TaxVersionRepository(dataAccessService);
            _menuItemRepository = new MenuItemCashRepository(dataAccessService);

            _menuItemsToTaxesCashRepository = new MenuItemsToTaxesCashRepository(dataAccessService);
            _menuItemsToTaxesVersionRepository = new MenuItemsToTaxesVersionRepository(dataAccessService);
        }

        public void UpdateMenuItemsToTaxes(Models.Tax tax, IEnumerable<Guid> menuItemIds)
        {
            if (menuItemIds == null)
                menuItemIds = _menuItemsToTaxesCashRepository.GetMenuItemIdsByTaxId(tax.TaxId);

            var listOfMenuItemsToTaxes = menuItemIds.Select(menuItemId => new MenuItemToTax()
            {
                TaxId = tax.TaxId,
                TaxVersionId = tax.TaxVersionId,
                ItemId = menuItemId,
                ItemVersionId = _menuItemRepository.GetByMenuItemId(menuItemId).ItemVersionId,
            }).ToList();

            _menuItemsToTaxesCashRepository.RemoveByTaxId(tax.TaxId);

            foreach (var mt in listOfMenuItemsToTaxes)
            {
                mt.Modified = DateTimeOffset.Now;
                mt.ModifiedBy = tax.ModifiedBy;

                _menuItemsToTaxesVersionRepository.Insert(mt);
                _menuItemsToTaxesCashRepository.Insert(mt);
            }
        }

        public Guid Insert(Models.Tax tax)
        {
            tax.Modified = DateTimeOffset.Now;

            tax.TaxVersionId = _taxVersionRepository.Insert(tax);
            _taxCashRepository.Insert(tax);

            return tax.TaxVersionId;
        }

        public Guid Update(Models.Tax tax)
        {
            tax.Modified = DateTimeOffset.Now;

            tax.TaxVersionId = _taxVersionRepository.Insert(tax);
            _taxCashRepository.Update(tax);

            UpdateMenuItemsToTaxes(tax, null);

            return tax.TaxVersionId;
        }

        public Guid Remove(Models.Tax tax)
        {
            tax.IsDeleted = true;

            return Update(tax);
        }

        public Models.Tax GetByTaxId(System.Guid taxId, bool? isDeleted = false)
        {
            return _taxCashRepository.GetByTaxId(taxId);
        }
    }
}
