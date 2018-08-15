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
		private const string InsertManyQuery = @"INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive],[ElectronicCoupons].[TenantId]) OUTPUT INSERTED.Id VALUES(@Name{0},@PrintText{0},@ImageId{0},@ValidFrom{0},@ValidTo{0},@IsDeleted{0},@LimitPerOrder{0},@Priority{0},@MaxTimesPerCustomer{0},@IsActive{0},@TenantId) ";
		private const string UpdateQueryBy = @"UPDATE [ElectronicCoupons] SET [ElectronicCoupons].[Name] = @Name,[ElectronicCoupons].[PrintText] = @PrintText,[ElectronicCoupons].[ImageId] = @ImageId,[ElectronicCoupons].[ValidFrom] = @ValidFrom,[ElectronicCoupons].[ValidTo] = @ValidTo,[ElectronicCoupons].[IsDeleted] = @IsDeleted,[ElectronicCoupons].[LimitPerOrder] = @LimitPerOrder,[ElectronicCoupons].[Priority] = @Priority,[ElectronicCoupons].[MaxTimesPerCustomer] = @MaxTimesPerCustomer,[ElectronicCoupons].[IsActive] = @IsActive FROM [ElectronicCoupons] ";
		private const string DeleteQueryBy = @"UPDATE [ElectronicCoupons] SET IsDeleted = 1 ";
		private const string InsertOrUpdateQuery = @"UPDATE [ElectronicCoupons] SET [ElectronicCoupons].[Name] = @Name,[ElectronicCoupons].[PrintText] = @PrintText,[ElectronicCoupons].[ImageId] = @ImageId,[ElectronicCoupons].[ValidFrom] = @ValidFrom,[ElectronicCoupons].[ValidTo] = @ValidTo,[ElectronicCoupons].[IsDeleted] = @IsDeleted,[ElectronicCoupons].[LimitPerOrder] = @LimitPerOrder,[ElectronicCoupons].[Priority] = @Priority,[ElectronicCoupons].[MaxTimesPerCustomer] = @MaxTimesPerCustomer,[ElectronicCoupons].[IsActive] = @IsActive FROM [ElectronicCoupons]  WHERE [ElectronicCoupons].[Id] = @Id{andTenantId:[ElectronicCoupons]}  IF @@ROWCOUNT = 0 BEGIN INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive],[ElectronicCoupons].[TenantId]) OUTPUT INSERTED.Id VALUES(@Name,@PrintText,@ImageId,@ValidFrom,@ValidTo,@IsDeleted,@LimitPerOrder,@Priority,@MaxTimesPerCustomer,@IsActive,@TenantId)  END";
		private const string WhereQueryById = "WHERE [ElectronicCoupons].[Id] = @Id{andTenantId:[ElectronicCoupons]} ";
		private const string AndWithIsDeletedFilter = "AND [ElectronicCoupons].[IsDeleted] = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE [ElectronicCoupons].[IsDeleted] = @IsDeleted{andTenantId:[ElectronicCoupons]} ";


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

			var query = new System.Text.StringBuilder();
			var counter = 0;
			var parameters = new Dictionary<string, object>();
			parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
			foreach (var electronicCoupon in electronicCouponList)
			{
				if (parameters.Count + 12 > MaxRepositoryParams)
				{
					DataAccessService.Execute(query.ToString(), parameters);
					query.Clear();
					counter = 0;
					parameters.Clear();
					parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				}
				parameters.Add($"Id{counter}", electronicCoupon.Id);
				parameters.Add($"Name{counter}", electronicCoupon.Name);
				parameters.Add($"PrintText{counter}", electronicCoupon.PrintText);
				parameters.Add($"ImageId{counter}", electronicCoupon.ImageId);
				parameters.Add($"ValidFrom{counter}", electronicCoupon.ValidFrom);
				parameters.Add($"ValidTo{counter}", electronicCoupon.ValidTo);
				parameters.Add($"IsDeleted{counter}", electronicCoupon.IsDeleted);
				parameters.Add($"LimitPerOrder{counter}", electronicCoupon.LimitPerOrder);
				parameters.Add($"Priority{counter}", electronicCoupon.Priority);
				parameters.Add($"MaxTimesPerCustomer{counter}", electronicCoupon.MaxTimesPerCustomer);
				parameters.Add($"IsActive{counter}", electronicCoupon.IsActive);
				query.AppendFormat(InsertManyQuery, counter);
				counter++;
			}
			DataAccessService.Execute(query.ToString(), parameters);
		}

		public async Task InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCoupon> electronicCouponList)
		{
			if (electronicCouponList == null) throw new ArgumentException(nameof(electronicCouponList));

			if (!electronicCouponList.Any()) return;

			var query = new System.Text.StringBuilder();
			var counter = 0;
			var parameters = new Dictionary<string, object>();
			parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
			foreach (var electronicCoupon in electronicCouponList)
			{
				if (parameters.Count + 12 > MaxRepositoryParams)
				{
					await DataAccessService.ExecuteAsync(query.ToString(), parameters);
					query.Clear();
					counter = 0;
					parameters.Clear();
					parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				}
				parameters.Add($"Id{counter}", electronicCoupon.Id);
				parameters.Add($"Name{counter}", electronicCoupon.Name);
				parameters.Add($"PrintText{counter}", electronicCoupon.PrintText);
				parameters.Add($"ImageId{counter}", electronicCoupon.ImageId);
				parameters.Add($"ValidFrom{counter}", electronicCoupon.ValidFrom);
				parameters.Add($"ValidTo{counter}", electronicCoupon.ValidTo);
				parameters.Add($"IsDeleted{counter}", electronicCoupon.IsDeleted);
				parameters.Add($"LimitPerOrder{counter}", electronicCoupon.LimitPerOrder);
				parameters.Add($"Priority{counter}", electronicCoupon.Priority);
				parameters.Add($"MaxTimesPerCustomer{counter}", electronicCoupon.MaxTimesPerCustomer);
				parameters.Add($"IsActive{counter}", electronicCoupon.IsActive);
				query.AppendFormat(InsertManyQuery, counter);
				counter++;
			}
			await DataAccessService.ExecuteAsync(query.ToString(), parameters);
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
