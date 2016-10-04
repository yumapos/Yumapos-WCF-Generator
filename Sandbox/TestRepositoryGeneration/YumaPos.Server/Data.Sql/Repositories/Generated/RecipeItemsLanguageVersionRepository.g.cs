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


namespace YumaPos.Server.Data.Sql
{
	internal class RecipeItemsLanguageVersionRepository : RepositoryBase
	{
		private const string InsertQuery = @"INSERT INTO [RecipieItemsLanguagesVersions]([RecipieItemsLanguages].[ItemId],[RecipieItemsLanguages].[Language],[RecipieItemsLanguages].[Description],[RecipieItemsLanguages].[Name],[RecipieItemsLanguages].[IsDeleted],[RecipieItemsLanguages].[Modified],[RecipieItemsLanguages].[ModifiedBy],[RecipieItemsLanguages].[ItemIdVersionId]{columns})
VALUES (@ItemId,@Language,@Description,@Name,@IsDeleted,@Modified,@ModifiedBy,@ItemIdVersionId{values})";
		private const string SelectByQuery = @"SELECT [RecipieItemsLanguages].[ItemId],[RecipieItemsLanguages].[Language],[RecipieItemsLanguages].[Description],[RecipieItemsLanguages].[Name],[RecipieItemsLanguages].[IsDeleted],[RecipieItemsLanguages].[Modified],[RecipieItemsLanguages].[ModifiedBy],[RecipieItemsLanguages].[ItemIdVersionId] FROM [RecipieItemsLanguages] ";
		private const string WhereQueryByItemIdVersionId = @"WHERE [RecipieItemsLanguages].[ItemIdVersionId] = @ItemIdVersionId{andTenantId:[RecipieItemsLanguages]} ";
		private const string AndWithFilterData = "AND [RecipieItemsLanguages].[IsDeleted] = @IsDeleted";

		public RecipeItemsLanguageVersionRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public void Insert(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			DataAccessService.InsertObject(recipeItemsLanguage, InsertQuery);
		}
		public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.RecipeItemsLanguage recipeItemsLanguage)
		{
			await DataAccessService.InsertObjectAsync(recipeItemsLanguage, InsertQuery);
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



	}
}
