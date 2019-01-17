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


namespace TestRepositoryGeneration.CustomRepositories.BaseRepositories
{
	public partial class ElectronicCouponRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase, TestRepositoryGeneration.RepositoryInterfaces.IElectronicCouponRepository
	{
		private const string Fields = @"[ElectronicCoupons].[Id],[ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive]";
		private const string SelectAllQuery = @"SELECT [ElectronicCoupons].[Id],[ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive] FROM [ElectronicCoupons]  {whereTenantId:[ElectronicCoupons]} ";
		private const string SelectByQuery = @"SELECT [ElectronicCoupons].[Id],[ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive] FROM [ElectronicCoupons] ";
		private const string InsertQuery = @"INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive],[ElectronicCoupons].[TenantId]) OUTPUT INSERTED.Id VALUES(@Name,@PrintText,@ImageId,@ValidFrom,@ValidTo,@IsDeleted,@LimitPerOrder,@Priority,@MaxTimesPerCustomer,@IsActive,@TenantId) ";
		private const string UpdateQueryBy = @"UPDATE [ElectronicCoupons] SET [ElectronicCoupons].[Name] = @Name,[ElectronicCoupons].[PrintText] = @PrintText,[ElectronicCoupons].[ImageId] = @ImageId,[ElectronicCoupons].[ValidFrom] = @ValidFrom,[ElectronicCoupons].[ValidTo] = @ValidTo,[ElectronicCoupons].[IsDeleted] = @IsDeleted,[ElectronicCoupons].[LimitPerOrder] = @LimitPerOrder,[ElectronicCoupons].[Priority] = @Priority,[ElectronicCoupons].[MaxTimesPerCustomer] = @MaxTimesPerCustomer,[ElectronicCoupons].[IsActive] = @IsActive FROM [ElectronicCoupons] ";
		private const string DeleteQueryBy = @"UPDATE [ElectronicCoupons] SET IsDeleted = 1 ";
		private const string InsertOrUpdateQuery = @"UPDATE [ElectronicCoupons] SET [ElectronicCoupons].[Name] = @Name,[ElectronicCoupons].[PrintText] = @PrintText,[ElectronicCoupons].[ImageId] = @ImageId,[ElectronicCoupons].[ValidFrom] = @ValidFrom,[ElectronicCoupons].[ValidTo] = @ValidTo,[ElectronicCoupons].[IsDeleted] = @IsDeleted,[ElectronicCoupons].[LimitPerOrder] = @LimitPerOrder,[ElectronicCoupons].[Priority] = @Priority,[ElectronicCoupons].[MaxTimesPerCustomer] = @MaxTimesPerCustomer,[ElectronicCoupons].[IsActive] = @IsActive FROM [ElectronicCoupons]  WHERE [ElectronicCoupons].[Id] = @Id{andTenantId:[ElectronicCoupons]}  IF @@ROWCOUNT = 0 BEGIN INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive],[ElectronicCoupons].[TenantId]) OUTPUT INSERTED.Id VALUES(@Name,@PrintText,@ImageId,@ValidFrom,@ValidTo,@IsDeleted,@LimitPerOrder,@Priority,@MaxTimesPerCustomer,@IsActive,@TenantId)  END";
		private const string WhereQueryById = "WHERE [ElectronicCoupons].[Id] = @Id{andTenantId:[ElectronicCoupons]} ";
		private const string AndWithIsDeletedFilter = "AND [ElectronicCoupons].[IsDeleted] = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE [ElectronicCoupons].[IsDeleted] = @IsDeleted{andTenantId:[ElectronicCoupons]} ";
		private const string InsertManyQueryTemplate = @"INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive],[ElectronicCoupons].[TenantId]) OUTPUT INSERTED.Id VALUES {0}";
		private const string InsertManyValuesTemplate = @"({0},@Name{9},@PrintText{9},{1},{2},{3},{4},{5},{6},{7},{8},@TenantId)";
		private const string NoCheckConstraint = @"ALTER TABLE [ElectronicCoupons] NOCHECK CONSTRAINT ALL";
		private const string CheckConstraint = @"ALTER TABLE [ElectronicCoupons] CHECK CONSTRAINT ALL";



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
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon>> GetAllAsync(bool? isDeleted = true)
		{
			var sql = SelectAllQuery;
			object parameters = new { isDeleted };
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon>(sql, parameters));
			return result.ToList();
		}

