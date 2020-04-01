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


namespace TestRepositoryGeneration.CustomRepositories.BaseRepositories
{
	public partial class ElectronicCouponRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase, TestRepositoryGeneration.RepositoryInterfaces.IElectronicCouponRepository
	{
		public const string Fields = @"[ElectronicCoupons].[Id],[ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive]";
		private const string SelectAllQuery = @"SELECT [ElectronicCoupons].[Id],[ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive] FROM [ElectronicCoupons]  {whereTenantId:[ElectronicCoupons]} ";
		private const string SelectByQuery = @"SELECT [ElectronicCoupons].[Id],[ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive] FROM [ElectronicCoupons] ";
		private const string InsertQuery = @"INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive],[ElectronicCoupons].[TenantId]) OUTPUT INSERTED.Id VALUES(@Name,@PrintText,@ImageId,@ValidFrom,@ValidTo,@IsDeleted,@LimitPerOrder,@Priority,@MaxTimesPerCustomer,@IsActive,@TenantId) ";
		private const string UpdateQueryBy = @"UPDATE [ElectronicCoupons] SET [ElectronicCoupons].[Name] = @Name,[ElectronicCoupons].[PrintText] = @PrintText,[ElectronicCoupons].[ImageId] = @ImageId,[ElectronicCoupons].[ValidFrom] = @ValidFrom,[ElectronicCoupons].[ValidTo] = @ValidTo,[ElectronicCoupons].[IsDeleted] = @IsDeleted,[ElectronicCoupons].[LimitPerOrder] = @LimitPerOrder,[ElectronicCoupons].[Priority] = @Priority,[ElectronicCoupons].[MaxTimesPerCustomer] = @MaxTimesPerCustomer,[ElectronicCoupons].[IsActive] = @IsActive FROM [ElectronicCoupons] ";
		private const string DeleteQueryBy = @"UPDATE [ElectronicCoupons] SET IsDeleted = 1 ";
		private const string InsertOrUpdateQuery = @"UPDATE [ElectronicCoupons] SET [ElectronicCoupons].[Name] = @Name,[ElectronicCoupons].[PrintText] = @PrintText,[ElectronicCoupons].[ImageId] = @ImageId,[ElectronicCoupons].[ValidFrom] = @ValidFrom,[ElectronicCoupons].[ValidTo] = @ValidTo,[ElectronicCoupons].[IsDeleted] = @IsDeleted,[ElectronicCoupons].[LimitPerOrder] = @LimitPerOrder,[ElectronicCoupons].[Priority] = @Priority,[ElectronicCoupons].[MaxTimesPerCustomer] = @MaxTimesPerCustomer,[ElectronicCoupons].[IsActive] = @IsActive FROM [ElectronicCoupons]  WHERE [ElectronicCoupons].[Id] = @Id{andTenantId:[ElectronicCoupons]}  IF @@ROWCOUNT = 0 BEGIN INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive],[ElectronicCoupons].[TenantId]) OUTPUT INSERTED.Id VALUES(@Name,@PrintText,@ImageId,@ValidFrom,@ValidTo,@IsDeleted,@LimitPerOrder,@Priority,@MaxTimesPerCustomer,@IsActive,@TenantId)  END";
		private const string UpdateManyByIdQueryTemplate = @"UPDATE [ElectronicCoupons] SET Name = @Name{0},PrintText = @PrintText{0},ImageId = '{1}',ValidFrom = '{2}',ValidTo = '{3}',IsDeleted = '{4}',LimitPerOrder = '{5}',Priority = '{6}',MaxTimesPerCustomer = '{7}',IsActive = '{8}' WHERE [ElectronicCoupons].[Id] = @Id{0}{{andTenantId:[ElectronicCoupons]}}";
		private const string WhereQueryById = "WHERE [ElectronicCoupons].[Id] = @Id{andTenantId:[ElectronicCoupons]} ";
		private const string AndWithIsDeletedFilter = "AND [ElectronicCoupons].[IsDeleted] = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE [ElectronicCoupons].[IsDeleted] = @IsDeleted{andTenantId:[ElectronicCoupons]} ";
		private const string InsertManyQueryTemplate = @"INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive],[ElectronicCoupons].[TenantId]) OUTPUT INSERTED.Id VALUES {0}";
		private const string InsertManyValuesTemplate = @"('{1}',@Name{0},@PrintText{0},'{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}',@TenantId)";


		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon> GetAll(bool? isDeleted = true)
		{
			var sql = SelectAllQuery;
			object parameters = new { isDeleted };
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon>(sql, parameters).ToList();
			return result.ToList();
		}

