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
using YumaPos.Server.Data.Sql;



namespace TestRepositoryGeneration
{
	internal partial class MenuItemToTaxCacheRepository : RepositoryBase, IMenuItemToTaxRepository
	{
		private const string Fields = @"[MenuItemToTaxs].[MenuItemId],[MenuItemToTaxs].[MenuItemVersionId],[MenuItemToTaxs].[Modified],[MenuItemToTaxs].[ModifiedBy],[MenuItemToTaxs].[TaxId],[MenuItemToTaxs].[TaxVersionId],[MenuItemToTaxs].[IsDeleted]{columns}";
		private const string Values = @"@MenuItemId,@MenuItemVersionId,@Modified,@ModifiedBy,@TaxId,@TaxVersionId,@IsDeleted{values}";
		private const string SelectAllQuery = @"SELECT [MenuItemToTaxs].[MenuItemId],[MenuItemToTaxs].[MenuItemVersionId],[MenuItemToTaxs].[Modified],[MenuItemToTaxs].[ModifiedBy],[MenuItemToTaxs].[TaxId],[MenuItemToTaxs].[TaxVersionId],[MenuItemToTaxs].[IsDeleted] FROM [MenuItemToTaxs]   ";
		private const string SelectByQuery = @"SELECT [MenuItemToTaxs].[MenuItemId],[MenuItemToTaxs].[MenuItemVersionId],[MenuItemToTaxs].[Modified],[MenuItemToTaxs].[ModifiedBy],[MenuItemToTaxs].[TaxId],[MenuItemToTaxs].[TaxVersionId],[MenuItemToTaxs].[IsDeleted] FROM [MenuItemToTaxs] ";
		private const string InsertQuery = @"INSERT INTO MenuItemToTaxs([MenuItemToTaxs].[MenuItemId],[MenuItemToTaxs].[MenuItemVersionId],[MenuItemToTaxs].[Modified],[MenuItemToTaxs].[ModifiedBy],[MenuItemToTaxs].[TaxId],[MenuItemToTaxs].[TaxVersionId],[MenuItemToTaxs].[IsDeleted]{columns})  VALUES(@MenuItemId,@MenuItemVersionId,@Modified,@ModifiedBy,@TaxId,@TaxVersionId,@IsDeleted{values}) ";
		private const string UpdateQueryBy = @"UPDATE [MenuItemToTaxs] SET MenuItemToTaxs.[MenuItemId] = @MenuItemId,MenuItemToTaxs.[MenuItemVersionId] = @MenuItemVersionId,MenuItemToTaxs.[Modified] = @Modified,MenuItemToTaxs.[ModifiedBy] = @ModifiedBy,MenuItemToTaxs.[TaxId] = @TaxId,MenuItemToTaxs.[TaxVersionId] = @TaxVersionId,MenuItemToTaxs.[IsDeleted] = @IsDeleted FROM [MenuItemToTaxs] ";
		private const string DeleteQueryBy = @"DELETE FROM [MenuItemToTaxs] ";
		private const string SelectIntoTempTable = @"DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [MenuItemToTaxs].[] FROM [MenuItemToTaxs] ";
		private const string WhereQueryByMenuItemId = "WHERE MenuItemToTaxs.[MenuItemId] = @MenuItemId ";
		private const string WhereQueryByTaxId = "WHERE MenuItemToTaxs.[TaxId] = @TaxId ";
		private const string AndWithFilterData = "AND MenuItemToTaxs.[IsDeleted] = @IsDeleted";


		public MenuItemToTaxCacheRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax> GetAll(bool? isDeleted = false)
		{
			object parameters = new { isDeleted };
			var sql = SelectAllQuery;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters).ToList();
			return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>> GetAllAsync(bool? isDeleted = false)
		{
			object parameters = new { isDeleted };
			var sql = SelectAllQuery;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters));
			return result.ToList();
		}

		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax> GetByMenuItemId(System.Guid menuItemId, bool? isDeleted = false)
		{
			object parameters = new { menuItemId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuItemId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>> GetByMenuItemIdAsync(System.Guid menuItemId, bool? isDeleted = false)
		{
			object parameters = new { menuItemId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuItemId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters));
			return result.ToList();
		}

		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax> GetByTaxId(int taxId, bool? isDeleted = false)
		{
			object parameters = new { taxId, isDeleted };
			var sql = SelectByQuery + WhereQueryByTaxId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>> GetByTaxIdAsync(int taxId, bool? isDeleted = false)
		{
			object parameters = new { taxId, isDeleted };
			var sql = SelectByQuery + WhereQueryByTaxId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters));
			return result.ToList();
		}


		public void Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			DataAccessService.InsertObject(menuItemToTax, InsertQuery);
		}
		public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			await DataAccessService.InsertObjectAsync(menuItemToTax, InsertQuery);
		}

		public void UpdateByMenuItemId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			var sql = UpdateQueryBy + WhereQueryByMenuItemId;
			DataAccessService.PersistObject(menuItemToTax, sql);
		}
		public async Task UpdateByMenuItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			var sql = UpdateQueryBy + WhereQueryByMenuItemId;
			await DataAccessService.PersistObjectAsync(menuItemToTax, sql);
		}

		public void UpdateByTaxId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			var sql = UpdateQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject(menuItemToTax, sql);
		}
		public async Task UpdateByTaxIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			var sql = UpdateQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync(menuItemToTax, sql);
		}


		public void RemoveByMenuItemId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			var sql = DeleteQueryBy + WhereQueryByMenuItemId;
			DataAccessService.PersistObject(menuItemToTax, sql);
		}
		public async Task RemoveByMenuItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			var sql = DeleteQueryBy + WhereQueryByMenuItemId;
			await DataAccessService.PersistObjectAsync(menuItemToTax, sql);
		}

		public void RemoveByMenuItemId(System.Guid menuItemId)
		{
			object parameters = new { menuItemId };
			var sql = DeleteQueryBy + WhereQueryByMenuItemId;
			DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters);
		}
		public async Task RemoveByMenuItemIdAsync(System.Guid menuItemId)
		{
			object parameters = new { menuItemId };
			var sql = DeleteQueryBy + WhereQueryByMenuItemId;
			await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters);
		}

		public void RemoveByTaxId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject(menuItemToTax, sql);
		}
		public async Task RemoveByTaxIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync(menuItemToTax, sql);
		}

		public void RemoveByTaxId(int taxId)
		{
			object parameters = new { taxId };
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters);
		}
		public async Task RemoveByTaxIdAsync(int taxId)
		{
			object parameters = new { taxId };
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters);
		}



	}
}