		/*
		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon> GetById(int id, bool? isDeleted = true)
		{
		object parameters = new {id, isDeleted};
		var sql = SelectByQuery + WhereQueryById;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithIsDeletedFilter;
		}
		var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon>(sql, parameters);
		return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon>> GetByIdAsync(int id, bool? isDeleted = true)
		{
		object parameters = new {id, isDeleted};
		var sql = SelectByQuery + WhereQueryById;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithIsDeletedFilter;
		}
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon>(sql, parameters));
		return result.ToList();
		}


		*/
		/*
		public int Insert(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon electronicCoupon)
		{
		var res = DataAccessService.InsertObject(electronicCoupon,InsertQuery);
		return (int)res;
		}
		public async Task<int> InsertAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon electronicCoupon)
		{
		var res = await DataAccessService.InsertObjectAsync(electronicCoupon,InsertQuery);
		return (int)res;
		}

		*/
		public void InsertMany(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon> electronicCouponList)
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

			DataAccessService.Execute(NoCheckConstraint);

			parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
			foreach (var items in itemsPerRequest)
			{
				foreach (var item in items)
				{
					var electronicCoupon = item.Value;
					var index = item.Index;
					parameters.Add($"Name{index}", electronicCoupon.Name);
					parameters.Add($"PrintText{index}", electronicCoupon.PrintText);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, electronicCoupon.Id, electronicCoupon.ImageId != null ? $"'{electronicCoupon.ImageId}'" : "NULL", electronicCoupon.ValidFrom != null ? $"'{electronicCoupon.ValidFrom}'" : "NULL", electronicCoupon.ValidTo != null ? $"'{electronicCoupon.ValidTo}'" : "NULL", electronicCoupon.IsDeleted?.ToString() ?? "NULL", electronicCoupon.LimitPerOrder?.ToString() ?? "NULL", electronicCoupon.Priority?.ToString() ?? "NULL", electronicCoupon.MaxTimesPerCustomer?.ToString() ?? "NULL", electronicCoupon.IsActive, index);
				}
				query.AppendFormat(InsertManyQueryTemplate, values.ToString());
				DataAccessService.Execute(query.ToString(), parameters);
			}

			DataAccessService.Execute(CheckConstraint);

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
			await DataAccessService.ExecuteAsync(NoCheckConstraint);

			parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
			foreach (var items in itemsPerRequest)
			{
				foreach (var item in items)
				{
					var electronicCoupon = item.Value;
					var index = item.Index;
					parameters.Add($"Name{index}", electronicCoupon.Name);
					parameters.Add($"PrintText{index}", electronicCoupon.PrintText);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, electronicCoupon.Id, electronicCoupon.ImageId != null ? $"'{electronicCoupon.ImageId}'" : "NULL", electronicCoupon.ValidFrom != null ? $"'{electronicCoupon.ValidFrom}'" : "NULL", electronicCoupon.ValidTo != null ? $"'{electronicCoupon.ValidTo}'" : "NULL", electronicCoupon.IsDeleted?.ToString() ?? "NULL", electronicCoupon.LimitPerOrder?.ToString() ?? "NULL", electronicCoupon.Priority?.ToString() ?? "NULL", electronicCoupon.MaxTimesPerCustomer?.ToString() ?? "NULL", electronicCoupon.IsActive, index);
				}
				query.AppendFormat(InsertManyQueryTemplate, values.ToString());
				await Task.Delay(10);
				await DataAccessService.ExecuteAsync(query.ToString(), parameters);
			}

			await Task.Delay(10);
			await DataAccessService.ExecuteAsync(CheckConstraint);

		}


		/*
		public void UpdateById(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon electronicCoupon)
		{
		var sql = UpdateQueryBy + WhereQueryById; 
		DataAccessService.PersistObject(electronicCoupon, sql);
		}
		public async Task UpdateByIdAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon electronicCoupon)
		{
		var sql = UpdateQueryBy + WhereQueryById; 
		await DataAccessService.PersistObjectAsync(electronicCoupon, sql);
		}


		*/
		/*
		public void RemoveById(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon electronicCoupon)
		{
		var sql = DeleteQueryBy + WhereQueryById; 
		DataAccessService.PersistObject(electronicCoupon, sql);
		}
		public async Task RemoveByIdAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon electronicCoupon)
		{
		var sql = DeleteQueryBy + WhereQueryById; 
		await DataAccessService.PersistObjectAsync(electronicCoupon, sql);
		}

		public void RemoveById(int id)
		{
		object parameters = new {id};
		var sql = DeleteQueryBy + WhereQueryById; 
		DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon>(sql, parameters);
		}
		public async Task RemoveByIdAsync(int id)
		{
		object parameters = new {id};
		var sql = DeleteQueryBy + WhereQueryById; 
		await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon>(sql, parameters);
		}


		*/
		/*
		public void InsertOrUpdate(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon electronicCoupon)
		{
		DataAccessService.ExecuteScalar(InsertOrUpdateQuery,electronicCoupon);
		}
		public async Task InsertOrUpdateAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon electronicCoupon)
		{
		await DataAccessService.ExecuteScalarAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon >(InsertOrUpdateQuery,electronicCoupon);
		}

		*/

	}
}
