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


namespace YumaPos.Server.Data.Sql
{
	internal partial class TaxCacheRepository : RepositoryBase, ITaxRepository
	{
		private const string Fields = @"[Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted]{columns}";
		private const string Values = @"@TaxId,@TaxVersionId,@Name,@Modified,@ModifiedBy,@IsDeleted{values}";
		private const string SelectAllQuery = @"SELECT [Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted] FROM [Taxs]  {whereTenantId:[Taxs]} ";
		private const string SelectByQuery = @"SELECT [Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted] FROM [Taxs] ";
		private const string InsertQuery = @"INSERT INTO [Taxs]([Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted]{columns}) OUTPUT INSERTED.TaxId VALUES(@TaxVersionId,@Name,@Modified,@ModifiedBy,@IsDeleted{values}) ";
		private const string UpdateQueryBy = @"UPDATE [Taxs] SET [Taxs].[TaxVersionId] = @TaxVersionId,[Taxs].[Name] = @Name,[Taxs].[Modified] = @Modified,[Taxs].[ModifiedBy] = @ModifiedBy,[Taxs].[IsDeleted] = @IsDeleted FROM [Taxs] ";
		private const string DeleteQueryBy = @"DELETE FROM [Taxs] ";
		private const string SelectIntoTempTable = @"DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [Taxs].[TaxId] FROM [Taxs] ";
		private const string WhereQueryByTaxId = "WHERE [Taxs].[TaxId] = @TaxId{andTenantId:[Taxs]} ";
		private const string WhereQueryByTaxVersionId = "WHERE [Taxs].[TaxVersionId] = @TaxVersionId{andTenantId:[Taxs]} ";
		private const string AndWithFilterData = "AND [Taxs].[IsDeleted] = @IsDeleted";


		public TaxCacheRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		/*
		public IEnumerable<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax> GetAll(bool? isDeleted = false)
		{
		object parameters = new {isDeleted};
		var sql = SelectAllQuery;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithFilterData;
		}
		var result = DataAccessService.Get<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, parameters).ToList();
		return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>> GetAllAsync(bool? isDeleted = false)
		{
		object parameters = new {isDeleted};
		var sql = SelectAllQuery;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithFilterData;
		}
		var result = (await DataAccessService.GetAsync<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, parameters));
		return result.ToList();
		}

		*/
		public YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax GetByTaxId(int taxId, bool? isDeleted = false)
		{
			object parameters = new { taxId, isDeleted };
			var sql = SelectByQuery + WhereQueryByTaxId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax> GetByTaxIdAsync(int taxId, bool? isDeleted = false)
		{
			object parameters = new { taxId, isDeleted };
			var sql = SelectByQuery + WhereQueryByTaxId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, parameters));
			return result.FirstOrDefault();
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
		public int Insert(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax tax)
		{
			var res = DataAccessService.InsertObject(tax, InsertQuery);
			return (int)res;
		}
		public async Task<int> InsertAsync(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax tax)
		{
			var res = await DataAccessService.InsertObjectAsync(tax, InsertQuery);
			return (int)res;
		}

		public void UpdateByTaxId(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax tax)
		{
			var sql = UpdateQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject(tax, sql);
		}
		public async Task UpdateByTaxIdAsync(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax tax)
		{
			var sql = UpdateQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync(tax, sql);
		}


		public void RemoveByTaxId(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax tax)
		{
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject(tax, sql);
		}
		public async Task RemoveByTaxIdAsync(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax tax)
		{
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync(tax, sql);
		}

		public void RemoveByTaxId(int taxId)
		{
			object parameters = new { taxId };
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			DataAccessService.PersistObject<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, parameters);
		}
		public async Task RemoveByTaxIdAsync(int taxId)
		{
			object parameters = new { taxId };
			var sql = DeleteQueryBy + WhereQueryByTaxId;
			await DataAccessService.PersistObjectAsync<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, parameters);
		}



	}
}