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


namespace TestRepositoryGeneration
{
	public partial class EmployeesInRolesScheduleRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase, TestRepositoryGeneration.RepositoryInterfaces.IEmployeesInRolesScheduleRepository
	{
		public const string Fields = @"[EmployeesInRolesSchedule].[ScheduleId],[EmployeesInRolesSchedule].[RoleId],[EmployeesInRolesSchedule].[UserId],[EmployeesInRolesSchedule].[StoreId],[EmployeesInRolesSchedule].[BusinessDayNumber],[EmployeesInRolesSchedule].[Start],[EmployeesInRolesSchedule].[End],[EmployeesInRolesSchedule].[IsDeleted]";
		private const string SelectAllQuery = @"SELECT [EmployeesInRolesSchedule].[ScheduleId],[EmployeesInRolesSchedule].[RoleId],[EmployeesInRolesSchedule].[UserId],[EmployeesInRolesSchedule].[StoreId],[EmployeesInRolesSchedule].[BusinessDayNumber],[EmployeesInRolesSchedule].[Start],[EmployeesInRolesSchedule].[End],[EmployeesInRolesSchedule].[IsDeleted] FROM [EmployeesInRolesSchedule]  {whereTenantId:[EmployeesInRolesSchedule]} ";
		private const string SelectByQuery = @"SELECT [EmployeesInRolesSchedule].[ScheduleId],[EmployeesInRolesSchedule].[RoleId],[EmployeesInRolesSchedule].[UserId],[EmployeesInRolesSchedule].[StoreId],[EmployeesInRolesSchedule].[BusinessDayNumber],[EmployeesInRolesSchedule].[Start],[EmployeesInRolesSchedule].[End],[EmployeesInRolesSchedule].[IsDeleted] FROM [EmployeesInRolesSchedule] ";
		private const string InsertQuery = @"INSERT INTO [EmployeesInRolesSchedule]([EmployeesInRolesSchedule].[RoleId],[EmployeesInRolesSchedule].[UserId],[EmployeesInRolesSchedule].[StoreId],[EmployeesInRolesSchedule].[BusinessDayNumber],[EmployeesInRolesSchedule].[Start],[EmployeesInRolesSchedule].[End],[EmployeesInRolesSchedule].[IsDeleted],[EmployeesInRolesSchedule].[TenantId]) OUTPUT INSERTED.ScheduleId VALUES(@RoleId,@UserId,@StoreId,@BusinessDayNumber,@Start,@End,@IsDeleted,@TenantId) ";
		private const string UpdateQueryBy = @"UPDATE [EmployeesInRolesSchedule] SET [EmployeesInRolesSchedule].[RoleId] = @RoleId,[EmployeesInRolesSchedule].[UserId] = @UserId,[EmployeesInRolesSchedule].[StoreId] = @StoreId,[EmployeesInRolesSchedule].[BusinessDayNumber] = @BusinessDayNumber,[EmployeesInRolesSchedule].[Start] = @Start,[EmployeesInRolesSchedule].[End] = @End,[EmployeesInRolesSchedule].[IsDeleted] = @IsDeleted FROM [EmployeesInRolesSchedule] ";
		private const string DeleteQueryBy = @"UPDATE [EmployeesInRolesSchedule] SET IsDeleted = 1 ";
		private const string InsertOrUpdateQuery = @"UPDATE [EmployeesInRolesSchedule] SET [EmployeesInRolesSchedule].[RoleId] = @RoleId,[EmployeesInRolesSchedule].[UserId] = @UserId,[EmployeesInRolesSchedule].[StoreId] = @StoreId,[EmployeesInRolesSchedule].[BusinessDayNumber] = @BusinessDayNumber,[EmployeesInRolesSchedule].[Start] = @Start,[EmployeesInRolesSchedule].[End] = @End,[EmployeesInRolesSchedule].[IsDeleted] = @IsDeleted FROM [EmployeesInRolesSchedule]  WHERE [EmployeesInRolesSchedule].[ScheduleId] = @ScheduleId{andTenantId:[EmployeesInRolesSchedule]}  IF @@ROWCOUNT = 0 BEGIN INSERT INTO [EmployeesInRolesSchedule]([EmployeesInRolesSchedule].[RoleId],[EmployeesInRolesSchedule].[UserId],[EmployeesInRolesSchedule].[StoreId],[EmployeesInRolesSchedule].[BusinessDayNumber],[EmployeesInRolesSchedule].[Start],[EmployeesInRolesSchedule].[End],[EmployeesInRolesSchedule].[IsDeleted],[EmployeesInRolesSchedule].[TenantId]) OUTPUT INSERTED.ScheduleId VALUES(@RoleId,@UserId,@StoreId,@BusinessDayNumber,@Start,@End,@IsDeleted,@TenantId)  END";
		private const string UpdateManyByScheduleIdQueryTemplate = @"UPDATE [EmployeesInRolesSchedule] SET RoleId = '{1}',UserId = '{2}',StoreId = '{3}',BusinessDayNumber = '{4}',Start = '{5}',End = '{6}',IsDeleted = '{7}' WHERE [EmployeesInRolesSchedule].[ScheduleId] = @ScheduleId{0}{{andTenantId:[EmployeesInRolesSchedule]}}";
		private const string WhereQueryByScheduleId = "WHERE [EmployeesInRolesSchedule].[ScheduleId] = @ScheduleId{andTenantId:[EmployeesInRolesSchedule]} ";
		private const string AndWithIsDeletedFilter = "AND [EmployeesInRolesSchedule].[IsDeleted] = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE [EmployeesInRolesSchedule].[IsDeleted] = @IsDeleted{andTenantId:[EmployeesInRolesSchedule]} ";
		private const string InsertManyQueryTemplate = @"INSERT INTO [EmployeesInRolesSchedule]([EmployeesInRolesSchedule].[RoleId],[EmployeesInRolesSchedule].[UserId],[EmployeesInRolesSchedule].[StoreId],[EmployeesInRolesSchedule].[BusinessDayNumber],[EmployeesInRolesSchedule].[Start],[EmployeesInRolesSchedule].[End],[EmployeesInRolesSchedule].[IsDeleted],[EmployeesInRolesSchedule].[TenantId]) OUTPUT INSERTED.ScheduleId VALUES {0}";
		private const string InsertManyValuesTemplate = @"('{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}',@TenantId)";



		public EmployeesInRolesScheduleRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService, TestRepositoryGeneration.Infrastructure.IDataAccessController dataAccessController) : base(dataAccessService, dataAccessController) { }
		/*
		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule> GetAll(bool? isDeleted = false)
		{
		var sql = SelectAllQuery;
		object parameters = new {isDeleted};
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithIsDeletedFilter;
		}
		var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule>(sql, parameters).ToList();
		return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule>> GetAllAsync(bool? isDeleted = false)
		{
		var sql = SelectAllQuery;
		object parameters = new {isDeleted};
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithIsDeletedFilter;
		}
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule>(sql, parameters));
		return result.ToList();
		}

		*/
		public TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule GetByScheduleId(System.Guid scheduleId, bool? isDeleted = false)
		{
			object parameters = new { scheduleId, isDeleted };
			var sql = SelectByQuery + WhereQueryByScheduleId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule> GetByScheduleIdAsync(System.Guid scheduleId, bool? isDeleted = false)
		{
			object parameters = new { scheduleId, isDeleted };
			var sql = SelectByQuery + WhereQueryByScheduleId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule>(sql, parameters));
			return result.FirstOrDefault();
		}


		public System.Guid Insert(TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
			var res = DataAccessService.InsertObject(employeesInRolesSchedule, InsertQuery);
			return (System.Guid)res;
		}
		public async Task<System.Guid> InsertAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
			var res = await DataAccessService.InsertObjectAsync(employeesInRolesSchedule, InsertQuery);
			return (System.Guid)res;
		}

		/*
		public void InsertMany(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule> employeesInRolesScheduleList)
		{
		if(employeesInRolesScheduleList==null) throw new ArgumentException(nameof(employeesInRolesScheduleList));

		if(!employeesInRolesScheduleList.Any()) return;

		var maxInsertManyRows = MaxInsertManyRows;
		var values = new System.Text.StringBuilder();
		var query = new System.Text.StringBuilder();
		var parameters = new Dictionary<string, object>();

		var itemsPerRequest = employeesInRolesScheduleList.Select((x, i) => new {Index = i,Value = x})
						.GroupBy(x => x.Index / maxInsertManyRows)
						.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
						.ToList(); 


		foreach (var items in itemsPerRequest)
		{
		parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
		foreach (var item in items)
		{
		var employeesInRolesSchedule = item.Value;
		var index = item.Index; 
		values.AppendLine(index != 0 ? ",":"");
		values.AppendFormat(InsertManyValuesTemplate, index, employeesInRolesSchedule.ScheduleId,employeesInRolesSchedule.RoleId,employeesInRolesSchedule.UserId,employeesInRolesSchedule.StoreId,employeesInRolesSchedule.BusinessDayNumber,employeesInRolesSchedule.Start.ToString(CultureInfo.InvariantCulture),employeesInRolesSchedule.End.ToString(CultureInfo.InvariantCulture),employeesInRolesSchedule.IsDeleted ? 1 : 0);
		}
		query.AppendFormat(InsertManyQueryTemplate, values.Replace("'NULL'","NULL").ToString());
		DataAccessService.Execute(query.ToString(), parameters);
		parameters.Clear();
		values.Clear();
		query.Clear();
		}


		}

		public async Task InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule> employeesInRolesScheduleList)
		{
		if(employeesInRolesScheduleList==null) throw new ArgumentException(nameof(employeesInRolesScheduleList));

		if(!employeesInRolesScheduleList.Any()) return;

		var maxInsertManyRows = MaxInsertManyRows;
		var values = new System.Text.StringBuilder();
		var query = new System.Text.StringBuilder();
		var parameters = new Dictionary<string, object>();

		var itemsPerRequest = employeesInRolesScheduleList.Select((x, i) => new {Index = i,Value = x})
						.GroupBy(x => x.Index / maxInsertManyRows)
						.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
						.ToList(); 

		await Task.Delay(10);

		foreach (var items in itemsPerRequest)
		{
		parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
		foreach (var item in items)
		{
		var employeesInRolesSchedule = item.Value;
		var index = item.Index; 
		values.AppendLine(index != 0 ? ",":"");
		values.AppendFormat(InsertManyValuesTemplate, index, employeesInRolesSchedule.ScheduleId,employeesInRolesSchedule.RoleId,employeesInRolesSchedule.UserId,employeesInRolesSchedule.StoreId,employeesInRolesSchedule.BusinessDayNumber,employeesInRolesSchedule.Start.ToString(CultureInfo.InvariantCulture),employeesInRolesSchedule.End.ToString(CultureInfo.InvariantCulture),employeesInRolesSchedule.IsDeleted ? 1 : 0);
		}
		query.AppendFormat(InsertManyQueryTemplate, values.Replace("'NULL'","NULL").ToString());
		await Task.Delay(10);
		await DataAccessService.ExecuteAsync(query.ToString(), parameters);
		parameters.Clear();
		values.Clear();
		query.Clear();
		}

		await Task.Delay(10);

		}


		*/
		/*
		public void UpdateByScheduleId(TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = UpdateQueryBy + WhereQueryByScheduleId; 
		DataAccessService.PersistObject(employeesInRolesSchedule, sql);
		}
		public async Task UpdateByScheduleIdAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = UpdateQueryBy + WhereQueryByScheduleId; 
		await DataAccessService.PersistObjectAsync(employeesInRolesSchedule, sql);
		}


		*/
		/*

		public void UpdateManyByScheduleId(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule> employeesInRolesScheduleList)
		{
		if(employeesInRolesScheduleList==null) throw new ArgumentException(nameof(employeesInRolesScheduleList));

		if(!employeesInRolesScheduleList.Any()) return;

		var maxUpdateManyRows = MaxUpdateManyRows;
		var query = new System.Text.StringBuilder();
		var parameters = new Dictionary<string, object>();

		var itemsPerRequest = employeesInRolesScheduleList.Select((x, i) => new {Index = i,Value = x})
						.GroupBy(x => x.Index / maxUpdateManyRows)
						.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
						.ToList(); 


		foreach (var items in itemsPerRequest)
		{
		parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
		foreach (var item in items)
		{
		var employeesInRolesSchedule = item.Value;
		var index = item.Index; 
		parameters.Add($"ScheduleId{index}", employeesInRolesSchedule.ScheduleId);
		query.AppendFormat($"{UpdateManyByScheduleIdQueryTemplate};", index, employeesInRolesSchedule.RoleId,employeesInRolesSchedule.UserId,employeesInRolesSchedule.StoreId,employeesInRolesSchedule.BusinessDayNumber,employeesInRolesSchedule.Start.ToString(CultureInfo.InvariantCulture),employeesInRolesSchedule.End.ToString(CultureInfo.InvariantCulture),employeesInRolesSchedule.IsDeleted ? 1 : 0);
		}
		var fullSqlStatement = DataAccessService.GenerateFullSqlStatement(query.ToString().Replace("'NULL'", "NULL"), typeof(TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule));
		DataAccessService.Execute(fullSqlStatement.ToString(), parameters);
		parameters.Clear();
		query.Clear();
		}


		}

		public async Task UpdateManyByScheduleIdAsync(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule> employeesInRolesScheduleList)
		{
		if(employeesInRolesScheduleList==null) throw new ArgumentException(nameof(employeesInRolesScheduleList));

		if(!employeesInRolesScheduleList.Any()) return;

		var maxUpdateManyRows = MaxUpdateManyRows;
		var query = new System.Text.StringBuilder();
		var parameters = new Dictionary<string, object>();

		var itemsPerRequest = employeesInRolesScheduleList.Select((x, i) => new {Index = i,Value = x})
						.GroupBy(x => x.Index / maxUpdateManyRows)
						.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
						.ToList(); 

		await Task.Delay(10);

		foreach (var items in itemsPerRequest)
		{
		parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
		foreach (var item in items)
		{
		var employeesInRolesSchedule = item.Value;
		var index = item.Index; 
		parameters.Add($"ScheduleId{index}", employeesInRolesSchedule.ScheduleId);
		query.AppendFormat($"{UpdateManyByScheduleIdQueryTemplate};", index, employeesInRolesSchedule.RoleId,employeesInRolesSchedule.UserId,employeesInRolesSchedule.StoreId,employeesInRolesSchedule.BusinessDayNumber,employeesInRolesSchedule.Start.ToString(CultureInfo.InvariantCulture),employeesInRolesSchedule.End.ToString(CultureInfo.InvariantCulture),employeesInRolesSchedule.IsDeleted ? 1 : 0);
		}
		await Task.Delay(10);
		var fullSqlStatement = DataAccessService.GenerateFullSqlStatement(query.ToString().Replace("'NULL'", "NULL"), typeof(TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule));
		await DataAccessService.ExecuteAsync(fullSqlStatement.ToString(), parameters);
		parameters.Clear();
		query.Clear();
		}

		await Task.Delay(10);

		}

		*/
		/*
		public void RemoveByScheduleId(TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = DeleteQueryBy + WhereQueryByScheduleId; 
		DataAccessService.PersistObject(employeesInRolesSchedule, sql);
		}
		public async Task RemoveByScheduleIdAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		var sql = DeleteQueryBy + WhereQueryByScheduleId; 
		await DataAccessService.PersistObjectAsync(employeesInRolesSchedule, sql);
		}

		public void RemoveByScheduleId(System.Guid scheduleId)
		{
		object parameters = new {scheduleId};
		var sql = DeleteQueryBy + WhereQueryByScheduleId; 
		DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule>(sql, parameters);
		}
		public async Task RemoveByScheduleIdAsync(System.Guid scheduleId)
		{
		object parameters = new {scheduleId};
		var sql = DeleteQueryBy + WhereQueryByScheduleId; 
		await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule>(sql, parameters);
		}


		*/
		/*
		public void InsertOrUpdate(TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		DataAccessService.ExecuteScalar(InsertOrUpdateQuery,employeesInRolesSchedule);
		}
		public async Task InsertOrUpdateAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule employeesInRolesSchedule)
		{
		await DataAccessService.ExecuteScalarAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.EmployeesInRolesSchedule >(InsertOrUpdateQuery,employeesInRolesSchedule);
		}

		*/

	}
}
