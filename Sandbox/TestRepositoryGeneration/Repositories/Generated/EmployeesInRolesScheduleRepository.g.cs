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
using YumaPos.Server.Infrastructure.Repositories;
using YumaPos.Server.Data.Sql;



namespace TestRepositoryGeneration
{
	public partial class EmployeesInRolesScheduleRepository : RepositoryBase, IEmployeesInRolesScheduleRepository
	{
		private const string Fields = "[EmployeesInRolesSchedule].[ScheduleId],[EmployeesInRolesSchedule].[RoleId],[EmployeesInRolesSchedule].[UserId],[EmployeesInRolesSchedule].[StoreId],[EmployeesInRolesSchedule].[BusinessDayNumber],[EmployeesInRolesSchedule].[Start],[EmployeesInRolesSchedule].[End]{columns}";
		private const string Values = "@ScheduleId,@RoleId,@UserId,@StoreId,@BusinessDayNumber,@Start,@End{values}";
		private const string SelectAllQuery = "SELECT [EmployeesInRolesSchedule].[ScheduleId],[EmployeesInRolesSchedule].[RoleId],[EmployeesInRolesSchedule].[UserId],[EmployeesInRolesSchedule].[StoreId],[EmployeesInRolesSchedule].[BusinessDayNumber],[EmployeesInRolesSchedule].[Start],[EmployeesInRolesSchedule].[End] FROM [EmployeesInRolesSchedule]  {whereTenantId:[EmployeesInRolesSchedule]} ";
		private const string SelectByQuery = "SELECT [EmployeesInRolesSchedule].[ScheduleId],[EmployeesInRolesSchedule].[RoleId],[EmployeesInRolesSchedule].[UserId],[EmployeesInRolesSchedule].[StoreId],[EmployeesInRolesSchedule].[BusinessDayNumber],[EmployeesInRolesSchedule].[Start],[EmployeesInRolesSchedule].[End] FROM [EmployeesInRolesSchedule] ";
		private const string InsertQuery = "INSERT INTO EmployeesInRolesSchedule([EmployeesInRolesSchedule].[ScheduleId],[EmployeesInRolesSchedule].[RoleId],[EmployeesInRolesSchedule].[UserId],[EmployeesInRolesSchedule].[StoreId],[EmployeesInRolesSchedule].[BusinessDayNumber],[EmployeesInRolesSchedule].[Start],[EmployeesInRolesSchedule].[End]{columns}) OUTPUT INSERTED.ScheduleId VALUES(@ScheduleId,@RoleId,@UserId,@StoreId,@BusinessDayNumber,@Start,@End{values}) ";
		private const string UpdateQueryBy = "UPDATE [EmployeesInRolesSchedule] SET EmployeesInRolesSchedule.[ScheduleId] = @ScheduleId,EmployeesInRolesSchedule.[RoleId] = @RoleId,EmployeesInRolesSchedule.[UserId] = @UserId,EmployeesInRolesSchedule.[StoreId] = @StoreId,EmployeesInRolesSchedule.[BusinessDayNumber] = @BusinessDayNumber,EmployeesInRolesSchedule.[Start] = @Start,EmployeesInRolesSchedule.[End] = @End FROM [EmployeesInRolesSchedule] ";
		private const string DeleteQueryBy = "DELETE FROM [EmployeesInRolesSchedule] ";
		private const string SelectIntoTempTable = "DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [EmployeesInRolesSchedule].[ScheduleId] FROM [EmployeesInRolesSchedule] ";
		private const string WhereQueryByScheduleId = "WHERE EmployeesInRolesSchedule.[ScheduleId] = @ScheduleId{andTenantId:[EmployeesInRolesSchedule]} ";
		private const string WhereQueryByUserId = "WHERE EmployeesInRolesSchedule.[UserId] = @UserId{andTenantId:[EmployeesInRolesSchedule]} ";
		private const string WhereQueryByUserIdAndRoleIdAndStoreId = "WHERE EmployeesInRolesSchedule.[UserId] = @UserId AND EmployeesInRolesSchedule.[RoleId] = @RoleId AND EmployeesInRolesSchedule.[StoreId] = @StoreId{andTenantId:[EmployeesInRolesSchedule]} ";


		public EmployeesInRolesScheduleRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		/*
		public IEnumerable<TestRepositoryGeneration.Models.EmployeesInRolesSchedule> GetAll()
		{
		var sql = SelectAllQuery;
		var result = DataAccessService.Get<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, null).ToList();
		return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>> GetAllAsync()
		{
		var sql = SelectAllQuery;
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, null));
		return result.ToList();
		}

		*/
		/*
		public TestRepositoryGeneration.Models.EmployeesInRolesSchedule GetByScheduleId(Guid scheduleId)
		{
		object parameters = new {scheduleId};
		var sql = SelectByQuery + WhereQueryByScheduleId;
		var result = DataAccessService.Get<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters);
		return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.Models.EmployeesInRolesSchedule> GetByScheduleIdAsync(Guid scheduleId)
		{
		object parameters = new {scheduleId};
		var sql = SelectByQuery + WhereQueryByScheduleId;
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters));
		return result.FirstOrDefault();
		}


		*/
		/*
		public IEnumerable<TestRepositoryGeneration.Models.EmployeesInRolesSchedule> GetByUserId(Guid userId)
		{
		object parameters = new {userId};
		var sql = SelectByQuery + WhereQueryByUserId;
		var result = DataAccessService.Get<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters);
		return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>> GetByUserIdAsync(Guid userId)
		{
		object parameters = new {userId};
		var sql = SelectByQuery + WhereQueryByUserId;
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters));
		return result.ToList();
		}


		*//*
		public IEnumerable<TestRepositoryGeneration.Models.EmployeesInRolesSchedule> GetByUserIdAndRoleIdAndStoreId(Guid userId, Guid roleId, Guid storeId)
		{
		object parameters = new {userId, roleId, storeId};
		var sql = SelectByQuery + WhereQueryByUserIdAndRoleIdAndStoreId;
		var result = DataAccessService.Get<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters);
		return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>> GetByUserIdAndRoleIdAndStoreIdAsync(Guid userId, Guid roleId, Guid storeId)
		{
		object parameters = new {userId, roleId, storeId};
		var sql = SelectByQuery + WhereQueryByUserIdAndRoleIdAndStoreId;
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters));
		return result.ToList();
		}


		*/
		public Guid Insert(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
			var res = DataAccessService.InsertObject(employeesInRolesSchedule, InsertQuery);
			return (Guid)res;
		}
		public async Task<Guid> InsertAsync(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
			var res = await DataAccessService.InsertObjectAsync(employeesInRolesSchedule, InsertQuery);
			return (Guid)res;
		}

