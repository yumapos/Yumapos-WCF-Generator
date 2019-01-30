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
	internal class MenuItemVersionRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase
	{
		private const string InsertQuery = @"INSERT INTO [RecipieItemVersions]([RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[Modified],[RecipieItems].[ModifiedBy],[RecipieItems].[CategoryId],[RecipieItems].[TenantId])
VALUES (@MenuItemId,@MenuItemVersionId,@IsDeleted,@Modified,@ModifiedBy,@CategoryId,@TenantId)
INSERT INTO [MenuItemVersions]([MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[MenuCategoryId],[MenuItems].[ExternalId],[MenuItems].[DiscountValue],[MenuItems].[DiscountStartDate],[MenuItems].[Type],[MenuItems].[BitKitchenPrinters],[MenuItems].[YesNoUnknown],[MenuItems].[TenantId])
VALUES (@MenuItemId,@MenuItemVersionId,@MenuCategoryId,@ExternalId,@DiscountValue,@DiscountStartDate,@Type,@BitKitchenPrinters,@YesNoUnknown,@TenantId)";
		private const string SelectBy = @"SELECT [MenuItemVersions].[MenuItemId],[MenuItemVersions].[MenuItemVersionId],[MenuItemVersions].[MenuCategoryId],[MenuItemVersions].[ExternalId],[MenuItemVersions].[DiscountValue],[MenuItemVersions].[DiscountStartDate],[MenuItemVersions].[Type],[MenuItemVersions].[BitKitchenPrinters],[MenuItemVersions].[YesNoUnknown],[RecipieItemVersions].[ItemId],[RecipieItemVersions].[ItemVersionId],[RecipieItemVersions].[IsDeleted],[RecipieItemVersions].[Modified],[RecipieItemVersions].[ModifiedBy],[RecipieItemVersions].[CategoryId] FROM [MenuItemVersions] INNER JOIN [RecipieItemVersions] ON [MenuItemVersions].[MenuItemVersionId] = [RecipieItemVersions].[ItemVersionId]  {filter} ";
		private const string SelectByKeyAndSliceDateQuery = @"SELECT [MenuItemVersions].[MenuItemId],[MenuItemVersions].[MenuItemVersionId],[MenuItemVersions].[MenuCategoryId],[MenuItemVersions].[ExternalId],[MenuItemVersions].[DiscountValue],[MenuItemVersions].[DiscountStartDate],[MenuItemVersions].[Type],[MenuItemVersions].[BitKitchenPrinters],[MenuItemVersions].[YesNoUnknown] FROM (SELECT versionTable1.[MenuItemId], MAX(joinVersionTable1.[Modified]) as Modified FROM [MenuItemVersions] versionTable1 INNER JOIN [RecipieItems] joinVersionTable1 ON versionTable1.[MenuItemVersionId] = joinVersionTable1.[ItemVersionId] {filter}  GROUP BY versionTable1.[MenuItemId]) versionTable INNER JOIN [RecipieItemVersions] ON versionTable.[MenuItemId] = [RecipieItemVersions].[ItemId] AND versionTable.[Modified] = [RecipieItemVersions].[Modified] INNER JOIN [MenuItemVersions] ON [RecipieItemVersions].[ItemVersionId] = [MenuItemVersions].[MenuItemVersionId]";
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
		private const string InsertManyQueryTemplate = @"INSERT INTO [RecipieItemVersions]([RecipieItemVersions].[ItemId],[RecipieItemVersions].[ItemVersionId],[RecipieItemVersions].[IsDeleted],[RecipieItemVersions].[Modified],[RecipieItemVersions].[ModifiedBy],[RecipieItemVersions].[CategoryId],[RecipieItemVersions].[TenantId])  VALUES {0};INSERT INTO [MenuItemVersions]([MenuItemVersions].[MenuItemId],[MenuItemVersions].[MenuItemVersionId],[MenuItemVersions].[MenuCategoryId],[MenuItemVersions].[ExternalId],[MenuItemVersions].[DiscountValue],[MenuItemVersions].[DiscountStartDate],[MenuItemVersions].[Type],[MenuItemVersions].[BitKitchenPrinters],[MenuItemVersions].[YesNoUnknown],[MenuItemVersions].[TenantId])  VALUES {1}";
		private const string InsertManyValuesTemplate = @"('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',@TenantId)";
		private const string InsertManyJoinedValuesTemplate = @"('{0}','{1}','{2}','{3}','{4}',@CategoryId{5},@TenantId)";


		public MenuItemVersionRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService, TestRepositoryGeneration.Infrastructure.IDataAccessController dataAccessController) : base(dataAccessService, dataAccessController) { }
		public void Insert(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			DataAccessService.InsertObject(menuItem, InsertQuery);
		}
		public async Task InsertAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			await DataAccessService.InsertObjectAsync(menuItem, InsertQuery);
		}

		public void InsertMany(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> menuItemList)
		{
			if (menuItemList == null) throw new ArgumentException(nameof(menuItemList));

			if (!menuItemList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 3;
			var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows
																	? maxInsertManyRowsWithParameters
																	: MaxInsertManyRows;
			var values = new System.Text.StringBuilder();
			var joinedValues = new System.Text.StringBuilder();
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = menuItemList.Select((x, i) => new { Index = i, Value = x })
							.GroupBy(x => x.Index / maxInsertManyRows)
							.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
							.ToList();


			foreach (var items in itemsPerRequest)
			{
				parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				foreach (var item in items)
				{
					var menuItem = item.Value;
					var index = item.Index;
					parameters.Add($"CategoryId{index}", menuItem.CategoryId);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, menuItem.MenuItemId, menuItem.MenuItemVersionId, menuItem.MenuCategoryId, menuItem.ExternalId?.ToString() ?? "NULL", menuItem.DiscountValue?.ToString(CultureInfo.InvariantCulture) ?? "NULL", menuItem.DiscountStartDate?.ToString(CultureInfo.InvariantCulture) ?? "NULL", (int)menuItem.Type, ((int?)menuItem.BitKitchenPrinters)?.ToString() ?? "NULL", (menuItem.YesNoUnknown != null ? (menuItem.YesNoUnknown.Value ? 1 : 0).ToString() : null) ?? "NULL", index);
					joinedValues.AppendLine(index != 0 ? "," : "");
					joinedValues.AppendFormat(InsertManyJoinedValuesTemplate, menuItem.ItemId, menuItem.ItemVersionId, menuItem.IsDeleted ? 1 : 0, menuItem.Modified.ToString(CultureInfo.InvariantCulture), menuItem.ModifiedBy, index);
				}
				query.AppendFormat(InsertManyQueryTemplate, joinedValues.Replace("'NULL'", "NULL").ToString(), values.Replace("'NULL'", "NULL").ToString());
				DataAccessService.Execute(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				joinedValues.Clear();
				query.Clear();
			}


		}

		public async Task InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> menuItemList)
		{
			if (menuItemList == null) throw new ArgumentException(nameof(menuItemList));

			if (!menuItemList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 3;
			var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows
																	? maxInsertManyRowsWithParameters
																	: MaxInsertManyRows;
			var values = new System.Text.StringBuilder();
			var joinedValues = new System.Text.StringBuilder();
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = menuItemList.Select((x, i) => new { Index = i, Value = x })
							.GroupBy(x => x.Index / maxInsertManyRows)
							.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
							.ToList();

			await Task.Delay(10);

			foreach (var items in itemsPerRequest)
			{
				parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				foreach (var item in items)
				{
					var menuItem = item.Value;
					var index = item.Index;
					parameters.Add($"CategoryId{index}", menuItem.CategoryId);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, menuItem.MenuItemId, menuItem.MenuItemVersionId, menuItem.MenuCategoryId, menuItem.ExternalId?.ToString() ?? "NULL", menuItem.DiscountValue?.ToString(CultureInfo.InvariantCulture) ?? "NULL", menuItem.DiscountStartDate?.ToString(CultureInfo.InvariantCulture) ?? "NULL", (int)menuItem.Type, ((int?)menuItem.BitKitchenPrinters)?.ToString() ?? "NULL", (menuItem.YesNoUnknown != null ? (menuItem.YesNoUnknown.Value ? 1 : 0).ToString() : null) ?? "NULL", index);
					joinedValues.AppendLine(index != 0 ? "," : "");
					joinedValues.AppendFormat(InsertManyJoinedValuesTemplate, menuItem.ItemId, menuItem.ItemVersionId, menuItem.IsDeleted ? 1 : 0, menuItem.Modified.ToString(CultureInfo.InvariantCulture), menuItem.ModifiedBy, index);
				}
				query.AppendFormat(InsertManyQueryTemplate, joinedValues.Replace("'NULL'", "NULL").ToString(), values.Replace("'NULL'", "NULL").ToString());
				await Task.Delay(10);
				await DataAccessService.ExecuteAsync(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				joinedValues.Clear();
				query.Clear();
			}

			await Task.Delay(10);

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
