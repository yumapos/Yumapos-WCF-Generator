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



namespace YumaPos.Server.Infrastructure.Repositories
{
public partial class AddressRepository : RepositoryBase, IAddressRepository
{
private const string Fields = "[Addresss].[Id],[Addresss].[Country],[Addresss].[City],[Addresss].[State],[Addresss].[Street],[Addresss].[Building],[Addresss].[ZipCode],[Addresss].[Latitude],[Addresss].[Longitude]{columns}";
private const string Values = "@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude{values}";
private const string SelectAllQuery = "SELECT [Addresss].[Id],[Addresss].[Country],[Addresss].[City],[Addresss].[State],[Addresss].[Street],[Addresss].[Building],[Addresss].[ZipCode],[Addresss].[Latitude],[Addresss].[Longitude] FROM [Addresss]   ";
private const string SelectByQuery = "SELECT [Addresss].[Id],[Addresss].[Country],[Addresss].[City],[Addresss].[State],[Addresss].[Street],[Addresss].[Building],[Addresss].[ZipCode],[Addresss].[Latitude],[Addresss].[Longitude] FROM [Addresss] ";
private const string InsertQuery = "INSERT INTO Addresss([Addresss].[Id],[Addresss].[Country],[Addresss].[City],[Addresss].[State],[Addresss].[Street],[Addresss].[Building],[Addresss].[ZipCode],[Addresss].[Latitude],[Addresss].[Longitude]) VALUES(@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude) ";
private const string UpdateQueryBy = "UPDATE [Addresss] SET Addresss.[Id] = @Id,Addresss.[Country] = @Country,Addresss.[City] = @City,Addresss.[State] = @State,Addresss.[Street] = @Street,Addresss.[Building] = @Building,Addresss.[ZipCode] = @ZipCode,Addresss.[Latitude] = @Latitude,Addresss.[Longitude] = @Longitude FROM [Addresss] ";
private const string DeleteQueryBy = "DELETE FROM[Addresss] ";
private const string SelectIntoTempTable = "DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [Addresss].[Id] FROM [Addresss] ";
private const string WhereQueryById = "WHERE Addresss.[Id] = @Id ";


public AddressRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.Address> GetAll()
{
var sql = SelectAllQuery;
var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, null).ToList();
return result.ToList();
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.Address>> GetAllAsync()
{
var sql = SelectAllQuery;
var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, null));
return result.ToList();
}

public YumaPos.Server.Infrastructure.DataObjects.Address GetById(Guid  id)
{
object parameters = new {id};
var sql = SelectAllQuery + WhereQueryById;
var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters);
return result.FirstOrDefault();
}
public async Task<YumaPos.Server.Infrastructure.DataObjects.Address> GetByIdAsync(Guid  id)
{
object parameters = new {id};
var sql = SelectAllQuery + WhereQueryById;
var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters));
return result.FirstOrDefault();
}


public void Insert(YumaPos.Server.Infrastructure.DataObjects.Address address)
{
DataAccessService.InsertObject(address,InsertQuery);
}
public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.Address address)
{
await DataAccessService.InsertObjectAsync(address,InsertQuery);
}

public void UpdateById(YumaPos.Server.Infrastructure.DataObjects.Address address)
{
var sql = UpdateQueryBy + WhereQueryById; 
DataAccessService.PersistObject(address, sql);
}
public async Task UpdateByIdAsync(YumaPos.Server.Infrastructure.DataObjects.Address address)
{
var sql = UpdateQueryBy + WhereQueryById; 
await DataAccessService.PersistObjectAsync(address, sql);
}


public void RemoveById(YumaPos.Server.Infrastructure.DataObjects.Address address)
{
var sql = DeleteQueryBy + WhereQueryById; 
DataAccessService.PersistObject(address, sql);
}
public async Task RemoveByIdAsync(YumaPos.Server.Infrastructure.DataObjects.Address address)
{
var sql = DeleteQueryBy + WhereQueryById; 
await DataAccessService.PersistObjectAsync(address, sql);
}

public void RemoveById(Guid  id)
{
var sql = DeleteQueryBy + WhereQueryById; 
DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, new {id});
}
public async Task RemoveByIdAsync(Guid  id)
{
var sql = DeleteQueryBy + WhereQueryById; 
await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, new {id});
}



}
}