		/*
		public void UpdateByScheduleId(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = UpdateQueryBy + WhereQueryByScheduleId; 
		DataAccessService.PersistObject(employeesInRolesSchedule, sql);
		}
		public async Task UpdateByScheduleIdAsync(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = UpdateQueryBy + WhereQueryByScheduleId; 
		await DataAccessService.PersistObjectAsync(employeesInRolesSchedule, sql);
		}


		*//*
		public void UpdateByUserId(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = UpdateQueryBy + WhereQueryByUserId; 
		DataAccessService.PersistObject(employeesInRolesSchedule, sql);
		}
		public async Task UpdateByUserIdAsync(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = UpdateQueryBy + WhereQueryByUserId; 
		await DataAccessService.PersistObjectAsync(employeesInRolesSchedule, sql);
		}


		*/
		public void UpdateByUserIdAndRoleIdAndStoreId(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
			var sql = UpdateQueryBy + WhereQueryByUserIdAndRoleIdAndStoreId;
			DataAccessService.PersistObject(employeesInRolesSchedule, sql);
		}
		public async Task UpdateByUserIdAndRoleIdAndStoreIdAsync(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
			var sql = UpdateQueryBy + WhereQueryByUserIdAndRoleIdAndStoreId;
			await DataAccessService.PersistObjectAsync(employeesInRolesSchedule, sql);
		}


		/*
		public void RemoveByScheduleId(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = DeleteQueryBy + WhereQueryByScheduleId; 
		DataAccessService.PersistObject(employeesInRolesSchedule, sql);
		}
		public async Task RemoveByScheduleIdAsync(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = DeleteQueryBy + WhereQueryByScheduleId; 
		await DataAccessService.PersistObjectAsync(employeesInRolesSchedule, sql);
		}

		public void RemoveByScheduleId(Guid scheduleId)
		{
		object parameters = new {scheduleId};
		var sql = DeleteQueryBy + WhereQueryByScheduleId; 
		DataAccessService.PersistObject<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters);
		}
		public async Task RemoveByScheduleIdAsync(Guid scheduleId)
		{
		object parameters = new {scheduleId};
		var sql = DeleteQueryBy + WhereQueryByScheduleId; 
		await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters);
		}


		*//*
		public void RemoveByUserId(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = DeleteQueryBy + WhereQueryByUserId; 
		DataAccessService.PersistObject(employeesInRolesSchedule, sql);
		}
		public async Task RemoveByUserIdAsync(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = DeleteQueryBy + WhereQueryByUserId; 
		await DataAccessService.PersistObjectAsync(employeesInRolesSchedule, sql);
		}

		public void RemoveByUserId(Guid userId)
		{
		object parameters = new {userId};
		var sql = DeleteQueryBy + WhereQueryByUserId; 
		DataAccessService.PersistObject<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters);
		}
		public async Task RemoveByUserIdAsync(Guid userId)
		{
		object parameters = new {userId};
		var sql = DeleteQueryBy + WhereQueryByUserId; 
		await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters);
		}


		*//*
		public void RemoveByUserIdAndRoleIdAndStoreId(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = DeleteQueryBy + WhereQueryByUserIdAndRoleIdAndStoreId; 
		DataAccessService.PersistObject(employeesInRolesSchedule, sql);
		}
		public async Task RemoveByUserIdAndRoleIdAndStoreIdAsync(TestRepositoryGeneration.Models.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = DeleteQueryBy + WhereQueryByUserIdAndRoleIdAndStoreId; 
		await DataAccessService.PersistObjectAsync(employeesInRolesSchedule, sql);
		}

		public void RemoveByUserIdAndRoleIdAndStoreId(Guid userId, Guid roleId, Guid storeId)
		{
		object parameters = new {userId, roleId, storeId};
		var sql = DeleteQueryBy + WhereQueryByUserIdAndRoleIdAndStoreId; 
		DataAccessService.PersistObject<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters);
		}
		public async Task RemoveByUserIdAndRoleIdAndStoreIdAsync(Guid userId, Guid roleId, Guid storeId)
		{
		object parameters = new {userId, roleId, storeId};
		var sql = DeleteQueryBy + WhereQueryByUserIdAndRoleIdAndStoreId; 
		await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.Models.EmployeesInRolesSchedule>(sql, parameters);
		}


		*/

	}
}
