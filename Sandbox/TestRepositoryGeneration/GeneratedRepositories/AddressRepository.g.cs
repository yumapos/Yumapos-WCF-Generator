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
using System.Globalization;
using TestRepositoryGeneration.RepositoryInterfaces;


namespace TestRepositoryGeneration.CustomRepositories.BaseRepositories
{
	public partial class AddressRepository : TestRepositoryGeneration.Infrastructure.RepositoryBase, TestRepositoryGeneration.RepositoryInterfaces.IAddressRepository
	{
		public const string Fields = @"[dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified],[dbo].[Addresses].[ExpireDate]";
		private const string SelectAllQuery = @"SELECT [dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified],[dbo].[Addresses].[ExpireDate] FROM [dbo].[Addresses]   ";
		private const string SelectByQuery = @"SELECT [dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified],[dbo].[Addresses].[ExpireDate] FROM [dbo].[Addresses] ";
		private const string InsertQuery = @"INSERT INTO [dbo].[Addresses]([dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified],[dbo].[Addresses].[ExpireDate]) OUTPUT INSERTED.Id VALUES(@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude,@Modified,@ExpireDate) ";
		private const string UpdateQueryBy = @"UPDATE [dbo].[Addresses] SET [dbo].[Addresses].[Country] = @Country,[dbo].[Addresses].[City] = @City,[dbo].[Addresses].[State] = @State,[dbo].[Addresses].[Street] = @Street,[dbo].[Addresses].[Building] = @Building,[dbo].[Addresses].[ZipCode] = @ZipCode,[dbo].[Addresses].[Latitude] = @Latitude,[dbo].[Addresses].[Longitude] = @Longitude,[dbo].[Addresses].[Modified] = @Modified,[dbo].[Addresses].[ExpireDate] = @ExpireDate FROM [dbo].[Addresses] ";
		private const string DeleteQueryBy = @"UPDATE [dbo].[Addresses] SET IsDeleted = 1 ";
		private const string InsertOrUpdateQuery = @"UPDATE [dbo].[Addresses] SET [dbo].[Addresses].[Country] = @Country,[dbo].[Addresses].[City] = @City,[dbo].[Addresses].[State] = @State,[dbo].[Addresses].[Street] = @Street,[dbo].[Addresses].[Building] = @Building,[dbo].[Addresses].[ZipCode] = @ZipCode,[dbo].[Addresses].[Latitude] = @Latitude,[dbo].[Addresses].[Longitude] = @Longitude,[dbo].[Addresses].[Modified] = @Modified,[dbo].[Addresses].[ExpireDate] = @ExpireDate FROM [dbo].[Addresses]  WHERE [dbo].[Addresses].[Id] = @Id  IF @@ROWCOUNT = 0 BEGIN INSERT INTO [dbo].[Addresses]([dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified],[dbo].[Addresses].[ExpireDate]) OUTPUT INSERTED.Id VALUES(@Id,@Country,@City,@State,@Street,@Building,@ZipCode,@Latitude,@Longitude,@Modified,@ExpireDate)  END";
		private const string WhereQueryById = "WHERE [dbo].[Addresses].[Id] = @Id ";
		private const string WhereQueryByExpireDate = "WHERE (([dbo].[Addresses].[ExpireDate] IS NULL AND @ExpireDate IS NULL) OR [dbo].[Addresses].[ExpireDate] = @ExpireDate) ";
		private const string WhereQueryByModifiedAndCountryAndCity = "WHERE [dbo].[Addresses].[Modified] >= @startModified AND [dbo].[Addresses].[Modified] < @endModified AND [dbo].[Addresses].[Country] = @Country AND [dbo].[Addresses].[City] = @City ";
		private const string WhereQueryByLatitudeAndLongitude = "WHERE (([dbo].[Addresses].[Latitude] IS NULL AND @Latitude IS NULL) OR [dbo].[Addresses].[Latitude] = @Latitude) AND (([dbo].[Addresses].[Longitude] IS NULL AND @Longitude IS NULL) OR [dbo].[Addresses].[Longitude] = @Longitude) ";
		private const string AndWithIsDeletedFilter = "AND [dbo].[Addresses].[IsDeleted] = @IsDeleted ";
		private const string WhereWithIsDeletedFilter = "WHERE [dbo].[Addresses].[IsDeleted] = @IsDeleted ";
		private const string InsertManyQueryTemplate = @"INSERT INTO [dbo].[Addresses]([dbo].[Addresses].[Id],[dbo].[Addresses].[Country],[dbo].[Addresses].[City],[dbo].[Addresses].[State],[dbo].[Addresses].[Street],[dbo].[Addresses].[Building],[dbo].[Addresses].[ZipCode],[dbo].[Addresses].[Latitude],[dbo].[Addresses].[Longitude],[dbo].[Addresses].[Modified],[dbo].[Addresses].[ExpireDate]) OUTPUT INSERTED.Id VALUES {0}";
		private const string InsertManyValuesTemplate = @"('{1}',@Country{0},@City{0},@State{0},@Street{0},@Building{0},@ZipCode{0},'{2}','{3}','{4}','{5}')";



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


		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> GetByExpireDate(System.DateTimeOffset? expireDate, bool? isDeleted = false)
		{
			object parameters = new { expireDate, isDeleted };
			var sql = SelectByQuery + WhereQueryByExpireDate;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>> GetByExpireDateAsync(System.DateTimeOffset? expireDate, bool? isDeleted = false)
		{
			object parameters = new { expireDate, isDeleted };
			var sql = SelectByQuery + WhereQueryByExpireDate;
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

		public IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> GetByLatitudeAndLongitude(decimal? latitude, decimal? longitude, bool? isDeleted = false)
		{
			object parameters = new { latitude, longitude, isDeleted };
			var sql = SelectByQuery + WhereQueryByLatitudeAndLongitude;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithIsDeletedFilter;
			}
			var result = DataAccessService.Get<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>> GetByLatitudeAndLongitudeAsync(decimal? latitude, decimal? longitude, bool? isDeleted = false)
		{
			object parameters = new { latitude, longitude, isDeleted };
			var sql = SelectByQuery + WhereQueryByLatitudeAndLongitude;
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

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 6;
			var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows
																	? maxInsertManyRowsWithParameters
																	: MaxInsertManyRows;
			var values = new System.Text.StringBuilder();
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = addressList.Select((x, i) => new { Index = i, Value = x })
							.GroupBy(x => x.Index / maxInsertManyRows)
							.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
							.ToList();


			foreach (var items in itemsPerRequest)
			{
				foreach (var item in items)
				{
					var address = item.Value;
					var index = item.Index;
					parameters.Add($"Country{index}", address.Country);
					parameters.Add($"City{index}", address.City);
					parameters.Add($"State{index}", address.State);
					parameters.Add($"Street{index}", address.Street);
					parameters.Add($"Building{index}", address.Building);
					parameters.Add($"ZipCode{index}", address.ZipCode);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, index, address.Id, address.Latitude?.ToString(CultureInfo.InvariantCulture) ?? "NULL", address.Longitude?.ToString(CultureInfo.InvariantCulture) ?? "NULL", address.Modified.ToString(CultureInfo.InvariantCulture), address.ExpireDate?.ToString(CultureInfo.InvariantCulture) ?? "NULL");
				}
				query.AppendFormat(InsertManyQueryTemplate, values.Replace("'NULL'", "NULL").ToString());
				DataAccessService.Execute(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				query.Clear();
			}


		}

		public async Task InsertManyAsync(IEnumerable<TestRepositoryGeneration.DataObjects.BaseRepositories.Address> addressList)
		{
			if (addressList == null) throw new ArgumentException(nameof(addressList));

			if (!addressList.Any()) return;

			var maxInsertManyRowsWithParameters = MaxRepositoryParams / 6;
			var maxInsertManyRows = maxInsertManyRowsWithParameters < MaxInsertManyRows
																	? maxInsertManyRowsWithParameters
																	: MaxInsertManyRows;
			var values = new System.Text.StringBuilder();
			var query = new System.Text.StringBuilder();
			var parameters = new Dictionary<string, object>();

			var itemsPerRequest = addressList.Select((x, i) => new { Index = i, Value = x })
							.GroupBy(x => x.Index / maxInsertManyRows)
							.Select(x => x.Select((v, i) => new { Index = i, Value = v.Value }).ToList())
							.ToList();

			await Task.Delay(10);

			foreach (var items in itemsPerRequest)
			{
				foreach (var item in items)
				{
					var address = item.Value;
					var index = item.Index;
					parameters.Add($"Country{index}", address.Country);
					parameters.Add($"City{index}", address.City);
					parameters.Add($"State{index}", address.State);
					parameters.Add($"Street{index}", address.Street);
					parameters.Add($"Building{index}", address.Building);
					parameters.Add($"ZipCode{index}", address.ZipCode);
					values.AppendLine(index != 0 ? "," : "");
					values.AppendFormat(InsertManyValuesTemplate, index, address.Id, address.Latitude?.ToString(CultureInfo.InvariantCulture) ?? "NULL", address.Longitude?.ToString(CultureInfo.InvariantCulture) ?? "NULL", address.Modified.ToString(CultureInfo.InvariantCulture), address.ExpireDate?.ToString(CultureInfo.InvariantCulture) ?? "NULL");
				}
				query.AppendFormat(InsertManyQueryTemplate, values.Replace("'NULL'", "NULL").ToString());
				await Task.Delay(10);
				await DataAccessService.ExecuteAsync(query.ToString(), parameters);
				parameters.Clear();
				values.Clear();
				query.Clear();
			}

			await Task.Delay(10);

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
		public void UpdateByExpireDate(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByExpireDate; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task UpdateByExpireDateAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByExpireDate; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}


		*//*
		public void UpdateByLatitudeAndLongitude(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByLatitudeAndLongitude; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task UpdateByLatitudeAndLongitudeAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = UpdateQueryBy + WhereQueryByLatitudeAndLongitude; 
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

		public void RemoveByExpireDate(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
			var sql = DeleteQueryBy + WhereQueryByExpireDate;
			DataAccessService.PersistObject(address, sql);
		}
		public async Task RemoveByExpireDateAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
			var sql = DeleteQueryBy + WhereQueryByExpireDate;
			await DataAccessService.PersistObjectAsync(address, sql);
		}

		public void RemoveByExpireDate(System.DateTimeOffset? expireDate)
		{
			object parameters = new { expireDate };
			var sql = DeleteQueryBy + WhereQueryByExpireDate;
			DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}
		public async Task RemoveByExpireDateAsync(System.DateTimeOffset? expireDate)
		{
			object parameters = new { expireDate };
			var sql = DeleteQueryBy + WhereQueryByExpireDate;
			await DataAccessService.PersistObjectAsync<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}

		/*
		public void RemoveByLatitudeAndLongitude(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByLatitudeAndLongitude; 
		DataAccessService.PersistObject(address, sql);
		}
		public async Task RemoveByLatitudeAndLongitudeAsync(TestRepositoryGeneration.DataObjects.BaseRepositories.Address address)
		{
		var sql = DeleteQueryBy + WhereQueryByLatitudeAndLongitude; 
		await DataAccessService.PersistObjectAsync(address, sql);
		}

		public void RemoveByLatitudeAndLongitude(decimal latitude, decimal longitude)
		{
		object parameters = new {latitude, longitude};
		var sql = DeleteQueryBy + WhereQueryByLatitudeAndLongitude; 
		DataAccessService.PersistObject<TestRepositoryGeneration.DataObjects.BaseRepositories.Address>(sql, parameters);
		}
		public async Task RemoveByLatitudeAndLongitudeAsync(decimal latitude, decimal longitude)
		{
		object parameters = new {latitude, longitude};
		var sql = DeleteQueryBy + WhereQueryByLatitudeAndLongitude; 
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
