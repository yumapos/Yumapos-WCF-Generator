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


namespace TestRepositoryGeneration.CustomRepositories.VersionsRepositories
{
	internal class MenuItemVersionRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase
	{
		private const string InsertQuery = @"INSERT INTO RecipieItemVersions(RecipieItems.item_id,RecipieItems.item_version_id,RecipieItems.is_deleted,RecipieItems.modified,RecipieItems.modified_by,RecipieItems.category_id,RecipieItems.tenant_id)
VALUES (@MenuItemId,@MenuItemVersionId,@IsDeleted,@Modified,@ModifiedBy,@CategoryId,@TenantId)
INSERT INTO MenuItemVersions(MenuItems.menu_item_id,MenuItems.menu_item_version_id,MenuItems.menu_category_id,MenuItems.tenant_id)
VALUES (@MenuItemId,@MenuItemVersionId,@MenuCategoryId,@TenantId)";
		private const string SelectBy = @"SELECT MenuItemVersions.menu_item_id,MenuItemVersions.menu_item_version_id,MenuItemVersions.menu_category_id,RecipieItemVersions.item_id,RecipieItemVersions.item_version_id,RecipieItemVersions.is_deleted,RecipieItemVersions.modified,RecipieItemVersions.modified_by,RecipieItemVersions.category_id FROM MenuItemVersions INNER JOIN RecipieItemVersions ON MenuItemVersions.menu_item_id = RecipieItemVersions.item_id  {filter} ";
		private const string SelectByKeyAndSliceDateQuery = @"SELECT MenuItemVersions.menu_item_id,MenuItemVersions.menu_item_version_id,MenuItemVersions.menu_category_id FROM (SELECT version_table1.MenuItemId, MAX(join_version_table1.modified) as modified FROM MenuItemVersions version_table1 INNER JOIN RecipieItems join_version_table1 ON version_table1.MenuItemVersionId = join_version_table1.ItemVersionId {filter}  GROUP BY version_table1.MenuItemId) version_table INNER JOIN RecipieItemVersions ON version_table.MenuItemId = RecipieItemVersions.ItemId AND version_table.modified = RecipieItemVersions.modified INNER JOIN MenuItemVersions ON RecipieItemVersions.ItemVersionId = MenuItemVersions.MenuItemVersionId";
		private const string WhereQueryByMenuItemId = "WHERE MenuItemVersions.menu_item_id = @MenuItemId{andTenantId:MenuItemVersions} ";
		private const string WhereQueryByWithAliasMenuItemId = "WHERE version_table1.menu_item_id = @MenuItemId{andTenantId:version_table1} ";
		private const string WhereQueryByMenuItemVersionId = "WHERE MenuItemVersions.menu_item_version_id = @MenuItemVersionId{andTenantId:MenuItemVersions} ";
		private const string WhereQueryByWithAliasMenuItemVersionId = "WHERE version_table1.menu_item_version_id = @MenuItemVersionId{andTenantId:version_table1} ";
		private const string WhereQueryByMenuCategoryId = "WHERE MenuItemVersions.menu_category_id = @MenuCategoryId{andTenantId:MenuItemVersions} ";
		private const string WhereQueryByWithAliasMenuCategoryId = "WHERE version_table1.menu_category_id = @MenuCategoryId{andTenantId:version_table1} ";
		private const string WhereQueryByJoinPk = "WHERE RecipieItemVersions.item_id = @ItemId{andTenantId:RecipieItemVersions} ";
		private const string AndWithIsDeletedFilter = "AND RecipieItemVersions.is_deleted = @IsDeleted ";
		private const string AndWithIsDeletedFilterWithAlias = "AND join_version_table1.is_deleted = @IsDeleted ";
		private const string AndWithSliceDateFilter = "AND join_version_table1.modified <= @Modified ";

		public MenuItemVersionRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public void Insert(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			DataAccessService.InsertObject(menuItem, InsertQuery);
		}
		public async Task InsertAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			await DataAccessService.InsertObjectAsync(menuItem, InsertQuery);
		}

		public TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem GetByMenuItemId(System.Guid menuItemId, DateTimeOffset modified, bool? isDeleted = false)
		{
			object parameters = new { menuItemId, modified, isDeleted };
			var filter = WhereQueryByWithAliasMenuItemId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilterWithAlias;
			}
			filter = filter + AndWithSliceDateFilter;
			var sql = SelectByKeyAndSliceDateQuery.Replace("{filter}", filter);
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetByMenuItemIdAsync(System.Guid menuItemId, DateTimeOffset modified, bool? isDeleted = false)
		{
			object parameters = new { menuItemId, modified, isDeleted };
			var filter = WhereQueryByWithAliasMenuItemId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilterWithAlias;
			}
			filter = filter + AndWithSliceDateFilter;
			var sql = SelectByKeyAndSliceDateQuery.Replace("{filter}", filter);
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters));
			return result.FirstOrDefault();
		}


		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetByMenuCategoryId(System.Guid menuCategoryId, DateTimeOffset modified, bool? isDeleted = false)
		{
			object parameters = new { menuCategoryId, modified, isDeleted };
			var filter = WhereQueryByWithAliasMenuCategoryId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilterWithAlias;
			}
			filter = filter + AndWithSliceDateFilter;
			var sql = SelectByKeyAndSliceDateQuery.Replace("{filter}", filter);
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>> GetByMenuCategoryIdAsync(System.Guid menuCategoryId, DateTimeOffset modified, bool? isDeleted = false)
		{
			object parameters = new { menuCategoryId, modified, isDeleted };
			var filter = WhereQueryByWithAliasMenuCategoryId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilterWithAlias;
			}
			filter = filter + AndWithSliceDateFilter;
			var sql = SelectByKeyAndSliceDateQuery.Replace("{filter}", filter);
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters));
			return result.ToList();
		}


		public TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem GetByMenuItemVersionId(System.Guid menuItemVersionId, bool? isDeleted = false)
		{
			object parameters = new { menuItemVersionId, isDeleted };
			var filter = WhereQueryByMenuItemVersionId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilter;
			}
			var sql = SelectBy.Replace("{filter}", filter);
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetByMenuItemVersionIdAsync(System.Guid menuItemVersionId, bool? isDeleted = false)
		{
			object parameters = new { menuItemVersionId, isDeleted };
			var filter = WhereQueryByMenuItemVersionId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilter;
			}
			var sql = SelectBy.Replace("{filter}", filter);
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters));
			return result.FirstOrDefault();
		}



	}
}
