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
using YumaPos.Server.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;
using YumaPos.FrontEnd.Infrastructure.Configuration;
using YumaPos.Server.Data.Sql;
using YumaPos.Server.Data.Sql.Menu;


namespace YumaPos.Server.Data.Sql
{
	public partial class RecipeItemsLanguageRepository : IRecipeItemsLanguageRepository
	{
		private IDataAccessController _dataAccessController;
		private RecipeItemsLanguageCacheRepository _recipeItemsLanguageCacheRepository;
		private RecipeItemsLanguageVersionRepository _recipeItemsLanguageVersionRepository;
		private MenuItemRepository _menuItemRepository;


		public RecipeItemsLanguageRepository(IDataAccessController dataAccessController,
		IDataAccessService dataAccessService,
		IMenuItemRepository menuItemRepository)
		{
			_dataAccessController = dataAccessController;
			_recipeItemsLanguageCacheRepository = new RecipeItemsLanguageCacheRepository(dataAccessService);
			_recipeItemsLanguageVersionRepository = new RecipeItemsLanguageVersionRepository(dataAccessService);
			_menuItemRepository = (MenuItemRepository)menuItemRepository;
		}

		/*
		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage> GetAll(bool? isDeleted = false)
		{
		return _recipeItemsLanguageCacheRepository.GetAll(isDeleted);
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>> GetAllAsync(bool? isDeleted = false)
		{
		return await _recipeItemsLanguageCacheRepository.GetAllAsync(isDeleted);
		}

		*/
		public YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage GetByItemIdAndLanguage(System.Guid itemId, string language, bool? isDeleted = false)
		{
			return _recipeItemsLanguageCacheRepository.GetByItemIdAndLanguage(itemId, language, isDeleted);
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage> GetByItemIdAndLanguageAsync(System.Guid itemId, string language, bool? isDeleted = false)
		{
			return await _recipeItemsLanguageCacheRepository.GetByItemIdAndLanguageAsync(itemId, language, isDeleted);
		}

		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage> GetByItemId(System.Guid itemId, bool? isDeleted = false)
		{
			var res = _recipeItemsLanguageCacheRepository.GetByItemId(itemId, isDeleted);
			return res;
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>> GetByItemIdAsync(System.Guid itemId, bool? isDeleted = false)
		{
			var res = await _recipeItemsLanguageCacheRepository.GetByItemIdAsync(itemId, isDeleted);
			return res;
		}

		public YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage GetByItemIdVersionId(System.Guid itemIdVersionId, bool? isDeleted = false)
		{
			return _recipeItemsLanguageVersionRepository.GetByItemIdVersionId(itemIdVersionId, isDeleted);
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage> GetByItemIdVersionIdAsync(System.Guid itemIdVersionId, bool? isDeleted = false)
		{
			return await _recipeItemsLanguageVersionRepository.GetByItemIdVersionIdAsync(itemIdVersionId, isDeleted);
		}

		public void Insert(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			recipeItemsLanguage.Modified = DateTimeOffset.Now;
			recipeItemsLanguage.ModifiedBy = _dataAccessController.EmployeeId.Value;
			recipeItemsLanguage.ItemIdVersionId = Guid.NewGuid();
			if (recipeItemsLanguage.ItemId == null || recipeItemsLanguage.ItemId == Guid.Empty)
			{
				throw new ArgumentException("ItemIdAndLanguage");
			}
			_recipeItemsLanguageVersionRepository.Insert(recipeItemsLanguage);
			_recipeItemsLanguageCacheRepository.Insert(recipeItemsLanguage);
			UpdateMenuItem(recipeItemsLanguage);

		}
		public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			recipeItemsLanguage.Modified = DateTimeOffset.Now;
			recipeItemsLanguage.ModifiedBy = _dataAccessController.EmployeeId.Value;
			recipeItemsLanguage.ItemIdVersionId = Guid.NewGuid();
			if (recipeItemsLanguage.ItemId == null || recipeItemsLanguage.ItemId == Guid.Empty)
			{
				throw new ArgumentException("ItemIdAndLanguage");
			}
			await _recipeItemsLanguageVersionRepository.InsertAsync(recipeItemsLanguage);
			await _recipeItemsLanguageCacheRepository.InsertAsync(recipeItemsLanguage);
			UpdateMenuItem(recipeItemsLanguage);

		}

		/*
		public void UpdateByItemIdAndLanguage(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
		recipeItemsLanguage.Modified = DateTimeOffset.Now;
		recipeItemsLanguage.ModifiedBy = _dataAccessController.EmployeeId.Value;
		recipeItemsLanguage.ItemIdVersionId = Guid.NewGuid();
		_recipeItemsLanguageVersionRepository.Insert(recipeItemsLanguage);
		_recipeItemsLanguageCacheRepository.UpdateByItemIdAndLanguage(recipeItemsLanguage);
		UpdateMenuItem(recipeItemsLanguage);
		}
		public async Task UpdateByItemIdAndLanguageAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
		recipeItemsLanguage.Modified = DateTimeOffset.Now;
		recipeItemsLanguage.ModifiedBy = _dataAccessController.EmployeeId.Value;
		recipeItemsLanguage.ItemIdVersionId = Guid.NewGuid();
		await _recipeItemsLanguageVersionRepository.InsertAsync(recipeItemsLanguage);
		await _recipeItemsLanguageCacheRepository.UpdateByItemIdAndLanguageAsync(recipeItemsLanguage);
		UpdateMenuItem(recipeItemsLanguage);
		}

		*/
		public void UpdateByItemId(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			recipeItemsLanguage.Modified = DateTimeOffset.Now;
			recipeItemsLanguage.ModifiedBy = _dataAccessController.EmployeeId.Value;
			recipeItemsLanguage.ItemIdVersionId = Guid.NewGuid();
			_recipeItemsLanguageVersionRepository.Insert(recipeItemsLanguage);
			_recipeItemsLanguageCacheRepository.UpdateByItemId(recipeItemsLanguage);
			UpdateMenuItem(recipeItemsLanguage);
		}
		public async Task UpdateByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			recipeItemsLanguage.Modified = DateTimeOffset.Now;
			recipeItemsLanguage.ModifiedBy = _dataAccessController.EmployeeId.Value;
			recipeItemsLanguage.ItemIdVersionId = Guid.NewGuid();
			await _recipeItemsLanguageVersionRepository.InsertAsync(recipeItemsLanguage);
			await _recipeItemsLanguageCacheRepository.UpdateByItemIdAsync(recipeItemsLanguage);
			UpdateMenuItem(recipeItemsLanguage);
		}

		private void UpdateMenuItem(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			var item = _menuItemRepository.GetByMenuItemId(recipeItemsLanguage.ItemId);
			_menuItemRepository.UpdateByMenuItemId(item);
		}

		/*
		public void RemoveByItemIdAndLanguage(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
		recipeItemsLanguage.IsDeleted = true;
		UpdateByItemIdAndLanguage(recipeItemsLanguage);
		}
		public async Task RemoveByItemIdAndLanguageAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
		recipeItemsLanguage.IsDeleted = true;
		await UpdateByItemIdAndLanguageAsync(recipeItemsLanguage);
		}
		public void RemoveByItemIdAndLanguage(System.Guid itemId,string language)
		{
		var result = _recipeItemsLanguageCacheRepository.GetByItemIdAndLanguage(itemId,language);
		result.IsDeleted = true;
		UpdateByItemIdAndLanguage(result);
		}
		public async Task RemoveByItemIdAndLanguageAsync(System.Guid itemId,string language)
		{
		var result = await _recipeItemsLanguageCacheRepository.GetByItemIdAndLanguageAsync(itemId,language);
		result.IsDeleted = true;
		await UpdateByItemIdAndLanguageAsync(result);
		}

		*/
		public void RemoveByItemId(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			recipeItemsLanguage.IsDeleted = true;
			_recipeItemsLanguageCacheRepository.UpdateByItemId(recipeItemsLanguage);
		}
		public async Task RemoveByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			recipeItemsLanguage.IsDeleted = true;
			await _recipeItemsLanguageCacheRepository.UpdateByItemIdAsync(recipeItemsLanguage);
		}
		public void RemoveByItemId(System.Guid itemId)
		{
			var result = _recipeItemsLanguageCacheRepository.GetByItemId(itemId);
			foreach (var item in result)
			{
				item.IsDeleted = true;
				UpdateByItemId(item);
			}
		}
		public async Task RemoveByItemIdAsync(System.Guid itemId)
		{
			var result = await _recipeItemsLanguageCacheRepository.GetByItemIdAsync(itemId);
			foreach (var item in result)
			{
				item.IsDeleted = true;
				await UpdateByItemIdAsync(item);
			}
		}


	}
}