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


namespace TestRepositoryGeneration
{
	internal class TaxVersionRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase
	{
		private const string InsertQuery = @"INSERT INTO TaxVersions(Taxs.tax_id,Taxs.tax_version_id,Taxs.name,Taxs.modified,Taxs.modified_by,Taxs.is_deleted,Taxs.tenant_id)
VALUES (@TaxId,@TaxVersionId,@Name,@Modified,@ModifiedBy,@IsDeleted,@TenantId)";
		private const string SelectBy = @"SELECT TaxVersions.tax_id,TaxVersions.tax_version_id,TaxVersions.name,TaxVersions.modified,TaxVersions.modified_by,TaxVersions.is_deleted FROM TaxVersions  {filter} ";
		private const string SelectByKeyAndSliceDateQuery = @"SELECT TaxVersions.tax_id,TaxVersions.tax_version_id,TaxVersions.name,TaxVersions.modified,TaxVersions.modified_by,TaxVersions.is_deleted FROM (SELECT version_table1.TaxId, MAX(version_table1.modified) as modified FROM TaxVersions version_table1  {filter}  GROUP BY version_table1.TaxId) version_table INNER JOIN TaxVersions ON version_table.TaxId = TaxVersions.TaxId AND version_table.modified = TaxVersions.modified";
		private const string WhereQueryByTaxId = "WHERE TaxVersions.tax_id = @TaxId{andTenantId:TaxVersions} ";
		private const string WhereQueryByWithAliasTaxId = "WHERE version_table1.tax_id = @TaxId{andTenantId:version_table1} ";
		private const string WhereQueryByTaxVersionId = "WHERE TaxVersions.tax_version_id = @TaxVersionId{andTenantId:TaxVersions} ";
		private const string WhereQueryByWithAliasTaxVersionId = "WHERE version_table1.tax_version_id = @TaxVersionId{andTenantId:version_table1} ";
		private const string AndWithIsDeletedFilter = "AND TaxVersions.is_deleted = @IsDeleted ";
		private const string AndWithIsDeletedFilterWithAlias = "AND version_table1.is_deleted = @IsDeleted ";
		private const string AndWithSliceDateFilter = "AND version_table1.modified <= @Modified ";

		public TaxVersionRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public void Insert(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
			DataAccessService.InsertObject(tax, InsertQuery);
		}
		public async Task InsertAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
			await DataAccessService.InsertObjectAsync(tax, InsertQuery);
		}

		public TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax GetByTaxId(int taxId, DateTimeOffset modified, bool? isDeleted = false)
		{
			object parameters = new { taxId, modified, isDeleted };
			var filter = WhereQueryByWithAliasTaxId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilterWithAlias;
			}
			filter = filter + AndWithSliceDateFilter;
			var sql = SelectByKeyAndSliceDateQuery.Replace("{filter}", filter);
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax> GetByTaxIdAsync(int taxId, DateTimeOffset modified, bool? isDeleted = false)
		{
			object parameters = new { taxId, modified, isDeleted };
			var filter = WhereQueryByWithAliasTaxId;
			if (isDeleted.HasValue)
			{
				filter = filter + AndWithIsDeletedFilterWithAlias;
			}
			filter = filter + AndWithSliceDateFilter;
			var sql = SelectByKeyAndSliceDateQuery.Replace("{filter}", filter);
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters));
			return result.FirstOrDefault();
		}


		/*
		public IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax> GetByTaxVersionId(System.Guid taxVersionId, bool? isDeleted = false)
		{
		object parameters = new {taxVersionId, isDeleted};
		var filter = WhereQueryByTaxVersionId;
		if (isDeleted.HasValue)
		{
		filter = filter + AndWithIsDeletedFilter;
		}
		var sql = SelectBy.Replace("{filter}", filter);
		var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters);
		return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>> GetByTaxVersionIdAsync(System.Guid taxVersionId, bool? isDeleted = false)
		{
		object parameters = new {taxVersionId, isDeleted};
		var filter = WhereQueryByTaxVersionId;
		if (isDeleted.HasValue)
		{
		filter = filter + AndWithIsDeletedFilter;
		}
		var sql = SelectBy.Replace("{filter}", filter);
		var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax>(sql, parameters));
		return result.ToList();
		}


		*/

	}
}
