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
using TestRepositoryGeneration.RepositoryInterfaces;


namespace TestRepositoryGeneration.CustomRepositories.VersionsRepositories
{
	internal partial class MenuItemCacheRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase
	{
		public const string Fields = @"[MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[MenuCategoryId],[MenuItems].[ExternalId],[MenuItems].[DiscountValue],[MenuItems].[DiscountStartDate],[MenuItems].[Type],[MenuItems].[BitKitchenPrinters],[MenuItems].[YesNoUnknown],[MenuItems].[CreatedBy]";
		private const string SelectAllQuery = @"SELECT [MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[MenuCategoryId],[MenuItems].[ExternalId],[MenuItems].[DiscountValue],[MenuItems].[DiscountStartDate],[MenuItems].[Type],[MenuItems].[BitKitchenPrinters],[MenuItems].[YesNoUnknown],[MenuItems].[CreatedBy],[RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[Modified],[RecipieItems].[ModifiedBy],[RecipieItems].[CategoryId] FROM [MenuItems] INNER JOIN [RecipieItems] ON [MenuItems].[MenuItemId] = [RecipieItems].[ItemId]  {whereTenantId:[MenuItems]} ";
		private const string SelectByQuery = @"SELECT [MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[MenuCategoryId],[MenuItems].[ExternalId],[MenuItems].[DiscountValue],[MenuItems].[DiscountStartDate],[MenuItems].[Type],[MenuItems].[BitKitchenPrinters],[MenuItems].[YesNoUnknown],[MenuItems].[CreatedBy],[RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[Modified],[RecipieItems].[ModifiedBy],[RecipieItems].[CategoryId] FROM [MenuItems] INNER JOIN [RecipieItems] ON [MenuItems].[MenuItemId] = [RecipieItems].[ItemId] ";
		private const string InsertQuery = @"DECLARE @TempTable TABLE (ItemId uniqueidentifier);INSERT INTO [RecipieItems]([RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[Modified],[RecipieItems].[ModifiedBy],[RecipieItems].[CategoryId],[RecipieItems].[TenantId]) OUTPUT INSERTED.ItemId INTO @TempTable VALUES(@MenuItemId,@MenuItemVersionId,@IsDeleted,@Modified,@ModifiedBy,@CategoryId,@TenantId);DECLARE @TempId uniqueidentifier; SELECT @TempId = ItemId FROM @TempTable;INSERT INTO [MenuItems]([MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[MenuCategoryId],[MenuItems].[ExternalId],[MenuItems].[DiscountValue],[MenuItems].[DiscountStartDate],[MenuItems].[Type],[MenuItems].[BitKitchenPrinters],[MenuItems].[YesNoUnknown],[MenuItems].[CreatedBy],[MenuItems].[TenantId]) OUTPUT INSERTED.MenuItemId INTO @TempTable VALUES(@TempId,@MenuItemVersionId,@MenuCategoryId,@ExternalId,@DiscountValue,@DiscountStartDate,@Type,@BitKitchenPrinters,@YesNoUnknown,@CreatedBy,@TenantId);SELECT ItemId FROM @TempTable;";
		private const string UpdateQueryBy = @"UPDATE [MenuItems] SET [MenuItems].[MenuItemVersionId] = @MenuItemVersionId,[MenuItems].[MenuCategoryId] = @MenuCategoryId,[MenuItems].[ExternalId] = @ExternalId,[MenuItems].[DiscountValue] = @DiscountValue,[MenuItems].[DiscountStartDate] = @DiscountStartDate,[MenuItems].[Type] = @Type,[MenuItems].[BitKitchenPrinters] = @BitKitchenPrinters,[MenuItems].[YesNoUnknown] = @YesNoUnknown FROM [MenuItems] ";
		private const string DeleteQueryBy = @"DELETE FROM [MenuItems] WHERE [MenuItems].[MenuItemId] IN (SELECT ItemId FROM @Temp);DELETE FROM [RecipieItems] WHERE [RecipieItems].[ItemId] IN (SELECT ItemId FROM @Temp); ";
		private const string InsertOrUpdateQuery = @"UPDATE [MenuItems] SET [MenuItems].[MenuItemVersionId] = @MenuItemVersionId,[MenuItems].[MenuCategoryId] = @MenuCategoryId,[MenuItems].[ExternalId] = @ExternalId,[MenuItems].[DiscountValue] = @DiscountValue,[MenuItems].[DiscountStartDate] = @DiscountStartDate,[MenuItems].[Type] = @Type,[MenuItems].[BitKitchenPrinters] = @BitKitchenPrinters,[MenuItems].[YesNoUnknown] = @YesNoUnknown FROM [MenuItems]  WHERE [MenuItems].[MenuItemId] = @MenuItemId{andTenantId:[MenuItems]}  IF @@ROWCOUNT = 0 BEGIN DECLARE @TempTable TABLE (ItemId uniqueidentifier);INSERT INTO [RecipieItems]([RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[Modified],[RecipieItems].[ModifiedBy],[RecipieItems].[CategoryId],[RecipieItems].[TenantId]) OUTPUT INSERTED.ItemId INTO @TempTable VALUES(@MenuItemId,@MenuItemVersionId,@IsDeleted,@Modified,@ModifiedBy,@CategoryId,@TenantId);DECLARE @TempId uniqueidentifier; SELECT @TempId = ItemId FROM @TempTable;INSERT INTO [MenuItems]([MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[MenuCategoryId],[MenuItems].[ExternalId],[MenuItems].[DiscountValue],[MenuItems].[DiscountStartDate],[MenuItems].[Type],[MenuItems].[BitKitchenPrinters],[MenuItems].[YesNoUnknown],[MenuItems].[CreatedBy],[MenuItems].[TenantId]) OUTPUT INSERTED.MenuItemId INTO @TempTable VALUES(@TempId,@MenuItemVersionId,@MenuCategoryId,@ExternalId,@DiscountValue,@DiscountStartDate,@Type,@BitKitchenPrinters,@YesNoUnknown,@CreatedBy,@TenantId);SELECT ItemId FROM @TempTable; END";
		private const string UpdateManyByMenuItemIdQueryTemplate = @"UPDATE [MenuItems] SET MenuItemVersionId = '{1}',MenuCategoryId = '{2}',ExternalId = '{3}',DiscountValue = '{4}',DiscountStartDate = '{5}',Type = '{6}',BitKitchenPrinters = '{7}',YesNoUnknown = '{8}' WHERE [MenuItems].[MenuItemId] = @MenuItemId{0}{{andTenantId:[MenuItems]}}";
		private const string UpdateQueryJoin = "UPDATE [RecipieItems] SET [RecipieItems].[ItemId] = @MenuItemId,[RecipieItems].[ItemVersionId] = @MenuItemVersionId,[RecipieItems].[IsDeleted] = @IsDeleted,[RecipieItems].[Modified] = @Modified,[RecipieItems].[ModifiedBy] = @ModifiedBy,[RecipieItems].[CategoryId] = @CategoryId FROM [RecipieItems] ";
		private const string UpdateManyByItemIdJoinedQueryTemplate = @"UPDATE [RecipieItems] SET ItemId = '{1}',ItemVersionId = '{2}',IsDeleted = '{3}',Modified = '{4}',ModifiedBy = '{5}',CategoryId = @CategoryId{0} WHERE [MenuItems].[ItemId] = @ItemId{0}{{andTenantId:[MenuItems]}}";
		private const string SelectIntoTempTable = @"DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [MenuItems].[MenuItemId] FROM [MenuItems] ";
		private const string WhereQueryByMenuItemId = "WHERE [MenuItems].[MenuItemId] = @MenuItemId{andTenantId:[MenuItems]} ";
		private const string WhereQueryByMenuItemVersionId = "WHERE [MenuItems].[MenuItemVersionId] = @MenuItemVersionId{andTenantId:[MenuItems]} ";
		private const string WhereQueryByMenuCategoryId = "WHERE [MenuItems].[MenuCategoryId] = @MenuCategoryId{andTenantId:[MenuItems]} ";
		private const string WhereQueryByJoinPk = "WHERE [RecipieItems].[ItemId] = @ItemId{andTenantId:[RecipieItems]} ";
		private const string AndWithIsDeletedFilter = "AND [RecipieItems].[IsDeleted] = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE [MenuItems].[IsDeleted] = @IsDeleted{andTenantId:[MenuItems]} ";
		private const string InsertManyQueryTemplate = @"INSERT INTO [RecipieItems]([RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[Modified],[RecipieItems].[ModifiedBy],[RecipieItems].[CategoryId],[RecipieItems].[TenantId])  VALUES {0};INSERT INTO [MenuItems]([MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[MenuCategoryId],[MenuItems].[ExternalId],[MenuItems].[DiscountValue],[MenuItems].[DiscountStartDate],[MenuItems].[Type],[MenuItems].[BitKitchenPrinters],[MenuItems].[YesNoUnknown],[MenuItems].[CreatedBy],[MenuItems].[TenantId])  VALUES {1}";
		private const string InsertManyValuesTemplate = @"('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',@CreatedBy{0},@TenantId)";
		private const string InsertManyJoinedValuesTemplate = @"('{1}','{2}','{3}','{4}','{5}',@CategoryId{0},@TenantId)";

