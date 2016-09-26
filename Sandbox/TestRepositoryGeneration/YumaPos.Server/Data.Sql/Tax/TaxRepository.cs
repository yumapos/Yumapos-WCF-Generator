//using System;
//using System.Collections.Generic;
//using System.Linq;
//using YumaPos.FrontEnd.Infrastructure.Configuration;
//using YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes;
//using YumaPos.Server.Data.Sql.Menu;
//using YumaPos.Server.Infrastructure.DataObjects;
//using YumaPos.Server.Infrastructure.Repositories;

//namespace YumaPos.Server.Data.Sql.Taxes
//{
//    class TaxRepository : RepositoryBase, ITaxRepository
//    {
//        private MenuItemCashRepository _menuItemRepository;

//        TaxCashRepository _taxCashRepository;
//        TaxVersionRepository _taxVersionRepository;

//        MenuItemsToTaxesСacheRepository _menuItemsToTaxesСacheRepository;
//        MenuItemsToTaxesVersionRepository _menuItemsToTaxesVersionRepository;

//        public TaxRepository(IDataAccessService dataAccessService) : base(dataAccessService)
//        {
//            _taxCashRepository = new TaxCashRepository(dataAccessService);
//            _taxVersionRepository = new TaxVersionRepository(dataAccessService);
//            _menuItemRepository = new MenuItemCashRepository(dataAccessService);

//            _menuItemsToTaxesСacheRepository = new MenuItemsToTaxesСacheRepository(dataAccessService);
//            _menuItemsToTaxesVersionRepository = new MenuItemsToTaxesVersionRepository(dataAccessService);
//        }

//        public void UpdateMenuItemsToTaxes(Tax tax, IEnumerable<Guid> menuItemIds)
//        {
//            if (menuItemIds == null)
//                menuItemIds = _menuItemsToTaxesСacheRepository.GetMenuItemIdsByTaxId(tax.TaxId);

//            var listOfMenuItemsToTaxes = menuItemIds.Select(menuItemId => new MenuItemToTax()
//            {
//                TaxId = tax.TaxId,
//                TaxVersionId = tax.TaxVersionId,
//                ItemId = menuItemId,
//                ItemVersionId = _menuItemRepository.GetByMenuItemId(menuItemId).ItemVersionId,
//            }).ToList();

//            _menuItemsToTaxesСacheRepository.RemoveByTaxId(tax.TaxId);

//            foreach (var mt in listOfMenuItemsToTaxes)
//            {
//                mt.Modified = DateTimeOffset.Now;
//                mt.ModifiedBy = tax.ModifiedBy;

//                _menuItemsToTaxesVersionRepository.Insert(mt);
//                _menuItemsToTaxesСacheRepository.Insert(mt);
//            }
//        }

//        public Guid Insert(Tax tax)
//        {
//            tax.Modified = DateTimeOffset.Now;

//            tax.TaxVersionId = _taxVersionRepository.Insert(tax);
//            _taxCashRepository.Insert(tax);

//            return tax.TaxVersionId;
//        }

//        public Guid Update(Tax tax)
//        {
//            tax.Modified = DateTimeOffset.Now;

//            tax.TaxVersionId = _taxVersionRepository.Insert(tax);
//            _taxCashRepository.Update(tax);

//            UpdateMenuItemsToTaxes(tax, null);

//            return tax.TaxVersionId;
//        }

//        public Guid Remove(Tax tax)
//        {
//            tax.IsDeleted = true;

//            return Update(tax);
//        }

//        public Tax GetByTaxId(System.Guid taxId, bool? isDeleted = false)
//        {
//            return _taxCashRepository.GetByTaxId(taxId);
//        }
//    }
//}
