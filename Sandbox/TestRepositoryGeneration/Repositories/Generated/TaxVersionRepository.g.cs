﻿//------------------------------------------------------------------------------
// <auto-generated>
//	 This code was generated from a template.
//
//	 Manual changes to this file may cause unexpected behavior in your application.
//	 Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Threading.Tasks;
using YumaPos.Server.Data.Sql;



namespace TestRepositoryGeneration
{
	internal class TaxVersionRepository : RepositoryBase
	{
		private const string InsertQuery = @"INSERT INTO TaxVersions([Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted])
VALUES (@TaxId,@TaxVersionId,@Name,@Modified,@ModifiedBy,@IsDeleted)";
		private const string SelectByQuery = @"SELECT [Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted] FROM [Taxs] ";
		private const string WhereQueryByTaxVersionId = @"WHERE Taxs.[TaxVersionId] = @TaxVersionId{andTenantId:[Taxs]} ";
		private const string AndWithFilterData = "AND Taxs.[IsDeleted] = @IsDeleted";

		public TaxVersionRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public void Insert(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax tax)
		{
			DataAccessService.InsertObject(tax, InsertQuery);
		}
		public async Task InsertAsync(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax tax)
		{
			await DataAccessService.InsertObjectAsync(tax, InsertQuery);
		}

		/*
		public YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax GetByTaxVersionId(System.Guid taxVersionId, bool? isDeleted = false)
		{
		object parameters = new {taxVersionId,isDeleted};
		var sql = SelectByQuery + WhereQueryByTaxVersionId;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithFilterData;
		}
		var result = DataAccessService.Get<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, parameters);
		return result.FirstOrDefault();
		}
		public async Task<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax> GetByTaxVersionIdAsync(System.Guid taxVersionId, bool? isDeleted = false)
		{
		object parameters = new {taxVersionId,isDeleted};
		var sql = SelectByQuery + WhereQueryByTaxVersionId;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithFilterData;
		}
		var result = (await DataAccessService.GetAsync<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, parameters));
		return result.FirstOrDefault();
		}


		*/

	}
}
