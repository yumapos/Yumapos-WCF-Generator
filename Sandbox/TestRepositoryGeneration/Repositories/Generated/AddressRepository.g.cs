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
using YumaPos.Server.Data.Sql;



namespace YumaPos.Server.Infrastructure.Repositories
{
	public partial class AddressRepository : RepositoryBase, IAddressRepository
	{
		private const string Fields = "[dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude]{columns}";
		private const string Values = "@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude{values}";
		private const string SelectAllQuery = "SELECT [dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude] FROM [dbo].[Addresses]   ";
		private const string SelectByQuery = "SELECT [dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude] FROM [dbo].[Addresses] ";
		private const string InsertQuery = "INSERT INTO dbo.Addresses([dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude]{columns})  VALUES(@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude{values}) ";
		private const string UpdateQueryBy = "UPDATE [dbo].[Addresses] SET dbo.Addresses.[Id] = @Id,dbo.Addresses.[Country] = @Country,dbo.Addresses.[City] = @City,dbo.Addresses.[State] = @State,dbo.Addresses.[Street] = @Street,dbo.Addresses.[Building] = @Building,dbo.Addresses.[ZipCode] = @ZipCode,dbo.Addresses.[Latitude] = @Latitude,dbo.Addresses.[Longitude] = @Longitude FROM [dbo].[Addresses] ";
		private const string DeleteQueryBy = "DELETE FROM [dbo].[Addresses] ";
		private const string SelectIntoTempTable = "DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [dbo].[Addresses].[IdAndZipCode] FROM [dbo].[Addresses] ";
		private const string WhereQueryByIdAndZipCode = "WHERE dbo.Addresses.[Id] = @Id AND dbo.Addresses.[ZipCode] = @ZipCode ";
		private const string WhereQueryByCity = "WHERE dbo.Addresses.[City] = @City ";
		private const string WhereQueryByZipCode = "WHERE dbo.Addresses.[ZipCode] = @ZipCode ";


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

		public YumaPos.Server.Infrastructure.DataObjects.Address GetByIdAndZipCode(System.Guid id, string zipCode)
		{
			object parameters = new { id, zipCode };
			var sql = SelectByQuery + WhereQueryByIdAndZipCode;
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.Address> GetByIdAndZipCodeAsync(System.Guid id, string zipCode)
		{
			object parameters = new { id, zipCode };
			var sql = SelectByQuery + WhereQueryByIdAndZipCode;
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters));
			return result.FirstOrDefault();
		}


		/*
		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.Address> GetByCity(string city)
		{
		object parameters = new {city};
		var sql = SelectByQuery + WhereQueryByCity;
		var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters);
		return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.Address>> GetByCityAsync(string city)
		{
		object parameters = new {city};
		var sql = SelectByQuery + WhereQueryByCity;
		var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters));
		return result.ToList();
		}


		*/
		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.Address> GetByZipCode(string zipCode)
		{
			object parameters = new { zipCode };
			var sql = SelectByQuery + WhereQueryByZipCode;
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.Address>> GetByZipCodeAsync(string zipCode)
		{
			object parameters = new { zipCode };
			var sql = SelectByQuery + WhereQueryByZipCode;
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters));
			return result.ToList();
		}


		public void Insert(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
			DataAccessService.InsertObject(address, InsertQuery);
		}
		public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
			await DataAccessService.InsertObjectAsync(address, InsertQuery);
		}

		public void UpdateByIdAndZipCode(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
			var sql = UpdateQueryBy + WhereQueryByIdAndZipCode;
			DataAccessService.PersistObject(address, sql);
		}
		public async Task UpdateByIdAndZipCodeAsync(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
			var sql = UpdateQueryBy + WhereQueryByIdAndZipCode;
			await DataAccessService.PersistObjectAsync(address, sql);
		}

		/*
		public void UpdateByCity(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByCity; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task UpdateByCityAsync(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByCity; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}


		*//*
		public void UpdateByZipCode(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByZipCode; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task UpdateByZipCodeAsync(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByZipCode; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}


		*/
		public void RemoveByIdAndZipCode(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
			var sql = DeleteQueryBy + WhereQueryByIdAndZipCode;
			DataAccessService.PersistObject(address, sql);
		}
		public async Task RemoveByIdAndZipCodeAsync(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
			var sql = DeleteQueryBy + WhereQueryByIdAndZipCode;
			await DataAccessService.PersistObjectAsync(address, sql);
		}

		public void RemoveByIdAndZipCode(System.Guid id, string zipCode)
		{
			object parameters = new { id, zipCode };
			var sql = DeleteQueryBy + WhereQueryByIdAndZipCode;
			DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters);
		}
		public async Task RemoveByIdAndZipCodeAsync(System.Guid id, string zipCode)
		{
			object parameters = new { id, zipCode };
			var sql = DeleteQueryBy + WhereQueryByIdAndZipCode;
			await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters);
		}

		/*
		public void RemoveByCity(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByCity; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task RemoveByCityAsync(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByCity; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}

		public void RemoveByCity(string city)
		{
		object parameters = new {city};
		var sql = DeleteQueryBy + WhereQueryByCity; 
		DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters);
		}
		public async Task RemoveByCityAsync(string city)
		{
		object parameters = new {city};
		var sql = DeleteQueryBy + WhereQueryByCity; 
		await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters);
		}


		*//*
		public void RemoveByZipCode(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByZipCode; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task RemoveByZipCodeAsync(YumaPos.Server.Infrastructure.DataObjects.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByZipCode; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}

		public void RemoveByZipCode(string zipCode)
		{
		object parameters = new {zipCode};
		var sql = DeleteQueryBy + WhereQueryByZipCode; 
		DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters);
		}
		public async Task RemoveByZipCodeAsync(string zipCode)
		{
		object parameters = new {zipCode};
		var sql = DeleteQueryBy + WhereQueryByZipCode; 
		await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.Address>(sql, parameters);
		}


		*/

	}
}
