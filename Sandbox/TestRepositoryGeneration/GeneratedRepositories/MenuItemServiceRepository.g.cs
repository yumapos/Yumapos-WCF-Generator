﻿//------------------------------------------------------------------------------
// <auto-generated>
//	 This code was generated from a template.
//
//	 Manual changes to this file may cause unexpected behavior in your application.
//	 Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Globalization;


namespace TestRepositoryGeneration.CustomRepositories.VersionsRepositories
{
	public partial class MenuItemRepository : TestRepositoryGeneration.RepositoryInterfaces.IMenuItemRepository
	{
		private TestRepositoryGeneration.Infrastructure.IDataAccessController _dataAccessController;
		private TestRepositoryGeneration.Infrastructure.IDateTimeService _dateTimeService;
		private TestRepositoryGeneration.CustomRepositories.VersionsRepositories.MenuItemCacheRepository _menuItemCacheRepository;
		private TestRepositoryGeneration.CustomRepositories.VersionsRepositories.MenuItemVersionRepository _menuItemVersionRepository;
		private TestRepositoryGeneration.CustomRepositories.VersionsRepositories.MenuItems2TaxesCacheRepository _menuItems2TaxesCacheRepository;
		private TestRepositoryGeneration.CustomRepositories.VersionsRepositories.MenuItems2TaxesVersionRepository _menuItems2TaxesVersionRepository;
		private TestRepositoryGeneration.TaxCacheRepository _taxCacheRepository;


