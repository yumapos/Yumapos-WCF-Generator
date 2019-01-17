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


namespace TestRepositoryGeneration
{
	public partial class CustomerSubscriptionArchiveRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase, TestRepositoryGeneration.RepositoryInterfaces.ICustomerSubscriptionArchiveRepository
	{
		private const string Fields = @"archive.customer_subscriptions.customer_id,archive.customer_subscriptions.customer_notifications_type,archive.customer_subscriptions.email,archive.customer_subscriptions.s_m_s,archive.customer_subscriptions.push,archive.customer_subscriptions.is_customizable,archive.customer_subscriptions.resend_period,archive.customer_subscriptions.is_deleted";
		private const string SelectAllQuery = @"SELECT archive.customer_subscriptions.customer_id,archive.customer_subscriptions.customer_notifications_type,archive.customer_subscriptions.email,archive.customer_subscriptions.s_m_s,archive.customer_subscriptions.push,archive.customer_subscriptions.is_customizable,archive.customer_subscriptions.resend_period,archive.customer_subscriptions.is_deleted FROM archive.customer_subscriptions  {whereTenantId:archive.customer_subscriptions} ";
		private const string SelectByQuery = @"SELECT archive.customer_subscriptions.customer_id,archive.customer_subscriptions.customer_notifications_type,archive.customer_subscriptions.email,archive.customer_subscriptions.s_m_s,archive.customer_subscriptions.push,archive.customer_subscriptions.is_customizable,archive.customer_subscriptions.resend_period,archive.customer_subscriptions.is_deleted FROM archive.customer_subscriptions ";
		private const string InsertQuery = @"INSERT INTO archive.customer_subscriptions(archive.customer_subscriptions.customer_id,archive.customer_subscriptions.customer_notifications_type,archive.customer_subscriptions.email,archive.customer_subscriptions.s_m_s,archive.customer_subscriptions.push,archive.customer_subscriptions.is_customizable,archive.customer_subscriptions.resend_period,archive.customer_subscriptions.is_deleted,archive.customer_subscriptions.tenant_id)  VALUES(@CustomerId,@CustomerNotificationsType,@Email,@SMS,@Push,@IsCustomizable,@ResendPeriod,@IsDeleted,@TenantId) ";
		private const string UpdateQueryBy = @"UPDATE archive.customer_subscriptions SET archive.customer_subscriptions.customer_id = @CustomerId,archive.customer_subscriptions.customer_notifications_type = @CustomerNotificationsType,archive.customer_subscriptions.email = @Email,archive.customer_subscriptions.s_m_s = @SMS,archive.customer_subscriptions.push = @Push,archive.customer_subscriptions.is_customizable = @IsCustomizable,archive.customer_subscriptions.resend_period = @ResendPeriod,archive.customer_subscriptions.is_deleted = @IsDeleted FROM archive.customer_subscriptions ";
		private const string DeleteQueryBy = @"DELETE FROM archive.customer_subscriptions ";
		private const string InsertOrUpdateQuery = @"INSERT INTO archive.customer_subscriptions(archive.customer_subscriptions.customer_id,archive.customer_subscriptions.customer_notifications_type,archive.customer_subscriptions.email,archive.customer_subscriptions.s_m_s,archive.customer_subscriptions.push,archive.customer_subscriptions.is_customizable,archive.customer_subscriptions.resend_period,archive.customer_subscriptions.is_deleted,archive.customer_subscriptions.tenant_id)  VALUES(@CustomerId,@CustomerNotificationsType,@Email,@SMS,@Push,@IsCustomizable,@ResendPeriod,@IsDeleted,@TenantId)  ON CONFLICT (customer_id,customer_notifications_type) DO UPDATE archive.customer_subscriptions SET archive.customer_subscriptions.customer_id = @CustomerId,archive.customer_subscriptions.customer_notifications_type = @CustomerNotificationsType,archive.customer_subscriptions.email = @Email,archive.customer_subscriptions.s_m_s = @SMS,archive.customer_subscriptions.push = @Push,archive.customer_subscriptions.is_customizable = @IsCustomizable,archive.customer_subscriptions.resend_period = @ResendPeriod,archive.customer_subscriptions.is_deleted = @IsDeleted ";
		private const string WhereQueryByCustomerIdAndCustomerNotificationsType = "WHERE archive.customer_subscriptions.customer_id = @CustomerId AND ((archive.customer_subscriptions.customer_notifications_type IS NULL AND @CustomerNotificationsType IS NULL) OR archive.customer_subscriptions.customer_notifications_type = @CustomerNotificationsType){andTenantId:archive.customer_subscriptions} ";
		private const string InsertManyQueryTemplate = @"InsertMany script was not generated";
		private const string InsertManyValuesTemplate = @"InsertMany script was not generated";
		private const string NoCheckConstraint = @"NoCheckConstraint script was not generated";
		private const string CheckConstraint = @"CheckConstraint script was not generated";



		public CustomerSubscriptionArchiveRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService, TestRepositoryGeneration.Infrastructure.IDataAccessController dataAccessController) : base(dataAccessService, dataAccessController) { }
		/*
		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription> GetAll()
		{
		var sql = SelectAllQuery;
		object parameters = null;
		var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>(sql, parameters).ToList();
		return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>> GetAllAsync()
		{
		var sql = SelectAllQuery;
		object parameters = null;
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>(sql, parameters));
		return result.ToList();
		}

		*/
		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription> GetByCustomerIdAndCustomerNotificationsType(string customerId, int customerNotificationsType)
		{
			object parameters = new { customerId, customerNotificationsType };
			var sql = SelectByQuery + WhereQueryByCustomerIdAndCustomerNotificationsType;
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>> GetByCustomerIdAndCustomerNotificationsTypeAsync(string customerId, int customerNotificationsType)
		{
			object parameters = new { customerId, customerNotificationsType };
			var sql = SelectByQuery + WhereQueryByCustomerIdAndCustomerNotificationsType;
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>(sql, parameters));
			return result.ToList();
		}


		/*
		public void Insert(TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription customerSubscription)
		{
		DataAccessService.InsertObject(customerSubscription,InsertQuery);
		}
		public async Task InsertAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription customerSubscription)
		{
		await DataAccessService.InsertObjectAsync(customerSubscription,InsertQuery);
		}

		*/
		/*
		public void InsertMany(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription> customerSubscriptionList)
		{
		if(customerSubscriptionList==null) throw new ArgumentException(nameof(customerSubscriptionList));

		if(!customerSubscriptionList.Any()) return;

		var maxInsertManyRowsWithParameters = MaxRepositoryParams / 2;
		var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows 
																? maxInsertManyRowsWithParameters
																: MaxInsertManyRows;
		var values = new System.Text.StringBuilder();
		var query = new System.Text.StringBuilder();
		var parameters = new Dictionary<string, object>();

		var itemsPerRequest = customerSubscriptionList.Select((x, i) => new {Index = i,Value = x})
						.GroupBy(x => x.Index / maxInsertManyRows)
						.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
						.ToList(); 

		DataAccessService.Execute(NoCheckConstraint);

		parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
		foreach (var items in itemsPerRequest)
		{
		foreach (var item in items)
		{
		var customerSubscription = item.Value;
		var index = item.Index; 
		parameters.Add($"CustomerId{index}", customerSubscription.CustomerId);
		values.AppendLine(index != 0 ? ",":"");
		values.AppendFormat(InsertManyValuesTemplate, customerSubscription.CustomerNotificationsType?.ToString() ?? "NULL",customerSubscription.Email,customerSubscription.SMS,customerSubscription.Push,customerSubscription.IsCustomizable?.ToString() ?? "NULL",customerSubscription.ResendPeriod?.ToString() ?? "NULL",customerSubscription.IsDeleted, index);
		}
		query.AppendFormat(InsertManyQueryTemplate, values.ToString());
		DataAccessService.Execute(query.ToString(), parameters);
		}

		DataAccessService.Execute(CheckConstraint);

		}

		public async Task InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription> customerSubscriptionList)
		{
		if(customerSubscriptionList==null) throw new ArgumentException(nameof(customerSubscriptionList));

		if(!customerSubscriptionList.Any()) return;

		var maxInsertManyRowsWithParameters = MaxRepositoryParams / 2;
		var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows 
																? maxInsertManyRowsWithParameters
																: MaxInsertManyRows;
		var values = new System.Text.StringBuilder();
		var query = new System.Text.StringBuilder();
		var parameters = new Dictionary<string, object>();

		var itemsPerRequest = customerSubscriptionList.Select((x, i) => new {Index = i,Value = x})
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
		var customerSubscription = item.Value;
		var index = item.Index; 
		parameters.Add($"CustomerId{index}", customerSubscription.CustomerId);
		values.AppendLine(index != 0 ? ",":"");
		values.AppendFormat(InsertManyValuesTemplate, customerSubscription.CustomerNotificationsType?.ToString() ?? "NULL",customerSubscription.Email,customerSubscription.SMS,customerSubscription.Push,customerSubscription.IsCustomizable?.ToString() ?? "NULL",customerSubscription.ResendPeriod?.ToString() ?? "NULL",customerSubscription.IsDeleted, index);
		}
		query.AppendFormat(InsertManyQueryTemplate, values.ToString());
		await Task.Delay(10);
		await DataAccessService.ExecuteAsync(query.ToString(), parameters);
		}

		await Task.Delay(10);
		await DataAccessService.ExecuteAsync(CheckConstraint);

		}


		*/
		/*
		public void UpdateByCustomerIdAndCustomerNotificationsType(TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription customerSubscription)
		{
		var sql = UpdateQueryBy + WhereQueryByCustomerIdAndCustomerNotificationsType; 
		DataAccessService.PersistObject(customerSubscription, sql);
		}
		public async Task UpdateByCustomerIdAndCustomerNotificationsTypeAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription customerSubscription)
		{
		var sql = UpdateQueryBy + WhereQueryByCustomerIdAndCustomerNotificationsType; 
		await DataAccessService.PersistObjectAsync(customerSubscription, sql);
		}


		*/
		/*
		public void RemoveByCustomerIdAndCustomerNotificationsType(TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription customerSubscription)
		{
		var sql = DeleteQueryBy + WhereQueryByCustomerIdAndCustomerNotificationsType; 
		DataAccessService.PersistObject(customerSubscription, sql);
		}
		public async Task RemoveByCustomerIdAndCustomerNotificationsTypeAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription customerSubscription)
		{
		var sql = DeleteQueryBy + WhereQueryByCustomerIdAndCustomerNotificationsType; 
		await DataAccessService.PersistObjectAsync(customerSubscription, sql);
		}

		public void RemoveByCustomerIdAndCustomerNotificationsType(string customerId, int customerNotificationsType)
		{
		object parameters = new {customerId, customerNotificationsType};
		var sql = DeleteQueryBy + WhereQueryByCustomerIdAndCustomerNotificationsType; 
		DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>(sql, parameters);
		}
		public async Task RemoveByCustomerIdAndCustomerNotificationsTypeAsync(string customerId, int customerNotificationsType)
		{
		object parameters = new {customerId, customerNotificationsType};
		var sql = DeleteQueryBy + WhereQueryByCustomerIdAndCustomerNotificationsType; 
		await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>(sql, parameters);
		}


		*/
		public void InsertOrUpdate(TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription customerSubscription)
		{
			DataAccessService.ExecuteScalar(InsertOrUpdateQuery, customerSubscription);
		}
		public async Task InsertOrUpdateAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription customerSubscription)
		{
			await DataAccessService.ExecuteScalarAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>(InsertOrUpdateQuery, customerSubscription);
		}


	}
}
