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
		private const string InsertQuery = @"INSERT INTO [RecipieItemVersions]([RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[Modified],[RecipieItems].[ModifiedBy],[RecipieItems].[CategoryId],[RecipieItems].[TenantId])
VALUES (@MenuItemId,@MenuItemVersionId,@IsDeleted,@Modified,@ModifiedBy,@CategoryId,@TenantId)
INSERT INTO [MenuItemVersions]([MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[MenuCategoryId],[MenuItems].[TenantId])
VALUES (@MenuItemId,@MenuItemVersionId,@MenuCategoryId,@TenantId)";
		private const string SelectBy = @"SELECT [MenuItemVersions].[MenuItemId],[MenuItemVersions].[MenuItemVersionId],[MenuItemVersions].[MenuCategoryId],[RecipieItemVersions].[ItemId],[RecipieItemVersions].[ItemVersionId],[RecipieItemVersions].[IsDeleted],[RecipieItemVersions].[Modified],[RecipieItemVersions].[ModifiedBy],[RecipieItemVersions].[CategoryId] FROM [MenuItemVersions] INNER JOIN [RecipieItemVersions] ON [MenuItemVersions].[MenuItemId] = [RecipieItemVersions].[ItemId]  {filter} ";
		private const string SelectByKeyAndSliceDateQuery = @"SELECT [MenuItemVersions].[MenuItemId],[MenuItemVersions].[MenuItemVersionId],[MenuItemVersions].[MenuCategoryId] FROM (SELECT versionTable1.[MenuItemId], MAX(joinVersionTable1.[Modified]) as Modified FROM [MenuItemVersions] versionTable1 INNER JOIN [RecipieItems] joinVersionTable1 ON versionTable1.[MenuItemVersionId] = joinVersionTable1.[ItemVersionId] {filter}  GROUP BY versionTable1.[MenuItemId]) versionTable INNER JOIN [RecipieItemVersions] ON versionTable.[MenuItemId] = [RecipieItemVersions].[ItemId] AND versionTable.[Modified] = [RecipieItemVersions].[Modified] INNER JOIN [MenuItemVersions] ON [RecipieItemVersions].[ItemVersionId] = [MenuItemVersions].[MenuItemVersionId]";
		private const string WhereQueryByMenuItemId = "WHERE [MenuItemVersions].[MenuItemId] = @MenuItemId{andTenantId:[MenuItemVersions]} ";
		private const string WhereQueryByWithAliasMenuItemId = "WHERE versionTable1.[MenuItemId] = @MenuItemId{andTenantId:versionTable1} ";
		private const string WhereQueryByMenuItemVersionId = "WHERE [MenuItemVersions].[MenuItemVersionId] = @MenuItemVersionId{andTenantId:[MenuItemVersions]} ";
		private const string WhereQueryByWithAliasMenuItemVersionId = "WHERE versionTable1.[MenuItemVersionId] = @MenuItemVersionId{andTenantId:versionTable1} ";
		private const string WhereQueryByMenuCategoryId = "WHERE [MenuItemVersions].[MenuCategoryId] = @MenuCategoryId{andTenantId:[MenuItemVersions]} ";
		private const string WhereQueryByWithAliasMenuCategoryId = "WHERE versionTable1.[MenuCategoryId] = @MenuCategoryId{andTenantId:versionTable1} ";
		private const string WhereQueryByJoinPk = "WHERE [RecipieItemVersions].[ItemId] = @ItemId{andTenantId:[RecipieItemVersions]} ";
		private const string AndWithIsDeletedFilter = "AND [RecipieItemVersions].[IsDeleted] = @IsDeleted ";
		private const string AndWithIsDeletedFilterWithAlias = "AND joinVersionTable1.[IsDeleted] = @IsDeleted ";
		private const string AndWithSliceDateFilter = "AND joinVersionTable1.[Modified] <= @Modified ";

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
