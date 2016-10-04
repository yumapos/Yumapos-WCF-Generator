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


namespace YumaPos.Server.Data.Sql.Customer
{
	public partial class Address2Repository : RepositoryBase, IAddress2Repository
	{
		private const string Fields = @"[dbo].[Addresses2].[Id],[dbo].[Addresses2].[Country],[dbo].[Addresses2].[City],[dbo].[Addresses2].[State],[dbo].[Addresses2].[Street],[dbo].[Addresses2].[Building],[dbo].[Addresses2].[ZipCode],[dbo].[Addresses2].[Latitude],[dbo].[Addresses2].[Longitude]{columns}";
		private const string Values = @"@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude{values}";
		private const string SelectAllQuery = @"SELECT [dbo].[Addresses2].[Id],[dbo].[Addresses2].[Country],[dbo].[Addresses2].[City],[dbo].[Addresses2].[State],[dbo].[Addresses2].[Street],[dbo].[Addresses2].[Building],[dbo].[Addresses2].[ZipCode],[dbo].[Addresses2].[Latitude],[dbo].[Addresses2].[Longitude] FROM [dbo].[Addresses2]   ";
		private const string SelectByQuery = @"SELECT [dbo].[Addresses2].[Id],[dbo].[Addresses2].[Country],[dbo].[Addresses2].[City],[dbo].[Addresses2].[State],[dbo].[Addresses2].[Street],[dbo].[Addresses2].[Building],[dbo].[Addresses2].[ZipCode],[dbo].[Addresses2].[Latitude],[dbo].[Addresses2].[Longitude] FROM [dbo].[Addresses2] ";
		private const string InsertQuery = @"INSERT INTO [dbo].[Addresses2]([dbo].[Addresses2].[Country],[dbo].[Addresses2].[City],[dbo].[Addresses2].[State],[dbo].[Addresses2].[Street],[dbo].[Addresses2].[Building],[dbo].[Addresses2].[ZipCode],[dbo].[Addresses2].[Latitude],[dbo].[Addresses2].[Longitude]) OUTPUT INSERTED.Id VALUES(@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude) ";
		private const string UpdateQueryBy = @"UPDATE [dbo].[Addresses2] SET [dbo].[Addresses2].[Country] = @Country,[dbo].[Addresses2].[City] = @City,[dbo].[Addresses2].[State] = @State,[dbo].[Addresses2].[Street] = @Street,[dbo].[Addresses2].[Building] = @Building,[dbo].[Addresses2].[ZipCode] = @ZipCode,[dbo].[Addresses2].[Latitude] = @Latitude,[dbo].[Addresses2].[Longitude] = @Longitude FROM [dbo].[Addresses2] ";
		private const string DeleteQueryBy = @"DELETE FROM [dbo].[Addresses2] ";
		private const string SelectIntoTempTable = @"DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [dbo].[Addresses2].[Id] FROM [dbo].[Addresses2] ";
		private const string WhereQueryById = "WHERE [dbo].[Addresses2].[Id] = @Id ";
		private const string WhereQueryByZipCode = "WHERE [dbo].[Addresses2].[ZipCode] = @ZipCode ";
		private const string WhereQueryByBuilding = "WHERE [dbo].[Addresses2].[Building] = @Building ";