		public MenuItemCacheRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService, TestRepositoryGeneration.Infrastructure.IDataAccessController dataAccessController) : base(dataAccessService, dataAccessController) { }

		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetAll(bool? isDeleted = false)
		{
			var sql = SelectAllQuery;
			object parameters = new { isDeleted };
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters).ToList();
			return result.ToList();
		}

		public TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem GetByMenuItemId(System.Guid menuItemId, bool? isDeleted = false)
		{
			object parameters = new { menuItemId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuItemId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters);
			return result.FirstOrDefault();
		}

		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> GetByMenuCategoryId(System.Guid menuCategoryId, bool? isDeleted = false)
		{
			object parameters = new { menuCategoryId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuCategoryId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters);
			return result.ToList();
		}

		public TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem GetByMenuItemVersionId(System.Guid menuItemVersionId, bool? isDeleted = false)
		{
			object parameters = new { menuItemVersionId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuItemVersionId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters);
			return result.FirstOrDefault();
		}

		public void Insert(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			DataAccessService.InsertObject(menuItem, InsertQuery);
		}

		public void InsertMany(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> menuItemList)
		{
			if (menuItemList == null) throw new ArgumentException(nameof(menuItemList));

			if (!menuItemList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 4;
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
					parameters.Add($"CreatedBy{index}", menuItem.CreatedBy);
					parameters.Add($"CategoryId{index}", menuItem.CategoryId);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, index, menuItem.MenuItemId, menuItem.MenuItemVersionId, menuItem.MenuCategoryId, menuItem.ExternalId?.ToString() ?? "NULL", menuItem.DiscountValue?.ToString(CultureInfo.InvariantCulture) ?? "NULL", menuItem.DiscountStartDate?.ToString(CultureInfo.InvariantCulture) ?? "NULL", (int)menuItem.Type, ((int?)menuItem.BitKitchenPrinters)?.ToString() ?? "NULL", (menuItem.YesNoUnknown != null ? (menuItem.YesNoUnknown.Value ? 1 : 0).ToString() : null) ?? "NULL");
					joinedValues.AppendLine(index != 0 ? "," : "");
					joinedValues.AppendFormat(InsertManyJoinedValuesTemplate, index, menuItem.ItemId, menuItem.ItemVersionId, menuItem.IsDeleted ? 1 : 0, menuItem.Modified.ToString(CultureInfo.InvariantCulture), menuItem.ModifiedBy);
				}
				query.AppendFormat(InsertManyQueryTemplate, joinedValues.Replace("'NULL'", "NULL").ToString(), values.Replace("'NULL'", "NULL").ToString());
				DataAccessService.Execute(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				joinedValues.Clear();
				query.Clear();
			}

		}

		public void InsertManySplitByTransactions(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> menuItemList)
		{
			if (menuItemList == null) throw new ArgumentException(nameof(menuItemList));

			if (!menuItemList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 4;
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
				query.AppendLine("BEGIN TRANSACTION;");
				parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				foreach (var item in items)
				{
					var menuItem = item.Value;
					var index = item.Index;
					parameters.Add($"CreatedBy{index}", menuItem.CreatedBy);
					parameters.Add($"CategoryId{index}", menuItem.CategoryId);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, index, menuItem.MenuItemId, menuItem.MenuItemVersionId, menuItem.MenuCategoryId, menuItem.ExternalId?.ToString() ?? "NULL", menuItem.DiscountValue?.ToString(CultureInfo.InvariantCulture) ?? "NULL", menuItem.DiscountStartDate?.ToString(CultureInfo.InvariantCulture) ?? "NULL", (int)menuItem.Type, ((int?)menuItem.BitKitchenPrinters)?.ToString() ?? "NULL", (menuItem.YesNoUnknown != null ? (menuItem.YesNoUnknown.Value ? 1 : 0).ToString() : null) ?? "NULL");
					joinedValues.AppendLine(index != 0 ? "," : "");
					joinedValues.AppendFormat(InsertManyJoinedValuesTemplate, index, menuItem.ItemId, menuItem.ItemVersionId, menuItem.IsDeleted ? 1 : 0, menuItem.Modified.ToString(CultureInfo.InvariantCulture), menuItem.ModifiedBy);
				}
				query.AppendFormat(InsertManyQueryTemplate, joinedValues.Replace("'NULL'", "NULL").ToString(), values.Replace("'NULL'", "NULL").ToString());
				query.AppendLine("COMMIT TRANSACTION;");
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

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 4;
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
					parameters.Add($"CreatedBy{index}", menuItem.CreatedBy);
					parameters.Add($"CategoryId{index}", menuItem.CategoryId);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, index, menuItem.MenuItemId, menuItem.MenuItemVersionId, menuItem.MenuCategoryId, menuItem.ExternalId?.ToString() ?? "NULL", menuItem.DiscountValue?.ToString(CultureInfo.InvariantCulture) ?? "NULL", menuItem.DiscountStartDate?.ToString(CultureInfo.InvariantCulture) ?? "NULL", (int)menuItem.Type, ((int?)menuItem.BitKitchenPrinters)?.ToString() ?? "NULL", (menuItem.YesNoUnknown != null ? (menuItem.YesNoUnknown.Value ? 1 : 0).ToString() : null) ?? "NULL");
					joinedValues.AppendLine(index != 0 ? "," : "");
					joinedValues.AppendFormat(InsertManyJoinedValuesTemplate, index, menuItem.ItemId, menuItem.ItemVersionId, menuItem.IsDeleted ? 1 : 0, menuItem.Modified.ToString(CultureInfo.InvariantCulture), menuItem.ModifiedBy);
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

		public async Task InsertManySplitByTransactionsAsync(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> menuItemList)
		{
			if (menuItemList == null) throw new ArgumentException(nameof(menuItemList));

			if (!menuItemList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 4;
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
				query.AppendLine("BEGIN TRANSACTION;");
				parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				foreach (var item in items)
				{
					var menuItem = item.Value;
					var index = item.Index;
					parameters.Add($"CreatedBy{index}", menuItem.CreatedBy);
					parameters.Add($"CategoryId{index}", menuItem.CategoryId);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, index, menuItem.MenuItemId, menuItem.MenuItemVersionId, menuItem.MenuCategoryId, menuItem.ExternalId?.ToString() ?? "NULL", menuItem.DiscountValue?.ToString(CultureInfo.InvariantCulture) ?? "NULL", menuItem.DiscountStartDate?.ToString(CultureInfo.InvariantCulture) ?? "NULL", (int)menuItem.Type, ((int?)menuItem.BitKitchenPrinters)?.ToString() ?? "NULL", (menuItem.YesNoUnknown != null ? (menuItem.YesNoUnknown.Value ? 1 : 0).ToString() : null) ?? "NULL");
					joinedValues.AppendLine(index != 0 ? "," : "");
					joinedValues.AppendFormat(InsertManyJoinedValuesTemplate, index, menuItem.ItemId, menuItem.ItemVersionId, menuItem.IsDeleted ? 1 : 0, menuItem.Modified.ToString(CultureInfo.InvariantCulture), menuItem.ModifiedBy);
				}
				query.AppendFormat(InsertManyQueryTemplate, joinedValues.Replace("'NULL'", "NULL").ToString(), values.Replace("'NULL'", "NULL").ToString());
				query.AppendLine("COMMIT TRANSACTION;");

				await Task.Delay(10);
				await DataAccessService.ExecuteAsync(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				joinedValues.Clear();
				query.Clear();
			}

			await Task.Delay(10);

		}

		public void UpdateByMenuItemId(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			var sql = UpdateQueryBy + WhereQueryByMenuItemId + UpdateQueryJoin + WhereQueryByJoinPk;
			DataAccessService.PersistObject(menuItem, sql);
		}

		public async Task UpdateManyByMenuItemIdSplitByTransactionsAsync(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem> menuItemList)
		{
			if (menuItemList == null) throw new ArgumentException(nameof(menuItemList));

			if (!menuItemList.Any()) return;

			var maxUpdateManyRowsWithParameters = MaxRepositoryParams / 4;
			var maxUpdateManyRows = maxUpdateManyRowsWithParameters < MaxUpdateManyRows
																	? maxUpdateManyRowsWithParameters
																	: MaxUpdateManyRows;
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = menuItemList.Select((x, i) => new { Index = i, Value = x })
							.GroupBy(x => x.Index / maxUpdateManyRows)
							.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
							.ToList();

			await Task.Delay(10);

			foreach (var items in itemsPerRequest)
			{
				query.AppendLine("BEGIN TRANSACTION");
				parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				foreach (var item in items)
				{
					var menuItem = item.Value;
					var index = item.Index;
					parameters.Add($"MenuItemId{index}", menuItem.MenuItemId);
					parameters.Add($"CreatedBy{index}", menuItem.CreatedBy);
					parameters.Add($"CategoryId{index}", menuItem.CategoryId);
					query.AppendFormat($"{UpdateManyByMenuItemIdQueryTemplate};", index, menuItem.MenuItemVersionId, menuItem.MenuCategoryId, menuItem.ExternalId?.ToString() ?? "NULL", menuItem.DiscountValue?.ToString(CultureInfo.InvariantCulture) ?? "NULL", menuItem.DiscountStartDate?.ToString(CultureInfo.InvariantCulture) ?? "NULL", (int)menuItem.Type, ((int?)menuItem.BitKitchenPrinters)?.ToString() ?? "NULL", (menuItem.YesNoUnknown != null ? (menuItem.YesNoUnknown.Value ? 1 : 0).ToString() : null) ?? "NULL");
					query.AppendFormat($"{UpdateManyByItemIdJoinedQueryTemplate};", index, menuItem.ItemId, menuItem.ItemVersionId, menuItem.IsDeleted ? 1 : 0, menuItem.Modified.ToString(CultureInfo.InvariantCulture), menuItem.ModifiedBy);
				}
				query.AppendLine("COMMIT TRANSACTION");
				await Task.Delay(10);
				var fullSqlStatement = DataAccessService.GenerateFullSqlStatement(query.ToString().Replace("'NULL'", "NULL"), typeof(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem));
				await DataAccessService.ExecuteAsync(fullSqlStatement.ToString(), parameters);
				parameters.Clear();
				query.Clear();
			}

			await Task.Delay(10);

		}

		public void RemoveByMenuItemId(TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem menuItem)
		{
			var sql = SelectIntoTempTable + WhereQueryByMenuItemId + DeleteQueryBy;
			DataAccessService.PersistObject(menuItem, sql);
		}

		public void RemoveByMenuItemId(System.Guid menuItemId)
		{
			object parameters = new { menuItemId };
			var sql = SelectIntoTempTable + WhereQueryByMenuItemId + DeleteQueryBy;
			DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.VersionsRepositories.MenuItem>(sql, parameters);
		}
	}
}
