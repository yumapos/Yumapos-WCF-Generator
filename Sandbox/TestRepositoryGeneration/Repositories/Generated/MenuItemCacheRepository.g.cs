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
	internal partial class MenuItemCacheRepository : RepositoryBase, IMenuItemRepository
	{
		private const string Fields = "[MenuItems].[Name],[MenuItems].[Modified],[MenuItems].[ModifiedBy],[MenuItems].[TaxIds],[MenuItems].[MenuCategoryId]{columns}";
		private const string Values = "@Name,@Modified,@ModifiedBy,@TaxIds,@MenuCategoryId{values}";
		private const string SelectAllQuery = "SELECT [MenuItems].[Name],[MenuItems].[Modified],[MenuItems].[ModifiedBy],[MenuItems].[TaxIds],[MenuItems].[MenuCategoryId],[RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[CategoryId] FROM [MenuItems] INNER JOIN [RecipieItems] ON [MenuItems].[ItemId] = [RecipieItems].[ItemId]  {whereTenantId:[MenuItems]} ";
		private const string SelectByQuery = "SELECT [MenuItems].[Name],[MenuItems].[Modified],[MenuItems].[ModifiedBy],[MenuItems].[TaxIds],[MenuItems].[MenuCategoryId],[RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[CategoryId] FROM [MenuItems] INNER JOIN [RecipieItems] ON [MenuItems].[ItemId] = [RecipieItems].[ItemId] ";
		private const string InsertQuery = "DECLARE @Temp TABLE (ItemId uniqueidentifier); DECLARE @TempPKItemId uniqueidentifier;INSERT INTO RecipieItems([RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[CategoryId]{columns}) OUTPUT INSERTED.ItemId VALUES(@ItemId,@ItemVersionId,@IsDeleted,@CategoryId{values}) SELECT @TempPKItemId = ItemId FROM @Temp INSERT INTO MenuItems([MenuItems].[Name],[MenuItems].[Modified],[MenuItems].[ModifiedBy],[MenuItems].[TaxIds],[MenuItems].[MenuCategoryId]{columns}) OUTPUT INSERTED.ItemId VALUES(@TempPKItemId,@Name,@Modified,@ModifiedBy,@TaxIds,@MenuCategoryId{values}) ";
		private const string UpdateQueryBy = "UPDATE [MenuItems] SET MenuItems.[Name] = @Name,MenuItems.[Modified] = @Modified,MenuItems.[ModifiedBy] = @ModifiedBy,MenuItems.[TaxIds] = @TaxIds,MenuItems.[MenuCategoryId] = @MenuCategoryId FROM [MenuItems] ";
		private const string DeleteQueryBy = "DELETE FROM [MenuItems] WHERE [MenuItems].[ItemId] IN (SELECT ItemId FROM @Temp);DELETE FROM [RecipieItems] WHERE [RecipieItems].[ItemId] IN (SELECT ItemId FROM @Temp); ";
		private const string SelectIntoTempTable = "DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [MenuItems].[ItemId] FROM [MenuItems] ";
		private const string UpdateQueryJoin = "UPDATE [RecipieItems] SET MenuItems.[ItemId] = @ItemId,MenuItems.[ItemVersionId] = @ItemVersionId,MenuItems.[IsDeleted] = @IsDeleted,MenuItems.[CategoryId] = @CategoryId FROM [RecipieItems] INNER JOIN [RecipieItems] ON [MenuItems].[ItemId] = [RecipieItems].[ItemId] ";
		private const string WhereQueryByItemId = "WHERE MenuItems.[ItemId] = @ItemId{andTenantId:[MenuItems]} ";
		private const string WhereQueryByItemVersionId = "WHERE MenuItems.[ItemVersionId] = @ItemVersionId{andTenantId:[MenuItems]} ";
		private const string WhereQueryByMenuCategoryId = "WHERE MenuItems.[MenuCategoryId] = @MenuCategoryId{andTenantId:[MenuItems]} ";
		private const string AndWithFilterData = "AND RecipieItems.[IsDeleted] = @IsDeleted";


		public MenuItemCacheRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetAll(bool? isDeleted = false)
		{
			object parameters = new { isDeleted };
			var sql = SelectAllQuery;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters).ToList();
			return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem>> GetAllAsync(bool? isDeleted = false)
		{
			object parameters = new { isDeleted };
			var sql = SelectAllQuery;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters));
			return result.ToList();
		}

		public YumaPos.Server.Infrastructure.DataObjects.MenuItem GetByItemId(Guid itemId, bool? isDeleted = false)
		{
			object parameters = new { itemId, isDeleted };
			var sql = SelectByQuery + WhereQueryByItemId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetByItemIdAsync(Guid itemId, bool? isDeleted = false)
		{
			object parameters = new { itemId, isDeleted };
			var sql = SelectByQuery + WhereQueryByItemId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters));
			return result.FirstOrDefault();
		}

		public YumaPos.Server.Infrastructure.DataObjects.MenuItem GetByItemVersionId(Guid itemVersionId, bool? isDeleted = false)
		{
			object parameters = new { itemVersionId, isDeleted };
			var sql = SelectByQuery + WhereQueryByItemVersionId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetByItemVersionIdAsync(Guid itemVersionId, bool? isDeleted = false)
		{
			object parameters = new { itemVersionId, isDeleted };
			var sql = SelectByQuery + WhereQueryByItemVersionId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters));
			return result.FirstOrDefault();
		}


		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetByMenuCategoryId(Guid menuCategoryId, bool? isDeleted = false)
		{
			object parameters = new { menuCategoryId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuCategoryId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem>> GetByMenuCategoryIdAsync(Guid menuCategoryId, bool? isDeleted = false)
		{
			object parameters = new { menuCategoryId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuCategoryId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters));
			return result.ToList();
		}


		public Guid Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
			var res = DataAccessService.InsertObject(menuItem, InsertQuery);
			return (Guid)res;
		}
		public async Task<Guid> InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
			var res = await DataAccessService.InsertObjectAsync(menuItem, InsertQuery);
			return (Guid)res;
		}

		public void UpdateByItemId(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
			var sql = UpdateQueryBy + WhereQueryByItemId + UpdateQueryJoin + WhereQueryByItemId;
			DataAccessService.PersistObject(menuItem, sql);
		}
		public async Task UpdateByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
			var sql = UpdateQueryBy + WhereQueryByItemId + UpdateQueryJoin + WhereQueryByItemId;
			await DataAccessService.PersistObjectAsync(menuItem, sql);
		}

		/*
		public void UpdateByItemVersionId(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
		var sql = UpdateQueryBy + WhereQueryByItemVersionId + UpdateQueryJoin + WhereQueryByItemVersionId; 
		DataAccessService.PersistObject(menuItem, sql);
		}
		public async Task UpdateByItemVersionIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
		var sql = UpdateQueryBy + WhereQueryByItemVersionId + UpdateQueryJoin + WhereQueryByItemVersionId; 
		await DataAccessService.PersistObjectAsync(menuItem, sql);
		}


		*//*
		public void UpdateByMenuCategoryId(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
		var sql = UpdateQueryBy + WhereQueryByMenuCategoryId + UpdateQueryJoin + WhereQueryByMenuCategoryId; 
		DataAccessService.PersistObject(menuItem, sql);
		}
		public async Task UpdateByMenuCategoryIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
		var sql = UpdateQueryBy + WhereQueryByMenuCategoryId + UpdateQueryJoin + WhereQueryByMenuCategoryId; 
		await DataAccessService.PersistObjectAsync(menuItem, sql);
		}


		*/
		public void RemoveByItemId(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
			var sql = SelectIntoTempTable + WhereQueryByItemId + DeleteQueryBy;
			DataAccessService.PersistObject(menuItem, sql);
		}
		public async Task RemoveByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
			var sql = SelectIntoTempTable + WhereQueryByItemId + DeleteQueryBy;
			await DataAccessService.PersistObjectAsync(menuItem, sql);
		}

		public void RemoveByItemId(Guid itemId)
		{
			object parameters = new { itemId };
			var sql = SelectIntoTempTable + WhereQueryByItemId + DeleteQueryBy;
			DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
		}
		public async Task RemoveByItemIdAsync(Guid itemId)
		{
			object parameters = new { itemId };
			var sql = SelectIntoTempTable + WhereQueryByItemId + DeleteQueryBy;
			await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
		}

		/*
		public void RemoveByItemVersionId(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
		var sql = SelectIntoTempTable + WhereQueryByItemVersionId + DeleteQueryBy; 
		DataAccessService.PersistObject(menuItem, sql);
		}
		public async Task RemoveByItemVersionIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
		var sql = SelectIntoTempTable + WhereQueryByItemVersionId + DeleteQueryBy; 
		await DataAccessService.PersistObjectAsync(menuItem, sql);
		}

		public void RemoveByItemVersionId(Guid itemVersionId)
		{
		object parameters = new {itemVersionId};
		var sql = SelectIntoTempTable + WhereQueryByItemVersionId + DeleteQueryBy; 
		DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
		}
		public async Task RemoveByItemVersionIdAsync(Guid itemVersionId)
		{
		object parameters = new {itemVersionId};
		var sql = SelectIntoTempTable + WhereQueryByItemVersionId + DeleteQueryBy; 
		await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
		}


		*//*
		public void RemoveByMenuCategoryId(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
		var sql = SelectIntoTempTable + WhereQueryByMenuCategoryId + DeleteQueryBy; 
		DataAccessService.PersistObject(menuItem, sql);
		}
		public async Task RemoveByMenuCategoryIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
		var sql = SelectIntoTempTable + WhereQueryByMenuCategoryId + DeleteQueryBy; 
		await DataAccessService.PersistObjectAsync(menuItem, sql);
		}

		public void RemoveByMenuCategoryId(Guid menuCategoryId)
		{
		object parameters = new {menuCategoryId};
		var sql = SelectIntoTempTable + WhereQueryByMenuCategoryId + DeleteQueryBy; 
		DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
		}
		public async Task RemoveByMenuCategoryIdAsync(Guid menuCategoryId)
		{
		object parameters = new {menuCategoryId};
		var sql = SelectIntoTempTable + WhereQueryByMenuCategoryId + DeleteQueryBy; 
		await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
		}


		*/

	}
}
