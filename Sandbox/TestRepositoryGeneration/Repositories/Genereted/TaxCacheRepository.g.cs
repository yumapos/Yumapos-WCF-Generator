﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
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
internal class TaxCacheRepository : RepositoryBase
{
private const string Fields = "[Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted]{columns}";
private const string Values = "@TaxId,@TaxVersionId,@Name,@Modified,@ModifiedBy,@IsDeleted{values}";
private const string SelectAllQuery = "SELECT [Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted] FROM [Taxs]  {whereTenantId:[Taxs]} ";
private const string SelectByQuery = "SELECT [Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted] FROM [Taxs] ";
private const string InsertQuery = "INSERT INTO Taxs([Taxs].[TaxId],[Taxs].[TaxVersionId],[Taxs].[Name],[Taxs].[Modified],[Taxs].[ModifiedBy],[Taxs].[IsDeleted]) VALUES(@TaxId,@TaxVersionId,@Name,@Modified,@ModifiedBy,@IsDeleted) ";
private const string UpdateQueryBy = "UPDATE [Taxs] SET Taxs.[TaxId] = @TaxId,Taxs.[TaxVersionId] = @TaxVersionId,Taxs.[Name] = @Name,Taxs.[Modified] = @Modified,Taxs.[ModifiedBy] = @ModifiedBy,Taxs.[IsDeleted] = @IsDeleted FROM [Taxs] ";
private const string DeleteQueryBy = "DELETE FROM[Taxs] ";
private const string SelectIntoTempTable = "DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [Taxs].[TaxId] FROM [Taxs] ";
private const string WhereQueryByTaxId = "WHERE Taxs.[TaxId] = @TaxId{andTenantId:[Taxs]} ";
private const string AndWithFilterData = "WHERE Taxs.[IsDeleted] = @IsDeleted{andTenantId:[Taxs]} ";


public TaxCacheRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
/*
public IEnumerable<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax> GetAll(Boolean? isDeleted = false)
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
public async Task<IEnumerable<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>> GetAllAsync(Boolean? isDeleted = false)
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
public YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax GetByTaxId(Guid  taxId, Boolean? isDeleted = false)
{
object parameters = new {taxId,isDeleted};
var sql = SelectAllQuery + WhereQueryByTaxId;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = DataAccessService.Get<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, parameters);
return result.FirstOrDefault();
}
public async Task<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax> GetByTaxIdAsync(Guid  taxId, Boolean? isDeleted = false)
{
object parameters = new {taxId,isDeleted};
var sql = SelectAllQuery + WhereQueryByTaxId;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = (await DataAccessService.GetAsync<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, parameters));
return result.FirstOrDefault();
}


public void Insert(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax tax)
{
DataAccessService.InsertObject(tax,InsertQuery);
}
public async Task InsertAsync(YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax tax)
{
await DataAccessService.InsertObjectAsync(tax,InsertQuery);
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

public void RemoveByTaxId(Guid  taxId)
{
var sql = DeleteQueryBy + WhereQueryByTaxId; 
DataAccessService.PersistObject<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, new {taxId});
}
public async Task RemoveByTaxIdAsync(Guid  taxId)
{
var sql = DeleteQueryBy + WhereQueryByTaxId; 
await DataAccessService.PersistObjectAsync<YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes.Tax>(sql, new {taxId});
}



}
}
