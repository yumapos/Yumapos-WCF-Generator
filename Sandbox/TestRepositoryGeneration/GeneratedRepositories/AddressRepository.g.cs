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


namespace TestRepositoryGeneration.CustomRepositories.BaseRepositories
{
	public partial class AddressRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase, TestRepositoryGeneration.RepositoryInterfaces.IAddressRepository
	{
		private const string Fields = @"[dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified]";
		private const string SelectAllQuery = @"SELECT [dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified] FROM [dbo].[Addresses]   ";
		private const string SelectByQuery = @"SELECT [dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified] FROM [dbo].[Addresses] ";
		private const string InsertQuery = @"INSERT INTO [dbo].[Addresses]([dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified]) OUTPUT INSERTED.Id VALUES(@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude,@Modified) ";
		private const string InsertManyQuery = @"INSERT INTO [dbo].[Addresses]([dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified]) OUTPUT INSERTED.Id VALUES(@Id{0},@Country{0},@City{0},@State{0},@Street{0},@Building{0},@ZipCode{0},@Latitude{0},@Longitude{0},@Modified{0}) ";
		private const string UpdateQueryBy = @"UPDATE [dbo].[Addresses] SET [dbo].[Addresses].[Id] = @Id,[dbo].[Addresses].[Country] = @Country,[dbo].[Addresses].[City] = @City,[dbo].[Addresses].[State] = @State,[dbo].[Addresses].[Street] = @Street,[dbo].[Addresses].[Building] = @Building,[dbo].[Addresses].[ZipCode] = @ZipCode,[dbo].[Addresses].[Latitude] = @Latitude,[dbo].[Addresses].[Longitude] = @Longitude,[dbo].[Addresses].[Modified] = @Modified FROM [dbo].[Addresses] ";
		private const string DeleteQueryBy = @"UPDATE [dbo].[Addresses] SET IsDeleted = 1 ";
		private const string InsertOrUpdateQuery = @"UPDATE [dbo].[Addresses] SET [dbo].[Addresses].[Id] = @Id,[dbo].[Addresses].[Country] = @Country,[dbo].[Addresses].[City] = @City,[dbo].[Addresses].[State] = @State,[dbo].[Addresses].[Street] = @Street,[dbo].[Addresses].[Building] = @Building,[dbo].[Addresses].[ZipCode] = @ZipCode,[dbo].[Addresses].[Latitude] = @Latitude,[dbo].[Addresses].[Longitude] = @Longitude,[dbo].[Addresses].[Modified] = @Modified FROM [dbo].[Addresses]  WHERE [dbo].[Addresses].[Id] = @Id  IF @@ROWCOUNT = 0 BEGIN INSERT INTO [dbo].[Addresses]([dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified]) OUTPUT INSERTED.Id VALUES(@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude,@Modified)  END";
		private const string WhereQueryById = "WHERE [dbo].[Addresses].[Id] = @Id ";
		private const string WhereQueryByModified = " WHERE [dbo].[Addresses].[Modified] >= @startModified AND [dbo].[Addresses].[Modified] < @endModified";
		private const string WhereQueryByModifiedAndCountryAndCity = "WHERE [dbo].[Addresses].[Country] = @Country AND [dbo].[Addresses].[City] = @City AND [dbo].[Addresses].[Modified] >= @startModified AND [dbo].[Addresses].[Modified] < @endModified";
		private const string WhereQueryByCountryAndCityAndZipCode = "WHERE [dbo].[Addresses].[Country] = @Country AND [dbo].[Addresses].[City] = @City AND [dbo].[Addresses].[ZipCode] = @ZipCode ";
		private const string AndWithIsDeletedFilter = "AND [dbo].[Addresses].[IsDeleted] = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE [dbo].[Addresses].[IsDeleted] = @IsDeleted ";


