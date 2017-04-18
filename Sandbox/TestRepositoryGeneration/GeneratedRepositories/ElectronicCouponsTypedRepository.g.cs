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
		private const string UpdateQueryBy = @"UPDATE [ElectronicCouponsTyped] SET [ElectronicCouponsTyped].[ElectronicCouponsId] = @ElectronicCouponsId,[ElectronicCouponsTyped].[ElectronicCouponsPresetId] = @ElectronicCouponsPresetId,[ElectronicCouponsTyped].[IsPromotionalCampaign] = @IsPromotionalCampaign FROM [ElectronicCouponsTyped] ";
		private const string DeleteQueryBy = @"UPDATE [ElectronicCoupons] SET IsDeleted = 1 ";
		private const string UpdateQueryJoin = "UPDATE [ElectronicCoupons] SET [ElectronicCoupons].[Name] = @Name,[ElectronicCoupons].[PrintText] = @PrintText,[ElectronicCoupons].[ImageId] = @ImageId,[ElectronicCoupons].[ValidFrom] = @ValidFrom,[ElectronicCoupons].[ValidTo] = @ValidTo,[ElectronicCoupons].[IsDeleted] = @IsDeleted,[ElectronicCoupons].[LimitPerOrder] = @LimitPerOrder,[ElectronicCoupons].[Priority] = @Priority,[ElectronicCoupons].[MaxTimesPerCustomer] = @MaxTimesPerCustomer,[ElectronicCoupons].[IsActive] = @IsActive FROM [ElectronicCoupons] ";
		private const string SelectIntoTempTable = @"DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [ElectronicCouponsTyped].[ElectronicCouponsId] FROM [ElectronicCouponsTyped] ";
		private const string WhereQueryByElectronicCouponsId = "WHERE [ElectronicCouponsTyped].[ElectronicCouponsId] = @ElectronicCouponsId{andTenantId:[ElectronicCouponsTyped]} ";
		private const string WhereQueryByJoinPk = "WHERE [ElectronicCoupons].[Id] = @Id{andTenantId:[ElectronicCoupons]} ";
		private const string AndWithIsDeletedFilter = "AND [ElectronicCoupons].[IsDeleted] = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE [ElectronicCouponsTyped].[IsDeleted] = @IsDeleted{andTenantId:[ElectronicCouponsTyped]} ";


		public ElectronicCouponsTypedRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService) : base(dataAccessService) { }
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
		public int Insert(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped electronicCouponsTyped)
		{
		var res = DataAccessService.InsertObject(electronicCouponsTyped,InsertQuery);
		return (int)res;
		}
		public async Task<int> InsertAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.ElectronicCouponsTyped electronicCouponsTyped)
		{
		var res = await DataAccessService.InsertObjectAsync(electronicCouponsTyped,InsertQuery);
		return (int)res;
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

	}
}
