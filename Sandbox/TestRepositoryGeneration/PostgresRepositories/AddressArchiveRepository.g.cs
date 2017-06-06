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
using TestRepositoryGeneration.RepositoryInterfaces;


namespace TestRepositoryGeneration
{
	public partial class AddressArchiveRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase, TestRepositoryGeneration.RepositoryInterfaces.IAddressArchiveRepository
	{
		private const string Fields = @"archive.addresses.id,archive.addresses.country,archive.addresses.city,archive.addresses.state,archive.addresses.street,archive.addresses.building,archive.addresses.zip_code,archive.addresses.latitude,archive.addresses.longitude";
		private const string SelectAllQuery = @"SELECT archive.addresses.id,archive.addresses.country,archive.addresses.city,archive.addresses.state,archive.addresses.street,archive.addresses.building,archive.addresses.zip_code,archive.addresses.latitude,archive.addresses.longitude FROM archive.addresses   ";
		private const string SelectByQuery = @"SELECT archive.addresses.id,archive.addresses.country,archive.addresses.city,archive.addresses.state,archive.addresses.street,archive.addresses.building,archive.addresses.zip_code,archive.addresses.latitude,archive.addresses.longitude FROM archive.addresses ";
		private const string InsertQuery = @"INSERT INTO archive.addresses(archive.addresses.id,archive.addresses.country,archive.addresses.city,archive.addresses.state,archive.addresses.street,archive.addresses.building,archive.addresses.zip_code,archive.addresses.latitude,archive.addresses.longitude) OUTPUT INSERTED.Id VALUES(@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude) ";
		private const string UpdateQueryBy = @"UPDATE archive.addresses SET archive.addresses.id = @Id,archive.addresses.country = @Country,archive.addresses.city = @City,archive.addresses.state = @State,archive.addresses.street = @Street,archive.addresses.building = @Building,archive.addresses.zip_code = @ZipCode,archive.addresses.latitude = @Latitude,archive.addresses.longitude = @Longitude FROM archive.addresses ";
		private const string DeleteQueryBy = @"UPDATE archive.addresses SET is_deleted = TRUE ";
		private const string InsertOrUpdateQuery = @"INSERT INTO archive.addresses(archive.addresses.id,archive.addresses.country,archive.addresses.city,archive.addresses.state,archive.addresses.street,archive.addresses.building,archive.addresses.zip_code,archive.addresses.latitude,archive.addresses.longitude) OUTPUT INSERTED.Id VALUES(@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude)  ON CONFLICT (id) DO UPDATE archive.addresses SET archive.addresses.id = @Id,archive.addresses.country = @Country,archive.addresses.city = @City,archive.addresses.state = @State,archive.addresses.street = @Street,archive.addresses.building = @Building,archive.addresses.zip_code = @ZipCode,archive.addresses.latitude = @Latitude,archive.addresses.longitude = @Longitude ";
		private const string WhereQueryById = "WHERE archive.addresses.id = @Id ";
		private const string WhereQueryByCountry = "WHERE archive.addresses.country = @Country ";
		private const string WhereQueryByCountryAndCity = "WHERE archive.addresses.country = @Country AND archive.addresses.city = @City ";
		private const string WhereQueryByCountryAndCityAndZipCode = "WHERE archive.addresses.country = @Country AND archive.addresses.city = @City AND archive.addresses.zip_code = @ZipCode ";
		private const string AndWithIsDeletedFilter = "AND archive.addresses.is_deleted = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE archive.addresses.is_deleted = @IsDeleted ";


		public AddressArchiveRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> GetAll(bool? isDeleted = false)
		{
			var sql = SelectAllQuery;
			object parameters = new { isDeleted };
			if (isDeleted.HasValue)
			{
				sql = sql + WhereWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters).ToList();
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>> GetAllAsync(bool? isDeleted = false)
		{
			var sql = SelectAllQuery;
			object parameters = new { isDeleted };
			if (isDeleted.HasValue)
			{
				sql = sql + WhereWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters));
			return result.ToList();
		}

		public TestRepositoryGeneration.DataObjects.BaseRepositories.Address GetById(System.Guid id, bool? isDeleted = false)
		{
			object parameters = new { id, isDeleted };
			var sql = SelectByQuery + WhereQueryById;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> GetByIdAsync(System.Guid id, bool? isDeleted = false)
		{
			object parameters = new { id, isDeleted };
			var sql = SelectByQuery + WhereQueryById;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters));
			return result.FirstOrDefault();
		}