		public AddressRepository(TestRepositoryGeneration.Infrastructure.IDataAccessService dataAccessService, TestRepositoryGeneration.Infrastructure.IDataAccessController dataAccessController) : base(dataAccessService, dataAccessController) { }
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


		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> GetByModified(System.DateTime startModified, System.DateTime endModified, bool? isDeleted = false)
		{
			object parameters = new { startModified, endModified, isDeleted };
			var sql = SelectByQuery + WhereQueryByModified;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>> GetByModifiedAsync(System.DateTime startModified, System.DateTime endModified, bool? isDeleted = false)
		{
			object parameters = new { startModified, endModified, isDeleted };
			var sql = SelectByQuery + WhereQueryByModified;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters));
			return result.ToList();
		}

		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> GetByModifiedAndCountryAndCity(string country, string city, System.DateTime startModified, System.DateTime endModified, bool? isDeleted = false)
		{
			object parameters = new { country, city, startModified, endModified, isDeleted };
			var sql = SelectByQuery + WhereQueryByModifiedAndCountryAndCity;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>> GetByModifiedAndCountryAndCityAsync(string country, string city, System.DateTime startModified, System.DateTime endModified, bool? isDeleted = false)
		{
			object parameters = new { country, city, startModified, endModified, isDeleted };
			var sql = SelectByQuery + WhereQueryByModifiedAndCountryAndCity;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters));
			return result.ToList();
		}

		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> GetByCountryAndCityAndZipCode(string country, string city, string zipCode, bool? isDeleted = false)
		{
			object parameters = new { country, city, zipCode, isDeleted };
			var sql = SelectByQuery + WhereQueryByCountryAndCityAndZipCode;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>> GetByCountryAndCityAndZipCodeAsync(string country, string city, string zipCode, bool? isDeleted = false)
		{
			object parameters = new { country, city, zipCode, isDeleted };
			var sql = SelectByQuery + WhereQueryByCountryAndCityAndZipCode;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = (await DataAccessService.GetAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters));
			return result.ToList();
		}


		public void Insert(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
			DataAccessService.InsertObject(address, InsertQuery);
		}
		public async Task InsertAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
			await DataAccessService.InsertObjectAsync(address, InsertQuery);
		}

		public void InsertMany(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> addressList)
		{
			if (addressList == null) throw new ArgumentException(nameof(addressList));

			if (!addressList.Any()) return;

			var query = new System.Text.StringBuilder();
			var counter = 0;
			var parameters = new Dictionary<string, object>();
			foreach (var address in addressList)
			{
				if (parameters.Count + 10 > MaxRepositoryParams)
				{
					DataAccessService.Execute(query.ToString(), parameters);
					query.Clear();
					counter = 0;
					parameters.Clear();
				}
				parameters.Add($"Id{counter}", address.Id);
				parameters.Add($"Country{counter}", address.Country);
				parameters.Add($"City{counter}", address.City);
				parameters.Add($"State{counter}", address.State);
				parameters.Add($"Street{counter}", address.Street);
				parameters.Add($"Building{counter}", address.Building);
				parameters.Add($"ZipCode{counter}", address.ZipCode);
				parameters.Add($"Latitude{counter}", address.Latitude);
				parameters.Add($"Longitude{counter}", address.Longitude);
				parameters.Add($"Modified{counter}", address.Modified);
				query.AppendFormat(InsertManyQuery, counter);
				counter++;
			}
			DataAccessService.Execute(query.ToString(), parameters);
		}

		public async Task InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> addressList)
		{
			if (addressList == null) throw new ArgumentException(nameof(addressList));

			if (!addressList.Any()) return;

			var query = new System.Text.StringBuilder();
			var counter = 0;
			var parameters = new Dictionary<string, object>();
			foreach (var address in addressList)
			{
				if (parameters.Count + 10 > MaxRepositoryParams)
				{
					await DataAccessService.ExecuteAsync(query.ToString(), parameters);
					query.Clear();
					counter = 0;
					parameters.Clear();
				}
				parameters.Add($"Id{counter}", address.Id);
				parameters.Add($"Country{counter}", address.Country);
				parameters.Add($"City{counter}", address.City);
				parameters.Add($"State{counter}", address.State);
				parameters.Add($"Street{counter}", address.Street);
				parameters.Add($"Building{counter}", address.Building);
				parameters.Add($"ZipCode{counter}", address.ZipCode);
				parameters.Add($"Latitude{counter}", address.Latitude);
				parameters.Add($"Longitude{counter}", address.Longitude);
				parameters.Add($"Modified{counter}", address.Modified);
				query.AppendFormat(InsertManyQuery, counter);
				counter++;
			}
			await DataAccessService.ExecuteAsync(query.ToString(), parameters);
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
