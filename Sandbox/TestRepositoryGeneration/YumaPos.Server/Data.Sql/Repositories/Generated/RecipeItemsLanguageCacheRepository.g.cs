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


namespace YumaPos.Server.Data.Sql
{
	internal partial class RecipeItemsLanguageCacheRepository : RepositoryBase, IRecipeItemsLanguageRepository
	{
		private const string Fields = @"[RecipieItemsLanguages].[ItemId],[RecipieItemsLanguages].[Language],[RecipieItemsLanguages].[Description],[RecipieItemsLanguages].[Name],[RecipieItemsLanguages].[IsDeleted],[RecipieItemsLanguages].[Modified],[RecipieItemsLanguages].[ModifiedBy],[RecipieItemsLanguages].[ItemIdVersionId]{columns}";
		private const string Values = @"@ItemId,@Language,@Description,@Name,@IsDeleted,@Modified,@ModifiedBy,@ItemIdVersionId{values}";
		private const string SelectAllQuery = @"SELECT [RecipieItemsLanguages].[ItemId],[RecipieItemsLanguages].[Language],[RecipieItemsLanguages].[Description],[RecipieItemsLanguages].[Name],[RecipieItemsLanguages].[IsDeleted],[RecipieItemsLanguages].[Modified],[RecipieItemsLanguages].[ModifiedBy],[RecipieItemsLanguages].[ItemIdVersionId] FROM [RecipieItemsLanguages]  {whereTenantId:[RecipieItemsLanguages]} ";
		private const string SelectByQuery = @"SELECT [RecipieItemsLanguages].[ItemId],[RecipieItemsLanguages].[Language],[RecipieItemsLanguages].[Description],[RecipieItemsLanguages].[Name],[RecipieItemsLanguages].[IsDeleted],[RecipieItemsLanguages].[Modified],[RecipieItemsLanguages].[ModifiedBy],[RecipieItemsLanguages].[ItemIdVersionId] FROM [RecipieItemsLanguages] ";
		private const string InsertQuery = @"INSERT INTO [RecipieItemsLanguages]([RecipieItemsLanguages].[ItemId],[RecipieItemsLanguages].[Language],[RecipieItemsLanguages].[Description],[RecipieItemsLanguages].[Name],[RecipieItemsLanguages].[IsDeleted],[RecipieItemsLanguages].[Modified],[RecipieItemsLanguages].[ModifiedBy],[RecipieItemsLanguages].[ItemIdVersionId]{columns})  VALUES(@ItemId,@Language,@Description,@Name,@IsDeleted,@Modified,@ModifiedBy,@ItemIdVersionId{values}) ";
		private const string UpdateQueryBy = @"UPDATE [RecipieItemsLanguages] SET [RecipieItemsLanguages].[ItemId] = @ItemId,[RecipieItemsLanguages].[Language] = @Language,[RecipieItemsLanguages].[Description] = @Description,[RecipieItemsLanguages].[Name] = @Name,[RecipieItemsLanguages].[IsDeleted] = @IsDeleted,[RecipieItemsLanguages].[Modified] = @Modified,[RecipieItemsLanguages].[ModifiedBy] = @ModifiedBy,[RecipieItemsLanguages].[ItemIdVersionId] = @ItemIdVersionId FROM [RecipieItemsLanguages] ";
		private const string DeleteQueryBy = @"DELETE FROM [RecipieItemsLanguages] ";
		private const string SelectIntoTempTable = @"DECLARE @Temp TABLE (ItemId uniqueidentifier);INSERT INTO @Temp SELECT [RecipieItemsLanguages].[ItemIdAndLanguage] FROM [RecipieItemsLanguages] ";
		private const string WhereQueryByItemIdAndLanguage = "WHERE [RecipieItemsLanguages].[ItemId] = @ItemId AND [RecipieItemsLanguages].[Language] = @Language{andTenantId:[RecipieItemsLanguages]} ";
		private const string WhereQueryByItemIdVersionId = "WHERE [RecipieItemsLanguages].[ItemIdVersionId] = @ItemIdVersionId{andTenantId:[RecipieItemsLanguages]} ";
		private const string WhereQueryByItemId = "WHERE [RecipieItemsLanguages].[ItemId] = @ItemId{andTenantId:[RecipieItemsLanguages]} ";
		private const string AndWithFilterData = "AND [RecipieItemsLanguages].[IsDeleted] = @IsDeleted";


		public RecipeItemsLanguageCacheRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		/*
		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage> GetAll(bool? isDeleted = false)
		{
		object parameters = new {isDeleted};
		var sql = SelectAllQuery;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithFilterData;
		}
		var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters).ToList();
		return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>> GetAllAsync(bool? isDeleted = false)
		{
		object parameters = new {isDeleted};
		var sql = SelectAllQuery;
		if (isDeleted.HasValue)
		{
		sql = sql + AndWithFilterData;
		}
		var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters));
		return result.ToList();
		}

		*/
		public YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage GetByItemIdAndLanguage(System.Guid itemId, string language, bool? isDeleted = false)
		{
			object parameters = new { itemId, language, isDeleted };
			var sql = SelectByQuery + WhereQueryByItemIdAndLanguage;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage> GetByItemIdAndLanguageAsync(System.Guid itemId, string language, bool? isDeleted = false)
		{
			object parameters = new { itemId, language, isDeleted };
			var sql = SelectByQuery + WhereQueryByItemIdAndLanguage;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters));
			return result.FirstOrDefault();
		}

		public YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage GetByItemIdVersionId(System.Guid itemIdVersionId, bool? isDeleted = false)
		{
			object parameters = new { itemIdVersionId, isDeleted };
			var sql = SelectByQuery + WhereQueryByItemIdVersionId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage> GetByItemIdVersionIdAsync(System.Guid itemIdVersionId, bool? isDeleted = false)
		{
			object parameters = new { itemIdVersionId, isDeleted };
			var sql = SelectByQuery + WhereQueryByItemIdVersionId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters));
			return result.FirstOrDefault();
		}


		public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage> GetByItemId(System.Guid itemId, bool? isDeleted = false)
		{
			object parameters = new { itemId, isDeleted };
			var sql = SelectByQuery + WhereQueryByItemId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters);
			return result.ToList();
		}
		public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>> GetByItemIdAsync(System.Guid itemId, bool? isDeleted = false)
		{
			object parameters = new { itemId, isDeleted };
			var sql = SelectByQuery + WhereQueryByItemId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters));
			return result.ToList();
		}


		public void Insert(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			DataAccessService.InsertObject(recipeItemsLanguage, InsertQuery);
		}
		public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			await DataAccessService.InsertObjectAsync(recipeItemsLanguage, InsertQuery);
		}

		/*
		public void UpdateByItemIdAndLanguage(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
		var sql = UpdateQueryBy + WhereQueryByItemIdAndLanguage; 
		DataAccessService.PersistObject(recipeItemsLanguage, sql);
		}
		public async Task UpdateByItemIdAndLanguageAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
		var sql = UpdateQueryBy + WhereQueryByItemIdAndLanguage; 
		await DataAccessService.PersistObjectAsync(recipeItemsLanguage, sql);
		}


		*/
		public void UpdateByItemId(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			var sql = UpdateQueryBy + WhereQueryByItemId;
			DataAccessService.PersistObject(recipeItemsLanguage, sql);
		}
		public async Task UpdateByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			var sql = UpdateQueryBy + WhereQueryByItemId;
			await DataAccessService.PersistObjectAsync(recipeItemsLanguage, sql);
		}


		/*
		public void RemoveByItemIdAndLanguage(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
		var sql = DeleteQueryBy + WhereQueryByItemIdAndLanguage; 
		DataAccessService.PersistObject(recipeItemsLanguage, sql);
		}
		public async Task RemoveByItemIdAndLanguageAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
		var sql = DeleteQueryBy + WhereQueryByItemIdAndLanguage; 
		await DataAccessService.PersistObjectAsync(recipeItemsLanguage, sql);
		}

		public void RemoveByItemIdAndLanguage(System.Guid itemId, string language)
		{
		object parameters = new {itemId, language};
		var sql = DeleteQueryBy + WhereQueryByItemIdAndLanguage; 
		DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters);
		}
		public async Task RemoveByItemIdAndLanguageAsync(System.Guid itemId, string language)
		{
		object parameters = new {itemId, language};
		var sql = DeleteQueryBy + WhereQueryByItemIdAndLanguage; 
		await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters);
		}


		*/
		public void RemoveByItemId(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			var sql = DeleteQueryBy + WhereQueryByItemId;
			DataAccessService.PersistObject(recipeItemsLanguage, sql);
		}
		public async Task RemoveByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			var sql = DeleteQueryBy + WhereQueryByItemId;
			await DataAccessService.PersistObjectAsync(recipeItemsLanguage, sql);
		}

		public void RemoveByItemId(System.Guid itemId)
		{
			object parameters = new { itemId };
			var sql = DeleteQueryBy + WhereQueryByItemId;
			DataAccessService.PersistObject<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters);
		}
		public async Task RemoveByItemIdAsync(System.Guid itemId)
		{
			object parameters = new { itemId };
			var sql = DeleteQueryBy + WhereQueryByItemId;
			await DataAccessService.PersistObjectAsync<YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage>(sql, parameters);
		}



	}
}