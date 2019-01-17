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
		private const string InsertQuery = @"INSERT INTO [TaxVersions]([Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted],[Taxs].[TenantId])
VALUES (@TaxId,@TaxVersionId,@Name,@Modified,@ModifiedBy,@IsDeleted,@TenantId)";
		private const string SelectBy = @"SELECT [TaxVersions].[TaxId],[TaxVersions].[TaxVersionId],[TaxVersions].[Name],[TaxVersions].[Modified],[TaxVersions].[ModifiedBy],[TaxVersions].[IsDeleted] FROM [TaxVersions]  {filter} ";
		private const string SelectByKeyAndSliceDateQuery = @"SELECT [TaxVersions].[TaxId],[TaxVersions].[TaxVersionId],[TaxVersions].[Name],[TaxVersions].[Modified],[TaxVersions].[ModifiedBy],[TaxVersions].[IsDeleted] FROM (SELECT versionTable1.[TaxId], MAX(versionTable1.[Modified]) as Modified FROM [TaxVersions] versionTable1  {filter}  GROUP BY versionTable1.[TaxId]) versionTable INNER JOIN [TaxVersions] ON versionTable.[TaxId] = [TaxVersions].[TaxId] AND versionTable.[Modified] = [TaxVersions].[Modified]";
		private const string WhereQueryByTaxId = "WHERE [TaxVersions].[TaxId] = @TaxId{andTenantId:[TaxVersions]} ";
		private const string WhereQueryByWithAliasTaxId = "WHERE versionTable1.[TaxId] = @TaxId{andTenantId:versionTable1} ";
		private const string WhereQueryByTaxVersionId = "WHERE [TaxVersions].[TaxVersionId] = @TaxVersionId{andTenantId:[TaxVersions]} ";
		private const string WhereQueryByWithAliasTaxVersionId = "WHERE versionTable1.[TaxVersionId] = @TaxVersionId{andTenantId:versionTable1} ";
		private const string AndWithIsDeletedFilter = "AND [TaxVersions].[IsDeleted] = @IsDeleted ";
		private const string AndWithIsDeletedFilterWithAlias = "AND versionTable1.[IsDeleted] = @IsDeleted ";
		private const string AndWithSliceDateFilter = "AND versionTable1.[Modified] <= @Modified ";
		private const string InsertManyQueryTemplate = @"INSERT INTO [TaxVersions]([TaxVersions].[TaxId],[TaxVersions].[TaxVersionId],[TaxVersions].[Name],[TaxVersions].[Modified],[TaxVersions].[ModifiedBy],[TaxVersions].[IsDeleted],[TaxVersions].[TenantId]) OUTPUT INSERTED.TaxId VALUES {0}";
		private const string InsertManyValuesTemplate = @"('{0}','{1}',@Name{5},'{2}','{3}','{4}',@TenantId)";
		private const string NoCheckConstraintQuery = @"ALTER TABLE [TaxVersions] NOCHECK CONSTRAINT ALL";
		private const string CheckConstraintQuery = @"ALTER TABLE [TaxVersions] CHECK CONSTRAINT ALL";
		private const string ClearCacheQuery = @"DBCC DROPCLEANBUFFERS; DBCC FREEPROCCACHE;";


		public TaxVersionRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService, TestRepositoryGeneration.Infrastructure.IDataAccessController dataAccessController) : base(dataAccessService, dataAccessController) { }
		public void Insert(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
			DataAccessService.InsertObject(tax, InsertQuery);
		}
		public async Task InsertAsync(TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax tax)
		{
			await DataAccessService.InsertObjectAsync(tax, InsertQuery);
		}

		public void InsertMany(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax> taxList)
		{
			if (taxList == null) throw new ArgumentException(nameof(taxList));

			if (!taxList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 2;
			var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows
																	? maxInsertManyRowsWithParameters
																	: MaxInsertManyRows;
			var values = new System.Text.StringBuilder();
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = taxList.Select((x, i) => new { Index = i, Value = x })
							.GroupBy(x => x.Index / maxInsertManyRows)
							.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
							.ToList();

			if (CheckConstraintAfterInsertMany)
			{
				DataAccessService.Execute(NoCheckConstraintQuery);
			}

			foreach (var items in itemsPerRequest)
			{
				parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				foreach (var item in items)
				{
					var tax = item.Value;
					var index = item.Index;
					parameters.Add($"Name{index}", tax.Name);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, tax.TaxId, tax.TaxVersionId, tax.Modified, tax.ModifiedBy, tax.IsDeleted ? 1 : 0, index);
				}
				query.AppendFormat(InsertManyQueryTemplate, values.Replace("'NULL'", "NULL").ToString());
				if (ClearCache)
				{
					DataAccessService.Execute(ClearCacheQuery);
				}
				DataAccessService.Execute(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				query.Clear();
			}

			if (CheckConstraintAfterInsertMany)
			{
				DataAccessService.Execute(CheckConstraintQuery);
			}

		}

		public async Task InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.VersionsRepositories.Tax> taxList)
		{
			if (taxList == null) throw new ArgumentException(nameof(taxList));

			if (!taxList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 2;
			var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows
																	? maxInsertManyRowsWithParameters
																	: MaxInsertManyRows;
			var values = new System.Text.StringBuilder();
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = taxList.Select((x, i) => new { Index = i, Value = x })
							.GroupBy(x => x.Index / maxInsertManyRows)
							.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
							.ToList();

			await Task.Delay(10);
			if (CheckConstraintAfterInsertMany)
			{
				await DataAccessService.ExecuteAsync(NoCheckConstraintQuery);
			}

			foreach (var items in itemsPerRequest)
			{
				parameters.Add($"TenantId", DataAccessController.Tenant.TenantId);
				foreach (var item in items)
				{
					var tax = item.Value;
					var index = item.Index;
					parameters.Add($"Name{index}", tax.Name);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, tax.TaxId, tax.TaxVersionId, tax.Modified, tax.ModifiedBy, tax.IsDeleted ? 1 : 0, index);
				}
				query.AppendFormat(InsertManyQueryTemplate, values.Replace("'NULL'", "NULL").ToString());
				await Task.Delay(10);
				if (ClearCache)
				{
					await DataAccessService.ExecuteAsync(ClearCacheQuery);
				}
				await DataAccessService.ExecuteAsync(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				query.Clear();
			}

			await Task.Delay(10);
			if (CheckConstraintAfterInsertMany)
			{
				await DataAccessService.ExecuteAsync(CheckConstraintQuery);
			}

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