		public MenuItemRepository(TestRepositoryGeneration.Infrastructure.IDataAccessController dataAccessController,
		TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService,
		TestRepositoryGeneration.Infrastructure.IDateTimeService dateTimeService)
		{
			_dataAccessController = dataAccessController;
			_dateTimeService = dateTimeService;
			_menuItemCacheRepository = new TestRepositoryGeneration.CustomRepositories.VersionsRepositories.MenuItemCacheRepository(dataAccessService, dataAccessController);
			_menuItemVersionRepository = new TestRepositoryGeneration.CustomRepositories.VersionsRepositories.MenuItemVersionRepository(dataAccessService, dataAccessController);
			_menuItems2TaxesCacheRepository = new TestRepositoryGeneration.CustomRepositories.VersionsRepositories.MenuItems2TaxesCacheRepository(dataAccessService, dataAccessController);
			_menuItems2TaxesVersionRepository = new TestRepositoryGeneration.CustomRepositories.VersionsRepositories.MenuItems2TaxesVersionRepository(dataAccessService, dataAccessController);
			_taxCacheRepository = new TestRepositoryGeneration.TaxCacheRepository(dataAccessService, dataAccessController);
		}

		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetAll(bool? isDeleted = false)
		{
			return _menuItemCacheRepository.GetAll(isDeleted);
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>> GetAllAsync(bool? isDeleted = false)
		{
			return await _menuItemCacheRepository.GetAllAsync(isDeleted);
		}

		public TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem GetByMenuItemId(System.Guid menuItemId, DateTimeOffset modified, bool? isDeleted = false)
		{
			var result = _menuItemVersionRepository.GetByMenuItemId(menuItemId, modified, isDeleted);
			return result;
		}

		public async Task<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetByMenuItemIdAsync(System.Guid menuItemId, DateTimeOffset modified, bool? isDeleted = false)
		{
			var result = await _menuItemVersionRepository.GetByMenuItemIdAsync(menuItemId, modified, isDeleted);
			return result;
		}

		public TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem GetByMenuItemId(System.Guid menuItemId, bool? isDeleted = false)
		{
			var result = _menuItemCacheRepository.GetByMenuItemId(menuItemId, isDeleted);
			return result;
		}

		public async Task<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetByMenuItemIdAsync(System.Guid menuItemId, bool? isDeleted = false)
		{
			var result = await _menuItemCacheRepository.GetByMenuItemIdAsync(menuItemId, isDeleted);
			return result;
		}


		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetByMenuCategoryId(System.Guid menuCategoryId, DateTimeOffset modified, bool? isDeleted = false)
		{
			var result = _menuItemVersionRepository.GetByMenuCategoryId(menuCategoryId, modified, isDeleted);
			return result.ToList();
		}

		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>> GetByMenuCategoryIdAsync(System.Guid menuCategoryId, DateTimeOffset modified, bool? isDeleted = false)
		{
			var result = await _menuItemVersionRepository.GetByMenuCategoryIdAsync(menuCategoryId, modified, isDeleted);
			return result.ToList();
		}

		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetByMenuCategoryId(System.Guid menuCategoryId, bool? isDeleted = false)
		{
			var result = _menuItemCacheRepository.GetByMenuCategoryId(menuCategoryId, isDeleted);
			return result.ToList();
		}

		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>> GetByMenuCategoryIdAsync(System.Guid menuCategoryId, bool? isDeleted = false)
		{
			var result = await _menuItemCacheRepository.GetByMenuCategoryIdAsync(menuCategoryId, isDeleted);
			return result.ToList();
		}


		public TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem GetByMenuItemVersionId(System.Guid menuItemVersionId, bool? isDeleted = false)
		{
			var result = _menuItemVersionRepository.GetByMenuItemVersionId(menuItemVersionId, isDeleted);
			return result;
		}

		public async Task<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetByMenuItemVersionIdAsync(System.Guid menuItemVersionId, bool? isDeleted = false)
		{
			var result = await _menuItemVersionRepository.GetByMenuItemVersionIdAsync(menuItemVersionId, isDeleted);
			return result;
		}


		public Guid Insert(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			menuItem.Modified = _dateTimeService.CurrentDateTimeOffset;
			menuItem.ModifiedBy = _dataAccessController.EmployeeId.Value;
			menuItem.MenuItemVersionId = menuItem.MenuItemVersionId == Guid.Empty ? Guid.NewGuid() : menuItem.MenuItemVersionId;
			if (menuItem.MenuItemId == null || menuItem.MenuItemId == Guid.Empty)
			{
				throw new ArgumentException("MenuItemId");
			}
			_menuItemVersionRepository.Insert(menuItem);
			_menuItemCacheRepository.Insert(menuItem);
			UpdateMenuItems2Taxes(menuItem);
			return menuItem.MenuItemVersionId;
		}
		public async Task<Guid> InsertAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			menuItem.Modified = _dateTimeService.CurrentDateTimeOffset;
			menuItem.ModifiedBy = _dataAccessController.EmployeeId.Value;
			menuItem.MenuItemVersionId = menuItem.MenuItemVersionId == Guid.Empty ? Guid.NewGuid() : menuItem.MenuItemVersionId;
			if (menuItem.MenuItemId == null || menuItem.MenuItemId == Guid.Empty)
			{
				throw new ArgumentException("MenuItemId");
			}
			await _menuItemVersionRepository.InsertAsync(menuItem);
			await _menuItemCacheRepository.InsertAsync(menuItem);
			UpdateMenuItems2Taxes(menuItem);
			return menuItem.MenuItemVersionId;
		}

		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> InsertMany(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> menuItemList)
		{
			foreach (var menuItem in menuItemList)
			{
				menuItem.Modified = _dateTimeService.CurrentDateTimeOffset;
				menuItem.ModifiedBy = _dataAccessController.EmployeeId.Value;
				menuItem.MenuItemVersionId = Guid.NewGuid();
				if (menuItem.MenuItemId == null || menuItem.MenuItemId == Guid.Empty)
				{
					throw new ArgumentException("MenuItemId");
				}
			}
			_menuItemVersionRepository.InsertMany(menuItemList);
			_menuItemCacheRepository.InsertMany(menuItemList);
			UpdateManyMenuItems2Taxes(menuItemList);
			return menuItemList;
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>> InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> menuItemList)
		{
			foreach (var menuItem in menuItemList)
			{
				menuItem.Modified = _dateTimeService.CurrentDateTimeOffset;
				menuItem.ModifiedBy = _dataAccessController.EmployeeId.Value;
				menuItem.MenuItemVersionId = Guid.NewGuid();
				if (menuItem.MenuItemId == null || menuItem.MenuItemId == Guid.Empty)
				{
					throw new ArgumentException("MenuItemId");
				}
			}
			await _menuItemVersionRepository.InsertManyAsync(menuItemList);
			await _menuItemCacheRepository.InsertManyAsync(menuItemList);
			UpdateManyMenuItems2Taxes(menuItemList);
			return menuItemList;
		}

