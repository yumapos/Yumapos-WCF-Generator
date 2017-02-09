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
	public partial class CustomerSubscriptionRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase, TestRepositoryGeneration.RepositoryInterfaces.ICustomerSubscriptionRepository
	{
		private const string Fields = @"[CustomerSubscriptions].[CustomerId],[CustomerSubscriptions].[CustomerNotificationsType],[CustomerSubscriptions].[Email],[CustomerSubscriptions].[SMS],[CustomerSubscriptions].[Push],[CustomerSubscriptions].[IsCustomizable],[CustomerSubscriptions].[ResendPeriod],[CustomerSubscriptions].[IsDeleted]";
		private const string SelectAllQuery = @"SELECT [CustomerSubscriptions].[CustomerId],[CustomerSubscriptions].[CustomerNotificationsType],[CustomerSubscriptions].[Email],[CustomerSubscriptions].[SMS],[CustomerSubscriptions].[Push],[CustomerSubscriptions].[IsCustomizable],[CustomerSubscriptions].[ResendPeriod],[CustomerSubscriptions].[IsDeleted] FROM [CustomerSubscriptions]  {whereTenantId:[CustomerSubscriptions]} ";
		private const string SelectByQuery = @"SELECT [CustomerSubscriptions].[CustomerId],[CustomerSubscriptions].[CustomerNotificationsType],[CustomerSubscriptions].[Email],[CustomerSubscriptions].[SMS],[CustomerSubscriptions].[Push],[CustomerSubscriptions].[IsCustomizable],[CustomerSubscriptions].[ResendPeriod],[CustomerSubscriptions].[IsDeleted] FROM [CustomerSubscriptions] ";
		private const string InsertQuery = @"INSERT INTO [CustomerSubscriptions]([CustomerSubscriptions].[CustomerId],[CustomerSubscriptions].[CustomerNotificationsType],[CustomerSubscriptions].[Email],[CustomerSubscriptions].[SMS],[CustomerSubscriptions].[Push],[CustomerSubscriptions].[IsCustomizable],[CustomerSubscriptions].[ResendPeriod],[CustomerSubscriptions].[IsDeleted],[CustomerSubscriptions].[TenantId])  VALUES(@CustomerId,@CustomerNotificationsType,@Email,@SMS,@Push,@IsCustomizable,@ResendPeriod,@IsDeleted,@TenantId) ";
		private const string UpdateQueryBy = @"UPDATE [CustomerSubscriptions] SET [CustomerSubscriptions].[CustomerId] = @CustomerId,[CustomerSubscriptions].[CustomerNotificationsType] = @CustomerNotificationsType,[CustomerSubscriptions].[Email] = @Email,[CustomerSubscriptions].[SMS] = @SMS,[CustomerSubscriptions].[Push] = @Push,[CustomerSubscriptions].[IsCustomizable] = @IsCustomizable,[CustomerSubscriptions].[ResendPeriod] = @ResendPeriod,[CustomerSubscriptions].[IsDeleted] = @IsDeleted FROM [CustomerSubscriptions] ";
		private const string DeleteQueryBy = @"DELETE FROM [CustomerSubscriptions] ";
		private const string WhereQueryByCustomerIdAndCustomerNotificationsType = "WHERE [CustomerSubscriptions].[CustomerId] = @CustomerId AND [CustomerSubscriptions].[CustomerNotificationsType] = @CustomerNotificationsType{andTenantId:[CustomerSubscriptions]} ";


		public CustomerSubscriptionRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService) : base(dataAccessService) { }
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

	}
}