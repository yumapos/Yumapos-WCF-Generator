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
	public partial class ElectronicCouponsTypedRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase, TestRepositoryGeneration.RepositoryInterfaces.IElectronicCouponsTypedRepository
	{
		private const string Fields = @"[ElectronicCouponsTyped].[ElectronicCouponsId],[ElectronicCouponsTyped].[ElectronicCouponsPresetId],[ElectronicCouponsTyped].[IsPromotionalCampaign]";
		private const string SelectAllQuery = @"SELECT [ElectronicCouponsTyped].[ElectronicCouponsId],[ElectronicCouponsTyped].[ElectronicCouponsPresetId],[ElectronicCouponsTyped].[IsPromotionalCampaign],[ElectronicCoupons].[Id],[ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive] FROM [ElectronicCouponsTyped] INNER JOIN [ElectronicCoupons] ON [ElectronicCouponsTyped].[ElectronicCouponsId] = [ElectronicCoupons].[Id]  {whereTenantId:[ElectronicCouponsTyped]} ";
		private const string SelectByQuery = @"SELECT [ElectronicCouponsTyped].[ElectronicCouponsId],[ElectronicCouponsTyped].[ElectronicCouponsPresetId],[ElectronicCouponsTyped].[IsPromotionalCampaign],[ElectronicCoupons].[Id],[ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive] FROM [ElectronicCouponsTyped] INNER JOIN [ElectronicCoupons] ON [ElectronicCouponsTyped].[ElectronicCouponsId] = [ElectronicCoupons].[Id] ";
		private const string InsertQuery = @"DECLARE @TempTable TABLE (Id int);INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive],[ElectronicCoupons].[TenantId]) OUTPUT INSERTED.Id INTO @TempTable VALUES(@Name,@PrintText,@ImageId,@ValidFrom,@ValidTo,@IsDeleted,@LimitPerOrder,@Priority,@MaxTimesPerCustomer,@IsActive,@TenantId);DECLARE @TempId int; SELECT @TempId = Id FROM @TempTable;INSERT INTO [ElectronicCouponsTyped]([ElectronicCouponsTyped].[ElectronicCouponsId],[ElectronicCouponsTyped].[ElectronicCouponsPresetId],[ElectronicCouponsTyped].[IsPromotionalCampaign],[ElectronicCouponsTyped].[TenantId]) OUTPUT INSERTED.ElectronicCouponsId INTO @TempTable VALUES(@TempId,@ElectronicCouponsPresetId,@IsPromotionalCampaign,@TenantId);SELECT Id FROM @TempTable;";
		private const string InsertManyQuery = @"DECLARE @TempTable TABLE (Id int);INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive]) OUTPUT INSERTED.Id INTO @TempTable VALUES(@Name{0},@PrintText{0},@ImageId{0},@ValidFrom{0},@ValidTo{0},@IsDeleted{0},@LimitPerOrder{0},@Priority{0},@MaxTimesPerCustomer{0},@IsActive{0},@TenantId);DECLARE @TempId int; SELECT @TempId = Id FROM @TempTable;INSERT INTO [ElectronicCouponsTyped]([ElectronicCouponsTyped].[ElectronicCouponsId],[ElectronicCouponsTyped].[ElectronicCouponsPresetId],[ElectronicCouponsTyped].[IsPromotionalCampaign]) OUTPUT INSERTED.ElectronicCouponsId INTO @TempTable VALUES(@ElectronicCouponsId{0},@ElectronicCouponsPresetId{0},@IsPromotionalCampaign{0},@TenantId);SELECT Id FROM @TempTable;";
		private const string UpdateQueryBy = @"UPDATE [ElectronicCouponsTyped] SET [ElectronicCouponsTyped].[ElectronicCouponsId] = @ElectronicCouponsId,[ElectronicCouponsTyped].[ElectronicCouponsPresetId] = @ElectronicCouponsPresetId,[ElectronicCouponsTyped].[IsPromotionalCampaign] = @IsPromotionalCampaign FROM [ElectronicCouponsTyped] ";
		private const string DeleteQueryBy = @"UPDATE [ElectronicCoupons] SET IsDeleted = 1 ";
		private const string InsertOrUpdateQuery = @"UPDATE [ElectronicCouponsTyped] SET [ElectronicCouponsTyped].[ElectronicCouponsId] = @ElectronicCouponsId,[ElectronicCouponsTyped].[ElectronicCouponsPresetId] = @ElectronicCouponsPresetId,[ElectronicCouponsTyped].[IsPromotionalCampaign] = @IsPromotionalCampaign FROM [ElectronicCouponsTyped]  WHERE [ElectronicCouponsTyped].[ElectronicCouponsId] = @ElectronicCouponsId{andTenantId:[ElectronicCouponsTyped]}  IF @@ROWCOUNT = 0 BEGIN DECLARE @TempTable TABLE (Id int);INSERT INTO [ElectronicCoupons]([ElectronicCoupons].[Name],[ElectronicCoupons].[PrintText],[ElectronicCoupons].[ImageId],[ElectronicCoupons].[ValidFrom],[ElectronicCoupons].[ValidTo],[ElectronicCoupons].[IsDeleted],[ElectronicCoupons].[LimitPerOrder],[ElectronicCoupons].[Priority],[ElectronicCoupons].[MaxTimesPerCustomer],[ElectronicCoupons].[IsActive],[ElectronicCoupons].[TenantId]) OUTPUT INSERTED.Id INTO @TempTable VALUES(@Name,@PrintText,@ImageId,@ValidFrom,@ValidTo,@IsDeleted,@LimitPerOrder,@Priority,@MaxTimesPerCustomer,@IsActive,@TenantId);DECLARE @TempId int; SELECT @TempId = Id FROM @TempTable;INSERT INTO [ElectronicCouponsTyped]([ElectronicCouponsTyped].[ElectronicCouponsId],[ElectronicCouponsTyped].[ElectronicCouponsPresetId],[ElectronicCouponsTyped].[IsPromotionalCampaign],[ElectronicCouponsTyped].[TenantId]) OUTPUT INSERTED.ElectronicCouponsId INTO @TempTable VALUES(@TempId,@ElectronicCouponsPresetId,@IsPromotionalCampaign,@TenantId);SELECT Id FROM @TempTable; END";
		private const string UpdateQueryJoin = "UPDATE [ElectronicCoupons] SET [ElectronicCoupons].[Name] = @Name,[ElectronicCoupons].[PrintText] = @PrintText,[ElectronicCoupons].[ImageId] = @ImageId,[ElectronicCoupons].[ValidFrom] = @ValidFrom,[ElectronicCoupons].[ValidTo] = @ValidTo,[ElectronicCoupons].[IsDeleted] = @IsDeleted,[ElectronicCoupons].[LimitPerOrder] = @LimitPerOrder,[ElectronicCoupons].[Priority] = @Priority,[ElectronicCoupons].[MaxTimesPerCustomer] = @MaxTimesPerCustomer,[ElectronicCoupons].[IsActive] = @IsActive FROM [ElectronicCoupons] ";
		private const string SelectIntoTempTable = @"DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [ElectronicCouponsTyped].[ElectronicCouponsId] FROM [ElectronicCouponsTyped] ";
		private const string WhereQueryByElectronicCouponsId = "WHERE [ElectronicCouponsTyped].[ElectronicCouponsId] = @ElectronicCouponsId{andTenantId:[ElectronicCouponsTyped]} ";
		private const string WhereQueryByJoinPk = "WHERE [ElectronicCoupons].[Id] = @Id{andTenantId:[ElectronicCoupons]} ";
		private const string AndWithIsDeletedFilter = "AND [ElectronicCoupons].[IsDeleted] = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE [ElectronicCouponsTyped].[IsDeleted] = @IsDeleted{andTenantId:[ElectronicCouponsTyped]} ";


		public ElectronicCouponsTypedRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService, TestRepositoryGeneration.Infrastructure.IDataAccessController dataAccessController) : base(dataAccessService, dataAccessController) { }
		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped> GetAll(bool? isDeleted = false)
		{
			var sql = SelectAllQuery;
			object parameters = new { isDeleted };
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped>(sql, parameters).ToList();
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped>> GetAllAsync(bool? isDeleted = false)
		{
			var sql = SelectAllQuery;
			object parameters = new { isDeleted };
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped>(sql, parameters));
			return result.ToList();
		}

		/*
		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped> GetByElectronicCouponsId(int electronicCouponsId, bool? isDeleted = false)
		{
		object parameters = new {electronicCouponsId, isDeleted};
		var sql = SelectByQuery + WhereQueryByElectronicCouponsId;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithIsDeletedFilter;
		}
		var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped>(sql, parameters);
		return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped>> GetByElectronicCouponsIdAsync(int electronicCouponsId, bool? isDeleted = false)
		{
		object parameters = new {electronicCouponsId, isDeleted};
		var sql = SelectByQuery + WhereQueryByElectronicCouponsId;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithIsDeletedFilter;
		}
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped>(sql, parameters));
		return result.ToList();
		}


		*/
		/*
		public void Insert(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped electronicCouponsTyped)
		{
		DataAccessService.InsertObject(electronicCouponsTyped,InsertQuery);
		}
		public async Task InsertAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped electronicCouponsTyped)
		{
		await DataAccessService.InsertObjectAsync(electronicCouponsTyped,InsertQuery);
		}

		*/
		/*
		public void InsertMany(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped> electronicCouponsTypedList)
		{
		if(electronicCouponsTypedList==null) throw new ArgumentException(nameof(electronicCouponsTypedList));

		if(!electronicCouponsTypedList.Any()) return;

		var query = new System.Text.StringBuilder();
		var counter = 0;
		var parameters = new Dictionary<string, object> ();
		foreach (var electronicCouponsTyped in electronicCouponsTypedList)
		{
		if (parameters.Count + 4 > MaxRepositoryParams)
		{
		DataAccessService.Execute(query.ToString(), parameters);
		query.Clear();
		counter = 0;
		parameters.Clear();
		}
		parameters.Add($"ElectronicCouponsId{counter}", electronicCouponsTyped.ElectronicCouponsId);
		parameters.Add($"ElectronicCouponsPresetId{counter}", electronicCouponsTyped.ElectronicCouponsPresetId);
		parameters.Add($"IsPromotionalCampaign{counter}", electronicCouponsTyped.IsPromotionalCampaign);
		parameters.Add($"Id{counter}", electronicCouponsTyped.Id);
		parameters.Add($"Name{counter}", electronicCouponsTyped.Name);
		parameters.Add($"PrintText{counter}", electronicCouponsTyped.PrintText);
		parameters.Add($"ImageId{counter}", electronicCouponsTyped.ImageId);
		parameters.Add($"ValidFrom{counter}", electronicCouponsTyped.ValidFrom);
		parameters.Add($"ValidTo{counter}", electronicCouponsTyped.ValidTo);
		parameters.Add($"IsDeleted{counter}", electronicCouponsTyped.IsDeleted);
		parameters.Add($"LimitPerOrder{counter}", electronicCouponsTyped.LimitPerOrder);
		parameters.Add($"Priority{counter}", electronicCouponsTyped.Priority);
		parameters.Add($"MaxTimesPerCustomer{counter}", electronicCouponsTyped.MaxTimesPerCustomer);
		parameters.Add($"IsActive{counter}", electronicCouponsTyped.IsActive);
		parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
		query.AppendFormat(InsertManyQuery, counter);
		counter++;
		}
		DataAccessService.Execute(query.ToString(), parameters);
		}

		public async Task InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped> electronicCouponsTypedList)
		{
		if(electronicCouponsTypedList==null) throw new ArgumentException(nameof(electronicCouponsTypedList));

		if(!electronicCouponsTypedList.Any()) return;

		var query = new System.Text.StringBuilder();
		var counter = 0;
		var parameters = new Dictionary<string, object>();
		parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
		foreach (var electronicCouponsTyped in electronicCouponsTypedList)
		{
		if (parameters.Count + 4 > MaxRepositoryParams)
		{
		await DataAccessService.ExecuteAsync(query.ToString(), parameters);
		query.Clear();
		counter = 0;
		parameters.Clear();
		}
		parameters.Add($"ElectronicCouponsId{counter}", electronicCouponsTyped.ElectronicCouponsId);
		parameters.Add($"ElectronicCouponsPresetId{counter}", electronicCouponsTyped.ElectronicCouponsPresetId);
		parameters.Add($"IsPromotionalCampaign{counter}", electronicCouponsTyped.IsPromotionalCampaign);
		parameters.Add($"Id{counter}", electronicCouponsTyped.Id);
		parameters.Add($"Name{counter}", electronicCouponsTyped.Name);
		parameters.Add($"PrintText{counter}", electronicCouponsTyped.PrintText);
		parameters.Add($"ImageId{counter}", electronicCouponsTyped.ImageId);
		parameters.Add($"ValidFrom{counter}", electronicCouponsTyped.ValidFrom);
		parameters.Add($"ValidTo{counter}", electronicCouponsTyped.ValidTo);
		parameters.Add($"IsDeleted{counter}", electronicCouponsTyped.IsDeleted);
		parameters.Add($"LimitPerOrder{counter}", electronicCouponsTyped.LimitPerOrder);
		parameters.Add($"Priority{counter}", electronicCouponsTyped.Priority);
		parameters.Add($"MaxTimesPerCustomer{counter}", electronicCouponsTyped.MaxTimesPerCustomer);
		parameters.Add($"IsActive{counter}", electronicCouponsTyped.IsActive);
		parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
		query.AppendFormat(InsertManyQuery, counter);
		counter++;
		}
		await DataAccessService.ExecuteAsync(query.ToString(), parameters);
		}

		*/
		/*
		public void UpdateByElectronicCouponsId(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped electronicCouponsTyped)
		{
		var sql = UpdateQueryBy + WhereQueryByElectronicCouponsId + UpdateQueryJoin + WhereQueryByJoinPk; 
		DataAccessService.PersistObject(electronicCouponsTyped, sql);
		}
		public async Task UpdateByElectronicCouponsIdAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped electronicCouponsTyped)
		{
		var sql = UpdateQueryBy + WhereQueryByElectronicCouponsId + UpdateQueryJoin + WhereQueryByJoinPk; 
		await DataAccessService.PersistObjectAsync(electronicCouponsTyped, sql);
		}


		*/
		/*
		public void RemoveByElectronicCouponsId(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped electronicCouponsTyped)
		{
		var sql = SelectIntoTempTable + WhereQueryByElectronicCouponsId + DeleteQueryBy; 
		DataAccessService.PersistObject(electronicCouponsTyped, sql);
		}
		public async Task RemoveByElectronicCouponsIdAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped electronicCouponsTyped)
		{
		var sql = SelectIntoTempTable + WhereQueryByElectronicCouponsId + DeleteQueryBy; 
		await DataAccessService.PersistObjectAsync(electronicCouponsTyped, sql);
		}

		public void RemoveByElectronicCouponsId(int electronicCouponsId)
		{
		object parameters = new {electronicCouponsId};
		var sql = SelectIntoTempTable + WhereQueryByElectronicCouponsId + DeleteQueryBy; 
		DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped>(sql, parameters);
		}
		public async Task RemoveByElectronicCouponsIdAsync(int electronicCouponsId)
		{
		object parameters = new {electronicCouponsId};
		var sql = SelectIntoTempTable + WhereQueryByElectronicCouponsId + DeleteQueryBy; 
		await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped>(sql, parameters);
		}


		*/
		/*
		public void InsertOrUpdate(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped electronicCouponsTyped)
		{
		DataAccessService.ExecuteScalar(InsertOrUpdateQuery,electronicCouponsTyped);
		}
		public async Task InsertOrUpdateAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped electronicCouponsTyped)
		{
		await DataAccessService.ExecuteScalarAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped >(InsertOrUpdateQuery,electronicCouponsTyped);
		}

		*/

	}
}
