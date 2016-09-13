//using System;
//using System.Collections.Generic;
//using System.Linq;
//using TestRepositoryGeneration;
//using YumaPos.FrontEnd.Infrastructure.Configuration;
//using YumaPos.Server.Data.Sql.Taxes;
//using YumaPos.Server.Infrastructure.DataObjects;
//using YumaPos.Server.Infrastructure.Repositories;

//namespace YumaPos.Server.Data.Sql.Menu
//{
//    partial class MenuItemRepository : RepositoryBase, IMenuItemRepository
//    {
//        private TaxCashRepository _taxRepository;

//        MenuItemСacheRepository _menuItemCashRepository;
//        MenuItemVersionRepository _menuItemVersionRepository;

//        MenuItemsToTaxesСacheRepository _menuItemsToTaxesCashRepository;
//        MenuItemsToTaxesVersionRepository _menuItemsToTaxesVersionRepository;

//        public MenuItemRepository(IDataAccessService dataAccessService) : base(dataAccessService)
//        {
//            _taxRepository = new TaxCashRepository(dataAccessService);
//            _menuItemCashRepository = new MenuItemСacheRepository(dataAccessService);
//            _menuItemVersionRepository = new MenuItemVersionRepository(dataAccessService);

//            _menuItemsToTaxesCashRepository = new MenuItemsToTaxesСacheRepository(dataAccessService);
//            _menuItemsToTaxesVersionRepository = new MenuItemsToTaxesVersionRepository(dataAccessService);
//        }

//        private void UpdateMenuItemsToTaxes(MenuItem menuItem)
//        {
//            if (menuItem.TaxesId == null)
//                menuItem.TaxesId = _menuItemsToTaxesCashRepository.GetTaxeIdsByMenuItemId(menuItem.ItemId);

//            var listOfMenuItemsToTaxes = menuItem.TaxesId.Select(taxId => new MenuItemToTax()
//            {
//                TaxId = taxId,
//                TaxVersionId = _taxRepository.GetByTaxId(taxId).TaxVersionId,
//                ItemId = menuItem.ItemId,
//                ItemVersionId = menuItem.ItemVersionId,
//            }).ToList();

//            _menuItemsToTaxesCashRepository.RemoveByMenuItemId(   .ItemId);

//            foreach (var mt in listOfMenuItemsToTaxes)
//            {
//                mt.Modified = DateTimeOffset.Now;
//                mt.ModifiedBy = menuItem.ModifiedBy;

//                _menuItemsToTaxesVersionRepository.Insert(mt);
//                _menuItemsToTaxesCashRepository.Insert(mt);
//            }
//        }

//        public void Insert(MenuItem menuItem)
//        {
//            menuItem.Modified = DateTimeOffset.Now;

//            menuItem.ItemVersionId = _menuItemVersionRepository.Insert(menuItem);
//            _menuItemCashRepository.Insert(menuItem);

//            UpdateMenuItemsToTaxes(menuItem, menuItem.TaxesId);
//        }

//        public void Update(MenuItem menuItem)
//        {
//            menuItem.Modified = DateTimeOffset.Now;

//            menuItem.ItemVersionId = _menuItemVersionRepository.Insert(menuItem);
//            _menuItemCashRepository.UpdateByModifiedBy(menuItem);
//        }

//        public void RemoveById(MenuItem menuItem)
//        {
//            menuItem.IsDeleted = true;

//            Update(menuItem);
//        }

//        public void RemoveByModifiedBy(Guid modifiedBy)
//        {
//            var result = _menuItemCashRepository.GetByModifiedBy(modifiedBy);
//            foreach (var item in result)
//            {
//                item.IsDeleted = true;
//                Update(item);
//            }
//        }

//        public IEnumerable<MenuItem> GetByMenuItemId(System.Guid menuItemId, bool? isDeleted = false)
//        {
//            return _menuItemCashRepository.GetByModifiedBy(menuItemId, isDeleted);
//        }

//        public IEnumerable<MenuItem> GetAll(bool? isDeleted = false)
//        {
//            return _menuItemCashRepository.GetAll();
//        }

//    }
//}
