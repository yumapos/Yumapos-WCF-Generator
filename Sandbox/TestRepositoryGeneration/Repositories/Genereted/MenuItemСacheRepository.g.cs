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
public partial class MenuItemСacheRepository : RepositoryBase
{
private const string SelectQuery = "SELECT [Name],[Modified],[ModifiedBy],[TaxesId],[MenuCategoryId] FROM [MenuItems]";
private const string InsertQuery = "INSERT INTO MenuItems([Name],[Modified],[ModifiedBy],[TaxesId],[MenuCategoryId]) VALUES(@Name,@Modified,@ModifiedBy,@TaxesId,@MenuCategoryId)";
private const string WhereQueryByMenuCategoryId = "WHERE MenuItems.[MenuCategoryId] = @MenuCategoryId";
private const string UpdateQueryByMenuCategoryId = "UPDATE [MenuItems] SET MenuItems.[MenuCategoryId] = @MenuCategoryId";
private const string DeleteQueryByMenuCategoryId = "DELETE FROM[MenuItems] WHERE MenuItems.[MenuCategoryId] = @MenuCategoryId";
private const string WhereQueryByModifiedBy = "WHERE MenuItems.[ModifiedBy] = @ModifiedBy";
private const string UpdateQueryByModifiedBy = "UPDATE [MenuItems] SET MenuItems.[ModifiedBy] = @ModifiedBy";
private const string DeleteQueryByModifiedBy = "DELETE FROM[MenuItems] WHERE MenuItems.[ModifiedBy] = @ModifiedBy";
private const string AndWithFilterData = "WHERE MenuItems.[IsDeleted] = @IsDeleted";


public MenuItemСacheRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetAll(Boolean? isDeleted = false)
{
object parameters = new {isDeleted};
var sql = SelectQuery;
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
var sql = SelectQuery;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters));
return result.ToList();
}

/*
public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetByModifiedBy(Guid  modifiedBy, Boolean? isDeleted = false)
{
object parameters = new {modifiedBy,isDeleted};
var sql = SelectQuery + WhereQueryByModifiedBy;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {modifiedBy});
return result.ToList();
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem>> GetByModifiedByAsync(Guid  modifiedBy, Boolean? isDeleted = false)
{
object parameters = new {modifiedBy,isDeleted};
var sql = SelectQuery + WhereQueryByModifiedBy;
if (isDeleted.HasValue)
{
sql = sql + AndWithFilterData;
}
var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {modifiedBy}));
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

/*
public void UpdateByModifiedBy(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = UpdateQueryByModifiedBy + WhereQueryByModifiedBy; 
DataAccessService.PersistObject(menuItem, sql);
}
public async Task UpdateByModifiedByAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = UpdateQueryByModifiedBy + WhereQueryByModifiedBy; 
await DataAccessService.PersistObjectAsync(menuItem, sql);
}


*/
/*
public void RemoveByModifiedBy(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = DeleteQueryByModifiedBy + WhereQueryByModifiedBy; 
DataAccessService.PersistObject(menuItem, sql);
}
public async Task RemoveByModifiedByAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var sql = DeleteQueryByModifiedBy + WhereQueryByModifiedBy; 
await DataAccessService.PersistObjectAsync(menuItem, sql);
}

public void RemoveByModifiedBy(Guid  modifiedBy)
{
var sql = DeleteQueryByModifiedBy + WhereQueryByModifiedBy; 
DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {modifiedBy});
}
public async Task RemoveByModifiedByAsync(Guid  modifiedBy)
{
var sql = DeleteQueryByModifiedBy + WhereQueryByModifiedBy; 
await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, new {modifiedBy});
}


*/

}
}
