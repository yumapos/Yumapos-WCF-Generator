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
public partial class MenuItemCacheRepository : RepositoryBase
{
private const string Fields = "[MenuItems].[Name],[MenuItems].[Modified],[MenuItems].[ModifiedBy],[MenuItems].[TaxesId],[MenuItems].[MenuCategoryId]{columns}";
private const string Values = "@Name,@Modified,@ModifiedBy,@TaxesId,@MenuCategoryId{values}";
private const string SelectAllQuery = "SELECT [MenuItems].[Name],[MenuItems].[Modified],[MenuItems].[ModifiedBy],[MenuItems].[TaxesId],[MenuItems].[MenuCategoryId],[RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[CategoryId] FROM [MenuItems] INNER JOIN [RecipieItems] ON [MenuItems].[ItemId] = [RecipieItems].[ItemId]{whereTenantId:[MenuItems]}";
private const string SelectByQuery = "SELECT [MenuItems].[Name],[MenuItems].[Modified],[MenuItems].[ModifiedBy],[MenuItems].[TaxesId],[MenuItems].[MenuCategoryId],[RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[CategoryId] FROM [MenuItems] INNER JOIN [RecipieItems] ON [MenuItems].[ItemId] = [RecipieItems].[ItemId]";
private const string InsertQuery = "INSERT INTO MenuItems([Name],[Modified],[ModifiedBy],[TaxesId],[MenuCategoryId]) VALUES(@Name,@Modified,@ModifiedBy,@TaxesId,@MenuCategoryId)";
private const string UpdateQueryBy = "UPDATE [MenuItems] SET MenuItems.[Name] = @Name,MenuItems.[Modified] = @Modified,MenuItems.[ModifiedBy] = @ModifiedBy,MenuItems.[TaxesId] = @TaxesId,MenuItems.[MenuCategoryId] = @MenuCategoryId";
private const string DeleteQueryBy = "DELETE FROM[MenuItems] WHERE [MenuItems].[ItemId] IN (SELECT ItemId FROM @TempValueDb);DELETE FROM[RecipieItems] WHERE [RecipieItems].[ItemId] IN (SELECT ItemId FROM @TempValueDb);";
private const string SelectIntoTempTable = "DECLARE @TempValueDb TABLE (ItemId uniqueidentifier);INSERT INTO @TempValueDb SELECT [MenuItems].[ItemId] FROM [MenuItems]";
private const string UpdateQueryJoin = "UPDATE [RecipieItems] SET MenuItems.[ItemId] = @ItemId,MenuItems.[ItemVersionId] = @ItemVersionId,MenuItems.[IsDeleted] = @IsDeleted,MenuItems.[CategoryId] = @CategoryId INNER JOIN [RecipieItems] ON [MenuItems].[ItemId] = [RecipieItems].[ItemId]";
private const string WhereQueryByItemId = "WHERE MenuItems.[ItemId] = @ItemId{andTenantId:[MenuItems]}";
private const string WhereQueryByMenuCategoryId = "WHERE MenuItems.[MenuCategoryId] = @MenuCategoryId{andTenantId:[MenuItems]}";
private const string AndWithFilterData = "WHERE MenuItems.[IsDeleted] = @IsDeleted{andTenantId:[MenuItems]}";


public MenuItemCacheRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetAll(Boolean? isDeleted = false)
{
object parameters = new {isDeleted};
var sql = SelectAllQuery;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters).ToList();
return result.ToList();
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem>> GetAllAsync(Boolean? isDeleted = false)
{
object parameters = new {isDeleted};
var sql = SelectAllQuery;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters));
return result.ToList();
}

public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetByItemId(Guid  itemId, Boolean? isDeleted = false)
{
object parameters = new {itemId,isDeleted};
var sql = SelectAllQuery + WhereQueryByItemId;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {itemId});
return result.ToList();
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem>> GetByItemIdAsync(Guid  itemId, Boolean? isDeleted = false)
{
object parameters = new {itemId,isDeleted};
var sql = SelectAllQuery + WhereQueryByItemId;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {itemId}));
return result.ToList();
}

/*
public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetByMenuCategoryId(Guid  menuCategoryId, Boolean? isDeleted = false)
{
object parameters = new {menuCategoryId,isDeleted};
var sql = SelectAllQuery + WhereQueryByMenuCategoryId;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {menuCategoryId});
return result.ToList();
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem>> GetByMenuCategoryIdAsync(Guid  menuCategoryId, Boolean? isDeleted = false)
{
object parameters = new {menuCategoryId,isDeleted};
var sql = SelectAllQuery + WhereQueryByMenuCategoryId;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {menuCategoryId}));
return result.ToList();
}


*/
public void Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
DataAccessService.InsertObject(menuItem,InsertQuery);
}
public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
await DataAccessService.InsertObjectAsync(menuItem,InsertQuery);
}

public void UpdateByItemId(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = UpdateQueryBy + WhereQueryByItemId; 
DataAccessService.PersistObject(menuItem, sql);
}
public async Task UpdateByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = UpdateQueryBy + WhereQueryByItemId; 
await DataAccessService.PersistObjectAsync(menuItem, sql);
}

/*
public void UpdateByMenuCategoryId(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = UpdateQueryBy + WhereQueryByMenuCategoryId; 
DataAccessService.PersistObject(menuItem, sql);
}
public async Task UpdateByMenuCategoryIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = UpdateQueryBy + WhereQueryByMenuCategoryId; 
await DataAccessService.PersistObjectAsync(menuItem, sql);
}


*/
public void RemoveByItemId(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = DeleteQueryBy + WhereQueryByItemId; 
DataAccessService.PersistObject(menuItem, sql);
}
public async Task RemoveByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = DeleteQueryBy + WhereQueryByItemId; 
await DataAccessService.PersistObjectAsync(menuItem, sql);
}

public void RemoveByItemId(Guid  itemId)
{
var sql = DeleteQueryBy + WhereQueryByItemId; 
DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {itemId});
}
public async Task RemoveByItemIdAsync(Guid  itemId)
{
var sql = DeleteQueryBy + WhereQueryByItemId; 
await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {itemId});
}

/*
public void RemoveByMenuCategoryId(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = DeleteQueryBy + WhereQueryByMenuCategoryId; 
DataAccessService.PersistObject(menuItem, sql);
}
public async Task RemoveByMenuCategoryIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = DeleteQueryBy + WhereQueryByMenuCategoryId; 
await DataAccessService.PersistObjectAsync(menuItem, sql);
}

public void RemoveByMenuCategoryId(Guid  menuCategoryId)
{
var sql = DeleteQueryBy + WhereQueryByMenuCategoryId; 
DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {menuCategoryId});
}
public async Task RemoveByMenuCategoryIdAsync(Guid  menuCategoryId)
{
var sql = DeleteQueryBy + WhereQueryByMenuCategoryId; 
await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {menuCategoryId});
}


*/

}
}
