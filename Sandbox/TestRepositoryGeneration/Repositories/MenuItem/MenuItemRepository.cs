//using System;
//using System.Collections.Generic;
//using System.Linq;
//using YumaPos.FrontEnd.Infrastructure.Configuration;
//using YumaPos.Server.Data.Sql.Taxes;
//using YumaPos.Server.Infrastructure.DataObjects;
//using YumaPos.Server.Infrastructure.Repositories;

//namespace YumaPos.Server.Data.Sql.Menu
//{
//    partial class MenuItemRepository : RepositoryBase, IMenuItemRepository
//    {
//        private TaxCashRepository _taxRepository;

//        MenuItemCashRepository _menuItemCashRepository;
//        MenuItemVersionRepository _menuItemVersionRepository;

//        MenuItemsToTaxesСacheRepository _menuItemsToTaxesCashRepository;
//        MenuItemsToTaxesVersionRepository _menuItemsToTaxesVersionRepository;

//        public MenuItemRepository(IDataAccessService dataAccessService) : base(dataAccessService)
//        {
//            _taxRepository = new TaxCashRepository(dataAccessService);
//            _menuItemCashRepository = new MenuItemCashRepository(dataAccessService);
//            _menuItemVersionRepository = new MenuItemVersionRepository(dataAccessService);

//            _menuItemsToTaxesCashRepository = new MenuItemsToTaxesСacheRepository(dataAccessService);
//            _menuItemsToTaxesVersionRepository = new MenuItemsToTaxesVersionRepository(dataAccessService);
//        }

//        private void UpdateMenuItemsToTaxes(MenuItem menuItem, IEnumerable<Guid> taxIds)
//        {
//            if (taxIds == null)
//                taxIds = _menuItemsToTaxesCashRepository.GetTaxeIdsByMenuItemId(menuItem.ItemId);

//            var listOfMenuItemsToTaxes = taxIds.Select(taxId => new MenuItemToTax()
//            {
//                TaxId = taxId,
//                TaxVersionId = _taxRepository.GetByTaxId(taxId).TaxVersionId,
//                ItemId = menuItem.ItemId,
//                ItemVersionId = menuItem.ItemVersionId,
//            }).ToList();

//            _menuItemsToTaxesCashRepository.RemoveByMenuItemId(menuItem.ItemId);

//            foreach (var mt in listOfMenuItemsToTaxes)
//            {
//                mt.Modified = DateTimeOffset.Now;
//                mt.ModifiedBy = menuItem.ModifiedBy;

//                _menuItemsToTaxesVersionRepository.Insert(mt);
//                _menuItemsToTaxesCashRepository.Insert(mt);
//            }
//        }

//        public Guid Insert(MenuItem menuItem)
//        {
//            menuItem.Modified = DateTimeOffset.Now;

//            menuItem.ItemVersionId = _menuItemVersionRepository.Insert(menuItem);
//            _menuItemCashRepository.Insert(menuItem);

//            UpdateMenuItemsToTaxes(menuItem, menuItem.TaxesId);

//            return menuItem.ItemVersionId;
//        }

//        public Guid Update(MenuItem menuItem)
//        {
//            menuItem.Modified = DateTimeOffset.Now;

//            menuItem.ItemVersionId = _menuItemVersionRepository.Insert(menuItem);
//            _menuItemCashRepository.Update(menuItem);

//            UpdateMenuItemsToTaxes(menuItem, menuItem.TaxesId);

//            return menuItem.ItemVersionId;
//        }

//        public Guid Remove(MenuItem menuItem)
//        {
//            menuItem.IsDeleted = true;
            
//            return Update(menuItem);
//        }

//        public MenuItem GetByMenuItemId(System.Guid menuItemId, bool? isDeleted = false)
//        {
//            return _menuItemCashRepository.GetByMenuItemId(menuItemId);
//        }

//        public IEnumerable<MenuItem> GetAll(bool? isDeleted = false)
//        {
//            return _menuItemCashRepository.GetAll();
//        }

//    }
//}