		public async Task InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon> electronicCouponList)
		{
			if (electronicCouponList == null) throw new ArgumentException(nameof(electronicCouponList));

			if (!electronicCouponList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 3;
			var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows
																	? maxInsertManyRowsWithParameters
																	: MaxInsertManyRows;
			var values = new System.Text.StringBuilder();
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = electronicCouponList.Select((x, i) => new { Index = i, Value = x })
							.GroupBy(x => x.Index / maxInsertManyRows)
							.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
							.ToList();

			await Task.Delay(10);

			foreach (var items in itemsPerRequest)
			{
				parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				foreach (var item in items)
				{
					var electronicCoupon = item.Value;
					var index = item.Index;
					parameters.Add($"Name{index}", electronicCoupon.Name);
					parameters.Add($"PrintText{index}", electronicCoupon.PrintText);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, index, electronicCoupon.Id, electronicCoupon.ImageId?.ToString() ?? "NULL", electronicCoupon.ValidFrom?.ToString(CultureInfo.InvariantCulture) ?? "NULL", electronicCoupon.ValidTo?.ToString(CultureInfo.InvariantCulture) ?? "NULL", (electronicCoupon.IsDeleted != null ? (electronicCoupon.IsDeleted.Value ? 1 : 0).ToString() : null) ?? "NULL", electronicCoupon.LimitPerOrder?.ToString() ?? "NULL", electronicCoupon.Priority?.ToString() ?? "NULL", electronicCoupon.MaxTimesPerCustomer?.ToString() ?? "NULL", electronicCoupon.IsActive ? 1 : 0);
				}
				query.AppendFormat(InsertManyQueryTemplate, values.Replace("'NULL'", "NULL").ToString());

				await Task.Delay(10);
				await DataAccessService.ExecuteAsync(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				query.Clear();
			}

			await Task.Delay(10);

		}

		public async Task InsertManySplitByTransactionsAsync(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon> electronicCouponList)
		{
			if (electronicCouponList == null) throw new ArgumentException(nameof(electronicCouponList));

			if (!electronicCouponList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 3;
			var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows
																	? maxInsertManyRowsWithParameters
																	: MaxInsertManyRows;
			var values = new System.Text.StringBuilder();
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = electronicCouponList.Select((x, i) => new { Index = i, Value = x })
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
					var electronicCoupon = item.Value;
					var index = item.Index;
					parameters.Add($"Name{index}", electronicCoupon.Name);
					parameters.Add($"PrintText{index}", electronicCoupon.PrintText);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, index, electronicCoupon.Id, electronicCoupon.ImageId?.ToString() ?? "NULL", electronicCoupon.ValidFrom?.ToString(CultureInfo.InvariantCulture) ?? "NULL", electronicCoupon.ValidTo?.ToString(CultureInfo.InvariantCulture) ?? "NULL", (electronicCoupon.IsDeleted != null ? (electronicCoupon.IsDeleted.Value ? 1 : 0).ToString() : null) ?? "NULL", electronicCoupon.LimitPerOrder?.ToString() ?? "NULL", electronicCoupon.Priority?.ToString() ?? "NULL", electronicCoupon.MaxTimesPerCustomer?.ToString() ?? "NULL", electronicCoupon.IsActive ? 1 : 0);
				}
				query.AppendFormat(InsertManyQueryTemplate, values.Replace("'NULL'", "NULL").ToString());
				query.AppendLine("COMMIT TRANSACTION;");

				await Task.Delay(10);
				await DataAccessService.ExecuteAsync(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				query.Clear();
			}

			await Task.Delay(10);

		}
	}
}
