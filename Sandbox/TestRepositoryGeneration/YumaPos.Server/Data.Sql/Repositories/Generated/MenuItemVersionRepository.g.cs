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


namespace YumaPos.Server.Data.Sql.Menu
{
	internal class MenuItemVersionRepository : RepositoryBase
	{
		private const string InsertQuery = @"INSERT INTO [RecipieItemVersions]([RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[Modified],[RecipieItems].[ModifiedBy],[RecipieItems].[CategoryId]{columns})
VALUES (@MenuItemId,@MenuItemVersionId,@IsDeleted,@Modified,@ModifiedBy,@CategoryId{values})
INSERT INTO [MenuItemVersions]([MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[MenuCategoryId]{columns})
VALUES (@MenuItemId,@MenuItemVersionId,@MenuCategoryId{values})";
		private const string SelectBy = @"SELECT  [MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[MenuCategoryId],[RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[Modified],[RecipieItems].[ModifiedBy],[RecipieItems].[CategoryId] FROM [MenuItems] INNER JOIN [RecipieItems] ON [MenuItems].[MenuItemId] = [RecipieItems].[ItemId]  {filter} ";
		private const string SelectByKeyAndSliceDateQuery = @"SELECT  [MenuItemVersions].[MenuItemId],[MenuItemVersions].[MenuItemVersionId],[MenuItemVersions].[MenuCategoryId] FROM (SELECT versionTable1.[MenuItemId], MAX(joinVersionTable1.[Modified]) as Modified FROM [MenuItemVersions] versionTable1 INNER JOIN [RecipieItems] joinVersionTable1 ON versionTable1.[MenuItemVersionId] = joinVersionTable1.[ItemVersionId] {filter}  GROUP BY versionTable1.[MenuItemId]) versionTable INNER JOIN [RecipieItemVersions] ON versionTable.[MenuItemId] = [RecipieItemVersions].[ItemId] AND versionTable.[Modified] = [RecipieItemVersions].[Modified] INNER JOIN [MenuItemVersions] ON [RecipieItemVersions].[ItemVersionId] = [MenuItemVersions].[MenuItemVersionId]";
		private const string WhereQueryByMenuItemId = "WHERE [MenuItems].[MenuItemId] = @MenuItemId{andTenantId:[MenuItems]} ";
		private const string WhereQueryByWithAliasMenuItemId = "WHERE versionTable1.[MenuItemId] = @MenuItemId{andTenantId:versionTable1} ";
		private const string WhereQueryByMenuItemVersionId = "WHERE [MenuItems].[MenuItemVersionId] = @MenuItemVersionId{andTenantId:[MenuItems]} ";
		private const string WhereQueryByWithAliasMenuItemVersionId = "WHERE versionTable1.[MenuItemVersionId] = @MenuItemVersionId{andTenantId:versionTable1} ";
		private const string WhereQueryByMenuCategoryId = "WHERE [MenuItems].[MenuCategoryId] = @MenuCategoryId{andTenantId:[MenuItems]} ";
		private const string WhereQueryByWithAliasMenuCategoryId = "WHERE versionTable1.[MenuCategoryId] = @MenuCategoryId{andTenantId:versionTable1} ";
		private const string WhereQueryByJoinPk = "WHERE [RecipieItems].[ItemId] = @ItemId{andTenantId:[RecipieItems]} ";
		private const string AndWithIsDeletedFilter = "AND [RecipieItems].[IsDeleted] = @IsDeleted ";
		private const string AndWithIsDeletedFilterWithAlias = "AND joinVersionTable1.[IsDeleted] = @IsDeleted ";
		private const string AndWithSliceDateFilter = "AND joinVersionTable1.[Modified] <= @Modified ";

		public MenuItemVersionRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public void Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
			DataAccessService.InsertObject(menuItem, InsertQuery);
		}
		public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
			await DataAccessService.InsertObjectAsync(menuItem, InsertQuery);
		}

		public YumaPos.Server.Infrastructure.DataObjects.MenuItem GetByMenuItemId(System.Guid menuItemId, DateTimeOffset modified, bool? isDeleted = false)
		{
			object parameters = new { menuItemId, modified, isDeleted };
			var filter = WhereQueryByWithAliasMenuItemId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilterWithAlias;
			}
			filter = filter + AndWithSliceDateFilter;
			var sql = SelectByKeyAndSliceDateQuery.Replace("{filter}", filter);
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetByMenuItemIdAsync(System.Guid menuItemId, DateTimeOffset modified, bool? isDeleted = false)
		{
			object parameters = new { menuItemId, modified, isDeleted };
			var filter = WhereQueryByWithAliasMenuItemId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilterWithAlias;
			}
			filter = filter + AndWithSliceDateFilter;
			var sql = SelectByKeyAndSliceDateQuery.Replace("{filter}", filter);
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters));
			return result.FirstOrDefault();
		}


		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetByMenuCategoryId(System.Guid menuCategoryId, DateTimeOffset modified, bool? isDeleted = false)
		{
			object parameters = new { menuCategoryId, modified, isDeleted };
			var filter = WhereQueryByWithAliasMenuCategoryId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilterWithAlias;
			}
			filter = filter + AndWithSliceDateFilter;
			var sql = SelectByKeyAndSliceDateQuery.Replace("{filter}", filter);
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem>> GetByMenuCategoryIdAsync(System.Guid menuCategoryId, DateTimeOffset modified, bool? isDeleted = false)
		{
			object parameters = new { menuCategoryId, modified, isDeleted };
			var filter = WhereQueryByWithAliasMenuCategoryId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilterWithAlias;
			}
			filter = filter + AndWithSliceDateFilter;
			var sql = SelectByKeyAndSliceDateQuery.Replace("{filter}", filter);
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters));
			return result.ToList();
		}


		public YumaPos.Server.Infrastructure.DataObjects.MenuItem GetByMenuItemVersionId(System.Guid menuItemVersionId, bool? isDeleted = false)
		{
			object parameters = new { menuItemVersionId, isDeleted };
			var filter = WhereQueryByMenuItemVersionId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilter;
			}
			var sql = SelectBy.Replace("{filter}", filter);
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetByMenuItemVersionIdAsync(System.Guid menuItemVersionId, bool? isDeleted = false)
		{
			object parameters = new { menuItemVersionId, isDeleted };
			var filter = WhereQueryByMenuItemVersionId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilter;
			}
			var sql = SelectBy.Replace("{filter}", filter);
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters));
			return result.FirstOrDefault();
		}



	}
}
