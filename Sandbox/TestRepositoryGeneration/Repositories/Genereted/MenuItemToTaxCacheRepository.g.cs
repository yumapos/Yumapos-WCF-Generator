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
using YumaPos.Server.Data.Sql;



namespace TestRepositoryGeneration
{
public partial class MenuItemToTaxCacheRepository : RepositoryBase
{
private const string Fields = "[MenuItemToTaxs].[ItemId],[MenuItemToTaxs].[ItemVersionId],[MenuItemToTaxs].[Modified],[MenuItemToTaxs].[ModifiedBy],[MenuItemToTaxs].[TaxId],[MenuItemToTaxs].[TaxVersionId],[MenuItemToTaxs].[IsDeleted]{columns}";
private const string Values = "@ItemId,@ItemVersionId,@Modified,@ModifiedBy,@TaxId,@TaxVersionId,@IsDeleted{values}";
private const string SelectAllQuery = "SELECT [MenuItemToTaxs].[ItemId],[MenuItemToTaxs].[ItemVersionId],[MenuItemToTaxs].[Modified],[MenuItemToTaxs].[ModifiedBy],[MenuItemToTaxs].[TaxId],[MenuItemToTaxs].[TaxVersionId],[MenuItemToTaxs].[IsDeleted] FROM [MenuItemToTaxs]";
private const string SelectByQuery = "SELECT [MenuItemToTaxs].[ItemId],[MenuItemToTaxs].[ItemVersionId],[MenuItemToTaxs].[Modified],[MenuItemToTaxs].[ModifiedBy],[MenuItemToTaxs].[TaxId],[MenuItemToTaxs].[TaxVersionId],[MenuItemToTaxs].[IsDeleted] FROM [MenuItemToTaxs]";
private const string InsertQuery = "INSERT INTO MenuItemToTaxs([ItemId],[ItemVersionId],[Modified],[ModifiedBy],[TaxId],[TaxVersionId],[IsDeleted]) VALUES(@ItemId,@ItemVersionId,@Modified,@ModifiedBy,@TaxId,@TaxVersionId,@IsDeleted)";
private const string UpdateQueryBy = "UPDATE [MenuItemToTaxs] SET MenuItemToTaxs.[ItemId] = @ItemId,MenuItemToTaxs.[ItemVersionId] = @ItemVersionId,MenuItemToTaxs.[Modified] = @Modified,MenuItemToTaxs.[ModifiedBy] = @ModifiedBy,MenuItemToTaxs.[TaxId] = @TaxId,MenuItemToTaxs.[TaxVersionId] = @TaxVersionId,MenuItemToTaxs.[IsDeleted] = @IsDeleted";
private const string DeleteQueryBy = "DELETE FROM[MenuItemToTaxs]";
private const string SelectIntoTempTable = "DECLARE @TempValueDb TABLE (ItemId uniqueidentifier);INSERT INTO @TempValueDb SELECT [MenuItemToTaxs].[] FROM [MenuItemToTaxs]";
private const string WhereQueryByItemId = "WHERE MenuItemToTaxs.[ItemId] = @ItemId";
private const string WhereQueryByTaxId = "WHERE MenuItemToTaxs.[TaxId] = @TaxId";
private const string AndWithFilterData = "WHERE MenuItemToTaxs.[IsDeleted] = @IsDeleted";


public MenuItemToTaxCacheRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax> GetAll(Boolean? isDeleted = false)
{
object parameters = new {isDeleted};
var sql = SelectAllQuery;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters).ToList();
return result.ToList();
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>> GetAllAsync(Boolean? isDeleted = false)
{
object parameters = new {isDeleted};
var sql = SelectAllQuery;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, parameters));
return result.ToList();
}

public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax> GetByItemId(Guid  itemId, Boolean? isDeleted = false)
{
object parameters = new {itemId,isDeleted};
var sql = SelectAllQuery + WhereQueryByItemId;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, new {itemId});
return result.ToList();
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>> GetByItemIdAsync(Guid  itemId, Boolean? isDeleted = false)
{
object parameters = new {itemId,isDeleted};
var sql = SelectAllQuery + WhereQueryByItemId;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, new {itemId}));
return result.ToList();
}

public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax> GetByTaxId(Guid  taxId, Boolean? isDeleted = false)
{
object parameters = new {taxId,isDeleted};
var sql = SelectAllQuery + WhereQueryByTaxId;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, new {taxId});
return result.ToList();
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>> GetByTaxIdAsync(Guid  taxId, Boolean? isDeleted = false)
{
object parameters = new {taxId,isDeleted};
var sql = SelectAllQuery + WhereQueryByTaxId;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, new {taxId}));
return result.ToList();
}


public void Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
DataAccessService.InsertObject(menuItemToTax,InsertQuery);
}
public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
await DataAccessService.InsertObjectAsync(menuItemToTax,InsertQuery);
}

public void UpdateByItemId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
var sql = UpdateQueryBy + WhereQueryByItemId; 
DataAccessService.PersistObject(menuItemToTax, sql);
}
public async Task UpdateByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
var sql = UpdateQueryBy + WhereQueryByItemId; 
await DataAccessService.PersistObjectAsync(menuItemToTax, sql);
}

public void UpdateByTaxId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
var sql = UpdateQueryBy + WhereQueryByTaxId; 
DataAccessService.PersistObject(menuItemToTax, sql);
}
public async Task UpdateByTaxIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
var sql = UpdateQueryBy + WhereQueryByTaxId; 
await DataAccessService.PersistObjectAsync(menuItemToTax, sql);
}


public void RemoveByItemId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
var sql = DeleteQueryBy + WhereQueryByItemId; 
DataAccessService.PersistObject(menuItemToTax, sql);
}
public async Task RemoveByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
var sql = DeleteQueryBy + WhereQueryByItemId; 
await DataAccessService.PersistObjectAsync(menuItemToTax, sql);
}

public void RemoveByItemId(Guid  itemId)
{
var sql = DeleteQueryBy + WhereQueryByItemId; 
DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, new {itemId});
}
public async Task RemoveByItemIdAsync(Guid  itemId)
{
var sql = DeleteQueryBy + WhereQueryByItemId; 
await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, new {itemId});
}

public void RemoveByTaxId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
var sql = DeleteQueryBy + WhereQueryByTaxId; 
DataAccessService.PersistObject(menuItemToTax, sql);
}
public async Task RemoveByTaxIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
var sql = DeleteQueryBy + WhereQueryByTaxId; 
await DataAccessService.PersistObjectAsync(menuItemToTax, sql);
}

public void RemoveByTaxId(Guid  taxId)
{
var sql = DeleteQueryBy + WhereQueryByTaxId; 
DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, new {taxId});
}
public async Task RemoveByTaxIdAsync(Guid  taxId)
{
var sql = DeleteQueryBy + WhereQueryByTaxId; 
await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>(sql, new {taxId});
}



}
}