		public TestRepositoryGeneration.DataObjects.BaseRepositories.Address GetByCountry(string country, bool? isDeleted = false)
		{
			object parameters = new { country, isDeleted };
			var sql = SelectByQuery + WhereQueryByCountry;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> GetByCountryAsync(string country, bool? isDeleted = false)
		{
			object parameters = new { country, isDeleted };
			var sql = SelectByQuery + WhereQueryByCountry;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters));
			return result.FirstOrDefault();
		}

		public TestRepositoryGeneration.DataObjects.BaseRepositories.Address GetByCountryAndCity(string country, string city, bool? isDeleted = false)
		{
			object parameters = new { country, city, isDeleted };
			var sql = SelectByQuery + WhereQueryByCountryAndCity;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> GetByCountryAndCityAsync(string country, string city, bool? isDeleted = false)
		{
			object parameters = new { country, city, isDeleted };
			var sql = SelectByQuery + WhereQueryByCountryAndCity;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters));
			return result.FirstOrDefault();
		}

		public TestRepositoryGeneration.DataObjects.BaseRepositories.Address GetByCountryAndCityAndZipCode(string country, string city, string zipCode, bool? isDeleted = false)
		{
			object parameters = new { country, city, zipCode, isDeleted };
			var sql = SelectByQuery + WhereQueryByCountryAndCityAndZipCode;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> GetByCountryAndCityAndZipCodeAsync(string country, string city, string zipCode, bool? isDeleted = false)
		{
			object parameters = new { country, city, zipCode, isDeleted };
			var sql = SelectByQuery + WhereQueryByCountryAndCityAndZipCode;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters));
			return result.FirstOrDefault();
		}


		public void Insert(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
			DataAccessService.InsertObject(address, InsertQuery);
		}
		public async Task InsertAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
			await DataAccessService.InsertObjectAsync(address, InsertQuery);
		}

		public void UpdateById(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
			var sql = UpdateQueryBy + WhereQueryById;
			DataAccessService.PersistObject(address, sql);
		}
		public async Task UpdateByIdAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
			var sql = UpdateQueryBy + WhereQueryById;
			await DataAccessService.PersistObjectAsync(address, sql);
		}

		/*
		public void UpdateByCountry(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByCountry; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task UpdateByCountryAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByCountry; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}


		*//*
		public void UpdateByCountryAndCity(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByCountryAndCity; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task UpdateByCountryAndCityAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByCountryAndCity; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}


		*//*
		public void UpdateByCountryAndCityAndZipCode(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByCountryAndCityAndZipCode; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task UpdateByCountryAndCityAndZipCodeAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByCountryAndCityAndZipCode; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}


		*/
		public void RemoveById(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
			var sql = DeleteQueryBy + WhereQueryById;
			DataAccessService.PersistObject(address, sql);
		}
		public async Task RemoveByIdAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
			var sql = DeleteQueryBy + WhereQueryById;
			await DataAccessService.PersistObjectAsync(address, sql);
		}

		public void RemoveById(System.Guid id)
		{
			object parameters = new { id };
			var sql = DeleteQueryBy + WhereQueryById;
			DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}
		public async Task RemoveByIdAsync(System.Guid id)
		{
			object parameters = new { id };
			var sql = DeleteQueryBy + WhereQueryById;
			await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}

		/*
		public void RemoveByCountry(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByCountry; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task RemoveByCountryAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByCountry; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}

		public void RemoveByCountry(string country)
		{
		object parameters = new {country};
		var sql = DeleteQueryBy + WhereQueryByCountry; 
		DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}
		public async Task RemoveByCountryAsync(string country)
		{
		object parameters = new {country};
		var sql = DeleteQueryBy + WhereQueryByCountry; 
		await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}


		*//*
		public void RemoveByCountryAndCity(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByCountryAndCity; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task RemoveByCountryAndCityAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByCountryAndCity; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}

		public void RemoveByCountryAndCity(string country, string city)
		{
		object parameters = new {country, city};
		var sql = DeleteQueryBy + WhereQueryByCountryAndCity; 
		DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}
		public async Task RemoveByCountryAndCityAsync(string country, string city)
		{
		object parameters = new {country, city};
		var sql = DeleteQueryBy + WhereQueryByCountryAndCity; 
		await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}


		*//*
		public void RemoveByCountryAndCityAndZipCode(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByCountryAndCityAndZipCode; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task RemoveByCountryAndCityAndZipCodeAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByCountryAndCityAndZipCode; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}

		public void RemoveByCountryAndCityAndZipCode(string country, string city, string zipCode)
		{
		object parameters = new {country, city, zipCode};
		var sql = DeleteQueryBy + WhereQueryByCountryAndCityAndZipCode; 
		DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}
		public async Task RemoveByCountryAndCityAndZipCodeAsync(string country, string city, string zipCode)
		{
		object parameters = new {country, city, zipCode};
		var sql = DeleteQueryBy + WhereQueryByCountryAndCityAndZipCode; 
		await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}


		*/
		  /*
		  public void InsertOrUpdate(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		  {
		  DataAccessService.ExecuteScalar(InsertOrUpdateQuery,address);
		  }
		  public async Task InsertOrUpdateAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		  {
		  await DataAccessService.ExecuteScalarAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address >(InsertOrUpdateQuery,address);
		  }

		  */

	}
}