		public Address2Repository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.Address2> GetAll()
		{
			var sql = SelectAllQuery;
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, null).ToList();
			return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.Address2>> GetAllAsync()
		{
			var sql = SelectAllQuery;
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, null));
			return result.ToList();
		}

		public YumaPos.Server.Infrastructure.DataObjects.Address2 GetById(int id)
		{
			object parameters = new { id };
			var sql = SelectByQuery + WhereQueryById;
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.Address2> GetByIdAsync(int id)
		{
			object parameters = new { id };
			var sql = SelectByQuery + WhereQueryById;
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters));
			return result.FirstOrDefault();
		}


		public YumaPos.Server.Infrastructure.DataObjects.Address2 GetByZipCode(string zipCode)
		{
			object parameters = new { zipCode };
			var sql = SelectByQuery + WhereQueryByZipCode;
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.Address2> GetByZipCodeAsync(string zipCode)
		{
			object parameters = new { zipCode };
			var sql = SelectByQuery + WhereQueryByZipCode;
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters));
			return result.FirstOrDefault();
		}

		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.Address2> GetByBuilding(string building)
		{
			object parameters = new { building };
			var sql = SelectByQuery + WhereQueryByBuilding;
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.Address2>> GetByBuildingAsync(string building)
		{
			object parameters = new { building };
			var sql = SelectByQuery + WhereQueryByBuilding;
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters));
			return result.ToList();
		}


		public int Insert(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		{
			var res = DataAccessService.InsertObject(address2, InsertQuery);
			return (int)res;
		}
		public async Task<int> InsertAsync(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		{
			var res = await DataAccessService.InsertObjectAsync(address2, InsertQuery);
			return (int)res;
		}

		/*
		public void UpdateById(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		{
		var sql = UpdateQueryBy + WhereQueryById; 
		DataAccessService.PersistObject(address2, sql);
		}
		public async Task UpdateByIdAsync(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		{
		var sql = UpdateQueryBy + WhereQueryById; 
		await DataAccessService.PersistObjectAsync(address2, sql);
		}


		*//*
		public void UpdateByZipCode(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		{
		var sql = UpdateQueryBy + WhereQueryByZipCode; 
		DataAccessService.PersistObject(address2, sql);
		}
		public async Task UpdateByZipCodeAsync(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		{
		var sql = UpdateQueryBy + WhereQueryByZipCode; 
		await DataAccessService.PersistObjectAsync(address2, sql);
		}


		*//*
		public void UpdateByBuilding(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		{
		var sql = UpdateQueryBy + WhereQueryByBuilding; 
		DataAccessService.PersistObject(address2, sql);
		}
		public async Task UpdateByBuildingAsync(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		{
		var sql = UpdateQueryBy + WhereQueryByBuilding; 
		await DataAccessService.PersistObjectAsync(address2, sql);
		}


		*/
		  /*
		  public void RemoveById(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		  {
		  var sql = DeleteQueryBy + WhereQueryById; 
		  DataAccessService.PersistObject(address2, sql);
		  }
		  public async Task RemoveByIdAsync(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		  {
		  var sql = DeleteQueryBy + WhereQueryById; 
		  await DataAccessService.PersistObjectAsync(address2, sql);
		  }

		  public void RemoveById(int id)
		  {
		  object parameters = new {id};
		  var sql = DeleteQueryBy + WhereQueryById; 
		  DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters);
		  }
		  public async Task RemoveByIdAsync(int id)
		  {
		  object parameters = new {id};
		  var sql = DeleteQueryBy + WhereQueryById; 
		  await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters);
		  }


		  *//*
		  public void RemoveByZipCode(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		  {
		  var sql = DeleteQueryBy + WhereQueryByZipCode; 
		  DataAccessService.PersistObject(address2, sql);
		  }
		  public async Task RemoveByZipCodeAsync(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		  {
		  var sql = DeleteQueryBy + WhereQueryByZipCode; 
		  await DataAccessService.PersistObjectAsync(address2, sql);
		  }

		  public void RemoveByZipCode(string zipCode)
		  {
		  object parameters = new {zipCode};
		  var sql = DeleteQueryBy + WhereQueryByZipCode; 
		  DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters);
		  }
		  public async Task RemoveByZipCodeAsync(string zipCode)
		  {
		  object parameters = new {zipCode};
		  var sql = DeleteQueryBy + WhereQueryByZipCode; 
		  await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters);
		  }


		  *//*
		  public void RemoveByBuilding(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		  {
		  var sql = DeleteQueryBy + WhereQueryByBuilding; 
		  DataAccessService.PersistObject(address2, sql);
		  }
		  public async Task RemoveByBuildingAsync(YumaPos.Server.Infrastructure.DataObjects.Address2 address2)
		  {
		  var sql = DeleteQueryBy + WhereQueryByBuilding; 
		  await DataAccessService.PersistObjectAsync(address2, sql);
		  }

		  public void RemoveByBuilding(string building)
		  {
		  object parameters = new {building};
		  var sql = DeleteQueryBy + WhereQueryByBuilding; 
		  DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters);
		  }
		  public async Task RemoveByBuildingAsync(string building)
		  {
		  object parameters = new {building};
		  var sql = DeleteQueryBy + WhereQueryByBuilding; 
		  await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.Address2>(sql, parameters);
		  }


		  */

	}
}