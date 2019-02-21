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
	public partial class CustomerSubscriptionRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase, TestRepositoryGeneration.RepositoryInterfaces.ICustomerSubscriptionRepository
	{
		public const string Fields = @"[CustomerSubscriptions].[CustomerId],[CustomerSubscriptions].[CustomerNotificationsType],[CustomerSubscriptions].[Email],[CustomerSubscriptions].[SMS],[CustomerSubscriptions].[Push],[CustomerSubscriptions].[IsCustomizable],[CustomerSubscriptions].[ResendPeriod],[CustomerSubscriptions].[IsDeleted]";
		private const string SelectAllQuery = @"SELECT [CustomerSubscriptions].[CustomerId],[CustomerSubscriptions].[CustomerNotificationsType],[CustomerSubscriptions].[Email],[CustomerSubscriptions].[SMS],[CustomerSubscriptions].[Push],[CustomerSubscriptions].[IsCustomizable],[CustomerSubscriptions].[ResendPeriod],[CustomerSubscriptions].[IsDeleted] FROM [CustomerSubscriptions]  {whereTenantId:[CustomerSubscriptions]} ";
		private const string SelectByQuery = @"SELECT [CustomerSubscriptions].[CustomerId],[CustomerSubscriptions].[CustomerNotificationsType],[CustomerSubscriptions].[Email],[CustomerSubscriptions].[SMS],[CustomerSubscriptions].[Push],[CustomerSubscriptions].[IsCustomizable],[CustomerSubscriptions].[ResendPeriod],[CustomerSubscriptions].[IsDeleted] FROM [CustomerSubscriptions] ";
		private const string InsertQuery = @"INSERT INTO [CustomerSubscriptions]([CustomerSubscriptions].[CustomerId],[CustomerSubscriptions].[CustomerNotificationsType],[CustomerSubscriptions].[Email],[CustomerSubscriptions].[SMS],[CustomerSubscriptions].[Push],[CustomerSubscriptions].[IsCustomizable],[CustomerSubscriptions].[ResendPeriod],[CustomerSubscriptions].[IsDeleted],[CustomerSubscriptions].[TenantId])  VALUES(@CustomerId,@CustomerNotificationsType,@Email,@SMS,@Push,@IsCustomizable,@ResendPeriod,@IsDeleted,@TenantId) ";
		private const string UpdateQueryBy = @"UPDATE [CustomerSubscriptions] SET [CustomerSubscriptions].[Email] = @Email,[CustomerSubscriptions].[SMS] = @SMS,[CustomerSubscriptions].[Push] = @Push,[CustomerSubscriptions].[IsCustomizable] = @IsCustomizable,[CustomerSubscriptions].[ResendPeriod] = @ResendPeriod,[CustomerSubscriptions].[IsDeleted] = @IsDeleted FROM [CustomerSubscriptions] ";
		private const string DeleteQueryBy = @"DELETE FROM [CustomerSubscriptions] ";
		private const string InsertOrUpdateQuery = @"UPDATE [CustomerSubscriptions] SET [CustomerSubscriptions].[Email] = @Email,[CustomerSubscriptions].[SMS] = @SMS,[CustomerSubscriptions].[Push] = @Push,[CustomerSubscriptions].[IsCustomizable] = @IsCustomizable,[CustomerSubscriptions].[ResendPeriod] = @ResendPeriod,[CustomerSubscriptions].[IsDeleted] = @IsDeleted FROM [CustomerSubscriptions]  WHERE [CustomerSubscriptions].[CustomerId] = @CustomerId AND (([CustomerSubscriptions].[CustomerNotificationsType] IS NULL AND @CustomerNotificationsType IS NULL) OR [CustomerSubscriptions].[CustomerNotificationsType] = @CustomerNotificationsType){andTenantId:[CustomerSubscriptions]}  IF @@ROWCOUNT = 0 BEGIN INSERT INTO [CustomerSubscriptions]([CustomerSubscriptions].[CustomerId],[CustomerSubscriptions].[CustomerNotificationsType],[CustomerSubscriptions].[Email],[CustomerSubscriptions].[SMS],[CustomerSubscriptions].[Push],[CustomerSubscriptions].[IsCustomizable],[CustomerSubscriptions].[ResendPeriod],[CustomerSubscriptions].[IsDeleted],[CustomerSubscriptions].[TenantId])  VALUES(@CustomerId,@CustomerNotificationsType,@Email,@SMS,@Push,@IsCustomizable,@ResendPeriod,@IsDeleted,@TenantId)  END";
		private const string WhereQueryByCustomerIdAndCustomerNotificationsType = "WHERE [CustomerSubscriptions].[CustomerId] = @CustomerId AND (([CustomerSubscriptions].[CustomerNotificationsType] IS NULL AND @CustomerNotificationsType IS NULL) OR [CustomerSubscriptions].[CustomerNotificationsType] = @CustomerNotificationsType){andTenantId:[CustomerSubscriptions]} ";
		private const string InsertManyQueryTemplate = @"INSERT INTO [CustomerSubscriptions]([CustomerSubscriptions].[CustomerId],[CustomerSubscriptions].[CustomerNotificationsType],[CustomerSubscriptions].[Email],[CustomerSubscriptions].[SMS],[CustomerSubscriptions].[Push],[CustomerSubscriptions].[IsCustomizable],[CustomerSubscriptions].[ResendPeriod],[CustomerSubscriptions].[IsDeleted],[CustomerSubscriptions].[TenantId])  VALUES {0}";
		private const string InsertManyValuesTemplate = @"(@CustomerId{0},'{1}','{2}','{3}','{4}','{5}','{6}','{7}',@TenantId)";



		public CustomerSubscriptionRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService, TestRepositoryGeneration.Infrastructure.IDataAccessController dataAccessController) : base(dataAccessService, dataAccessController) { }
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
		public TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription GetByCustomerIdAndCustomerNotificationsType(string customerId, int customerNotificationsType)
		{
			object parameters = new { customerId, customerNotificationsType };
			var sql = SelectByQuery + WhereQueryByCustomerIdAndCustomerNotificationsType;
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription> GetByCustomerIdAndCustomerNotificationsTypeAsync(string customerId, int customerNotificationsType)
		{
			object parameters = new { customerId, customerNotificationsType };
			var sql = SelectByQuery + WhereQueryByCustomerIdAndCustomerNotificationsType;
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription>(sql, parameters));
			return result.FirstOrDefault();
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
		public void InsertMany(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription> customerSubscriptionList)
		{
			if (customerSubscriptionList == null) throw new ArgumentException(nameof(customerSubscriptionList));

			if (!customerSubscriptionList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 2;
			var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows
																	? maxInsertManyRowsWithParameters
																	: MaxInsertManyRows;
			var values = new System.Text.StringBuilder();
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = customerSubscriptionList.Select((x, i) => new { Index = i, Value = x })
							.GroupBy(x => x.Index / maxInsertManyRows)
							.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
							.ToList();


			foreach (var items in itemsPerRequest)
			{
				parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				foreach (var item in items)
				{
					var customerSubscription = item.Value;
					var index = item.Index;
					parameters.Add($"CustomerId{index}", customerSubscription.CustomerId);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, index, customerSubscription.CustomerNotificationsType?.ToString() ?? "NULL", customerSubscription.Email ? 1 : 0, customerSubscription.SMS ? 1 : 0, customerSubscription.Push ? 1 : 0, (customerSubscription.IsCustomizable != null ? (customerSubscription.IsCustomizable.Value ? 1 : 0).ToString() : null) ?? "NULL", customerSubscription.ResendPeriod?.ToString() ?? "NULL", customerSubscription.IsDeleted ? 1 : 0);
				}
				query.AppendFormat(InsertManyQueryTemplate, values.Replace("'NULL'", "NULL").ToString());
				DataAccessService.Execute(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				query.Clear();
			}


		}

		public async Task InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription> customerSubscriptionList)
		{
			if (customerSubscriptionList == null) throw new ArgumentException(nameof(customerSubscriptionList));

			if (!customerSubscriptionList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 2;
			var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows
																	? maxInsertManyRowsWithParameters
																	: MaxInsertManyRows;
			var values = new System.Text.StringBuilder();
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = customerSubscriptionList.Select((x, i) => new { Index = i, Value = x })
							.GroupBy(x => x.Index / maxInsertManyRows)
							.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
							.ToList();

			await Task.Delay(10);

			foreach (var items in itemsPerRequest)
			{
				parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				foreach (var item in items)
				{
					var customerSubscription = item.Value;
					var index = item.Index;
					parameters.Add($"CustomerId{index}", customerSubscription.CustomerId);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, index, customerSubscription.CustomerNotificationsType?.ToString() ?? "NULL", customerSubscription.Email ? 1 : 0, customerSubscription.SMS ? 1 : 0, customerSubscription.Push ? 1 : 0, (customerSubscription.IsCustomizable != null ? (customerSubscription.IsCustomizable.Value ? 1 : 0).ToString() : null) ?? "NULL", customerSubscription.ResendPeriod?.ToString() ?? "NULL", customerSubscription.IsDeleted ? 1 : 0);
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
		/*
		public void InsertOrUpdate(TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription customerSubscription)
		{
		DataAccessService.ExecuteScalar(InsertOrUpdateQuery,customerSubscription);
		}
		public async Task InsertOrUpdateAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription customerSubscription)
		{
		await DataAccessService.ExecuteScalarAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.CustomerSubscription >(InsertOrUpdateQuery,customerSubscription);
		}

		*/

	}
}
