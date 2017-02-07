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
using TestRepositoryGeneration.RepositoryInterfaces;


namespace TestRepositoryGeneration.CustomRepositories.VersionsRepositories
{
	internal partial class MenuItems2TaxesCacheRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase
	{
		private const string Fields = @"[MenuItems2Taxess].[MenuItemId],[MenuItems2Taxess].[MenuItemVersionId],[MenuItems2Taxess].[Modified],[MenuItems2Taxess].[ModifiedBy],[MenuItems2Taxess].[TaxId],[MenuItems2Taxess].[TaxVersionId],[MenuItems2Taxess].[IsDeleted]";
		private const string SelectAllQuery = @"SELECT [MenuItems2Taxess].[MenuItemId],[MenuItems2Taxess].[MenuItemVersionId],[MenuItems2Taxess].[Modified],[MenuItems2Taxess].[ModifiedBy],[MenuItems2Taxess].[TaxId],[MenuItems2Taxess].[TaxVersionId],[MenuItems2Taxess].[IsDeleted] FROM [MenuItems2Taxess]   ";
		private const string SelectByQuery = @"SELECT [MenuItems2Taxess].[MenuItemId],[MenuItems2Taxess].[MenuItemVersionId],[MenuItems2Taxess].[Modified],[MenuItems2Taxess].[ModifiedBy],[MenuItems2Taxess].[TaxId],[MenuItems2Taxess].[TaxVersionId],[MenuItems2Taxess].[IsDeleted] FROM [MenuItems2Taxess] ";
		private const string InsertQuery = @"INSERT INTO [MenuItems2Taxess]([MenuItems2Taxess].[MenuItemId],[MenuItems2Taxess].[MenuItemVersionId],[MenuItems2Taxess].[Modified],[MenuItems2Taxess].[ModifiedBy],[MenuItems2Taxess].[TaxId],[MenuItems2Taxess].[TaxVersionId],[MenuItems2Taxess].[IsDeleted])  VALUES(@MenuItemId,@MenuItemVersionId,@Modified,@ModifiedBy,@TaxId,@TaxVersionId,@IsDeleted) ";
		private const string UpdateQueryBy = @"UPDATE [MenuItems2Taxess] SET [MenuItems2Taxess].[MenuItemId] = @MenuItemId,[MenuItems2Taxess].[MenuItemVersionId] = @MenuItemVersionId,[MenuItems2Taxess].[Modified] = @Modified,[MenuItems2Taxess].[ModifiedBy] = @ModifiedBy,[MenuItems2Taxess].[TaxId] = @TaxId,[MenuItems2Taxess].[TaxVersionId] = @TaxVersionId,[MenuItems2Taxess].[IsDeleted] = @IsDeleted FROM [MenuItems2Taxess] ";
		private const string DeleteQueryBy = @"DELETE FROM [MenuItems2Taxess] ";
		private const string WhereQueryByMenuItemId = "WHERE [MenuItems2Taxess].[MenuItemId] = @MenuItemId ";
		private const string WhereQueryByTaxId = "WHERE [MenuItems2Taxess].[TaxId] = @TaxId ";
		private const string AndWithIsDeletedFilter = "AND [MenuItems2Taxess].[IsDeleted] = @IsDeleted ";


		public MenuItems2TaxesCacheRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes> GetAll(bool? isDeleted = false)
		{
			var sql = SelectAllQuery;
			object parameters = new { isDeleted };
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>(sql, parameters).ToList();
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>> GetAllAsync(bool? isDeleted = false)
		{
			var sql = SelectAllQuery;
			object parameters = new { isDeleted };
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>(sql, parameters));
			return result.ToList();
		}

		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes> GetByMenuItemId(System.Guid menuItemId, bool? isDeleted = false)
		{
			object parameters = new { menuItemId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuItemId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>> GetByMenuItemIdAsync(System.Guid menuItemId, bool? isDeleted = false)
		{
			object parameters = new { menuItemId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuItemId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>(sql, parameters));
			return result.ToList();
		}

		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes> GetByTaxId(int taxId, bool? isDeleted = false)
		{
			object parameters = new { taxId, isDeleted };
			var sql = SelectByQuery + WhereQueryByTaxId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>> GetByTaxIdAsync(int taxId, bool? isDeleted = false)
		{
			object parameters = new { taxId, isDeleted };
			var sql = SelectByQuery + WhereQueryByTaxId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>(sql, parameters));
			return result.ToList();
		}


		public void Insert(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes menuItems2Taxes)
		{
			DataAccessService.InsertObject(menuItems2Taxes, InsertQuery);
		}
		public async Task InsertAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes menuItems2Taxes)
		{
			await DataAccessService.InsertObjectAsync(menuItems2Taxes, InsertQuery);
		}

		public void UpdateByMenuItemId(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes menuItems2Taxes)
		{
			var sql = UpdateQueryBy + WhereQueryByMenuItemId;
			DataAccessService.PersistObject(menuItems2Taxes, sql);
		}
		public async Task UpdateByMenuItemIdAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes menuItems2Taxes)
		{
			var sql = UpdateQueryBy + WhereQueryByMenuItemId;
			await DataAccessService.PersistObjectAsync(menuItems2Taxes, sql);
		}

		public void UpdateByTaxId(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes menuItems2Taxes)
		{
			var sql = UpdateQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject(menuItems2Taxes, sql);
		}
		public async Task UpdateByTaxIdAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes menuItems2Taxes)
		{
			var sql = UpdateQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync(menuItems2Taxes, sql);
		}


		public void RemoveByMenuItemId(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes menuItems2Taxes)
		{
			var sql = DeleteQueryBy + WhereQueryByMenuItemId;
			DataAccessService.PersistObject(menuItems2Taxes, sql);
		}
		public async Task RemoveByMenuItemIdAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes menuItems2Taxes)
		{
			var sql = DeleteQueryBy + WhereQueryByMenuItemId;
			await DataAccessService.PersistObjectAsync(menuItems2Taxes, sql);
		}

		public void RemoveByMenuItemId(System.Guid menuItemId)
		{
			object parameters = new { menuItemId };
			var sql = DeleteQueryBy + WhereQueryByMenuItemId;
			DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>(sql, parameters);
		}
		public async Task RemoveByMenuItemIdAsync(System.Guid menuItemId)
		{
			object parameters = new { menuItemId };
			var sql = DeleteQueryBy + WhereQueryByMenuItemId;
			await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>(sql, parameters);
		}

		public void RemoveByTaxId(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes menuItems2Taxes)
		{
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject(menuItems2Taxes, sql);
		}
		public async Task RemoveByTaxIdAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes menuItems2Taxes)
		{
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync(menuItems2Taxes, sql);
		}

		public void RemoveByTaxId(int taxId)
		{
			object parameters = new { taxId };
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>(sql, parameters);
		}
		public async Task RemoveByTaxIdAsync(int taxId)
		{
			object parameters = new { taxId };
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItems2Taxes>(sql, parameters);
		}



	}
}