		public void UpdateByMenuItemId(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			menuItem.Modified = _dateTimeService.CurrentDateTimeOffset;
			menuItem.ModifiedBy = _dataAccessController.EmployeeId.Value;
			menuItem.MenuItemVersionId = Guid.NewGuid();
			_menuItemVersionRepository.Insert(menuItem);
			_menuItemCacheRepository.UpdateByMenuItemId(menuItem);
			UpdateMenuItems2Taxes(menuItem);
		}
		public async Task UpdateByMenuItemIdAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			menuItem.Modified = _dateTimeService.CurrentDateTimeOffset;
			menuItem.ModifiedBy = _dataAccessController.EmployeeId.Value;
			menuItem.MenuItemVersionId = Guid.NewGuid();
			await _menuItemVersionRepository.InsertAsync(menuItem);
			await _menuItemCacheRepository.UpdateByMenuItemIdAsync(menuItem);
			UpdateMenuItems2Taxes(menuItem);
		}
		/*
		public void UpdateByMenuCategoryId(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
		menuItem.Modified = _dateTimeService.CurrentDateTimeOffset;
		menuItem.ModifiedBy = _dataAccessController.EmployeeId.Value;
		menuItem.MenuItemVersionId = Guid.NewGuid();
		_menuItemVersionRepository.Insert(menuItem);
		_menuItemCacheRepository.UpdateByMenuCategoryId(menuItem);
		UpdateMenuItems2Taxes(menuItem);
		}
		public async Task UpdateByMenuCategoryIdAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
		menuItem.Modified = _dateTimeService.CurrentDateTimeOffset;
		menuItem.ModifiedBy = _dataAccessController.EmployeeId.Value;
		menuItem.MenuItemVersionId = Guid.NewGuid();
		await _menuItemVersionRepository.InsertAsync(menuItem);
		await _menuItemCacheRepository.UpdateByMenuCategoryIdAsync(menuItem);
		UpdateMenuItems2Taxes(menuItem);
		}

		*/
		private void UpdateMenuItems2Taxes(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			if (menuItem.TaxIds == null)
				menuItem.TaxIds = _menuItems2TaxesCacheRepository.GetByMenuItemId(menuItem.MenuItemId).Select(i => i.TaxId);
			var listOfMenuItems2Taxes = menuItem.TaxIds
			.Select(ids => _taxCacheRepository.GetByTaxId(ids, null))
			.Select(item => new TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes()
			{
				TaxId = item.TaxId,
				TaxVersionId = item.TaxVersionId,
				MenuItemId = menuItem.MenuItemId,
				MenuItemVersionId = menuItem.MenuItemVersionId,
			}).ToList();
			_menuItems2TaxesCacheRepository.RemoveByMenuItemId(menuItem.MenuItemId);
			foreach (var mt in listOfMenuItems2Taxes)
			{
				mt.Modified = _dateTimeService.CurrentDateTimeOffset;
				mt.ModifiedBy = menuItem.ModifiedBy;
				_menuItems2TaxesCacheRepository.Insert(mt);
				_menuItems2TaxesVersionRepository.Insert(mt);
			}
		}
		private void UpdateManyMenuItems2Taxes(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> menuItemList)
		{
			foreach (var menuItem in menuItemList)
			{
				UpdateMenuItems2Taxes(menuItem);
			}
		}

		public void RemoveByMenuItemId(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			menuItem.IsDeleted = true;
			UpdateByMenuItemId(menuItem);
		}
		public async Task RemoveByMenuItemIdAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			menuItem.IsDeleted = true;
			await UpdateByMenuItemIdAsync(menuItem);
		}
		public void RemoveByMenuItemId(System.Guid menuItemId)
		{
			var result = _menuItemCacheRepository.GetByMenuItemId(menuItemId);
			result.IsDeleted = true;
			UpdateByMenuItemId(result);
		}
		public async Task RemoveByMenuItemIdAsync(System.Guid menuItemId)
		{
			var result = await _menuItemCacheRepository.GetByMenuItemIdAsync(menuItemId);
			result.IsDeleted = true;
			await UpdateByMenuItemIdAsync(result);
		}

		/*
		public void RemoveByMenuCategoryId(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
		menuItem.IsDeleted = true;
		_menuItemCacheRepository.UpdateByMenuCategoryId(menuItem);
		}
		public async Task RemoveByMenuCategoryIdAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
		menuItem.IsDeleted = true;
		await _menuItemCacheRepository.UpdateByMenuCategoryIdAsync(menuItem);
		}
		public void RemoveByMenuCategoryId(System.Guid menuCategoryId)
		{
		var result = _menuItemCacheRepository.GetByMenuCategoryId(menuCategoryId);
		foreach (var item in result)
		{
		item.IsDeleted = true;
		UpdateByMenuCategoryId(item);
		}
		}
		public async Task RemoveByMenuCategoryIdAsync(System.Guid menuCategoryId)
		{
		var result = await _menuItemCacheRepository.GetByMenuCategoryIdAsync(menuCategoryId);
		foreach (var item in result)
		{
		item.IsDeleted = true;
		await UpdateByMenuCategoryIdAsync(item);
		}
		}

		*/

	}
}
