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
	internal partial class TaxCacheRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase
	{
		private const string Fields = @"[Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted]";
		private const string SelectAllQuery = @"SELECT [Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted] FROM [Taxs]  {whereTenantId:[Taxs]} ";
		private const string SelectByQuery = @"SELECT [Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted] FROM [Taxs] ";
		private const string InsertQuery = @"INSERT INTO [Taxs]([Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted],[Taxs].[TenantId]) OUTPUT INSERTED.TaxId VALUES(@TaxId,@TaxVersionId,@Name,@Modified,@ModifiedBy,@IsDeleted,@TenantId) ";
		private const string UpdateQueryBy = @"UPDATE [Taxs] SET [Taxs].[TaxId] = @TaxId,[Taxs].[TaxVersionId] = @TaxVersionId,[Taxs].[Name] = @Name,[Taxs].[Modified] = @Modified,[Taxs].[ModifiedBy] = @ModifiedBy,[Taxs].[IsDeleted] = @IsDeleted FROM [Taxs] ";
		private const string DeleteQueryBy = @"DELETE FROM [Taxs] ";
		private const string InsertOrUpdateQuery = @"UPDATE [Taxs] SET [Taxs].[TaxId] = @TaxId,[Taxs].[TaxVersionId] = @TaxVersionId,[Taxs].[Name] = @Name,[Taxs].[Modified] = @Modified,[Taxs].[ModifiedBy] = @ModifiedBy,[Taxs].[IsDeleted] = @IsDeleted FROM [Taxs] WHERE [Taxs].[TaxId] = @TaxId{andTenantId:[Taxs]}  IF @@ROWCOUNT = 0 BEGIN INSERT INTO [Taxs]([Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted],[Taxs].[TenantId]) OUTPUT INSERTED.TaxId VALUES(@TaxId,@TaxVersionId,@Name,@Modified,@ModifiedBy,@IsDeleted,@TenantId)  END";
		private const string WhereQueryByTaxId = "WHERE [Taxs].[TaxId] = @TaxId{andTenantId:[Taxs]} ";
		private const string WhereQueryByTaxVersionId = "WHERE [Taxs].[TaxVersionId] = @TaxVersionId{andTenantId:[Taxs]} ";
		private const string AndWithIsDeletedFilter = "AND [Taxs].[IsDeleted] = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE [Taxs].[IsDeleted] = @IsDeleted{andTenantId:[Taxs]} ";


		public TaxCacheRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService) : base(dataAccessService) { }
		/*
		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax> GetAll(bool? isDeleted = false)
		{
		var sql = SelectAllQuery;
		object parameters = new {isDeleted};
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithIsDeletedFilter;
		}
		var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters).ToList();
		return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>> GetAllAsync(bool? isDeleted = false)
		{
		var sql = SelectAllQuery;
		object parameters = new {isDeleted};
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithIsDeletedFilter;
		}
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters));
		return result.ToList();
		}

		*/
		public TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax GetByTaxId(int taxId, bool? isDeleted = false)
		{
			object parameters = new { taxId, isDeleted };
			var sql = SelectByQuery + WhereQueryByTaxId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax> GetByTaxIdAsync(int taxId, bool? isDeleted = false)
		{
			object parameters = new { taxId, isDeleted };
			var sql = SelectByQuery + WhereQueryByTaxId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters));
			return result.FirstOrDefault();
		}


		/*
		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax> GetByTaxVersionId(System.Guid taxVersionId, bool? isDeleted = false)
		{
		object parameters = new {taxVersionId, isDeleted};
		var sql = SelectByQuery + WhereQueryByTaxVersionId;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithIsDeletedFilter;
		}
		var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters);
		return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>> GetByTaxVersionIdAsync(System.Guid taxVersionId, bool? isDeleted = false)
		{
		object parameters = new {taxVersionId, isDeleted};
		var sql = SelectByQuery + WhereQueryByTaxVersionId;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithIsDeletedFilter;
		}
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters));
		return result.ToList();
		}


		*/
		public void Insert(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
			DataAccessService.InsertObject(tax, InsertQuery);
		}
		public async Task InsertAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
			await DataAccessService.InsertObjectAsync(tax, InsertQuery);
		}

		public void UpdateByTaxId(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
			var sql = UpdateQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject(tax, sql);
		}
		public async Task UpdateByTaxIdAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
			var sql = UpdateQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync(tax, sql);
		}


		public void RemoveByTaxId(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject(tax, sql);
		}
		public async Task RemoveByTaxIdAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync(tax, sql);
		}

		public void RemoveByTaxId(int taxId)
		{
			object parameters = new { taxId };
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters);
		}
		public async Task RemoveByTaxIdAsync(int taxId)
		{
			object parameters = new { taxId };
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters);
		}


		/*
		public void InsertOrUpdate(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
		DataAccessService.ExecuteScalar(InsertOrUpdateQuery,tax);
		}
		public async Task InsertOrUpdateAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
		await DataAccessService.ExecuteScalarAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax >(InsertOrUpdateQuery,tax);
		}

		*/

	}
}
