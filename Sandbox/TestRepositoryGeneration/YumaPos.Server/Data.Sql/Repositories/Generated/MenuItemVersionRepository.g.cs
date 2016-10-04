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


namespace YumaPos.Server.Data.Sql.Menu
{
	internal class MenuItemVersionRepository : RepositoryBase
	{
		private const string InsertQuery = @"INSERT INTO [RecipieItemVersions]([RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[CategoryId]{columns})
VALUES (@MenuItemId,@MenuItemVersionId,@IsDeleted,@CategoryId{values})
INSERT INTO [MenuItemVersions]([MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[Name],[MenuItems].[Modified],[MenuItems].[ModifiedBy],[MenuItems].[TaxIds],[MenuItems].[MenuCategoryId]{columns})
VALUES (@MenuItemId,@MenuItemVersionId,@Name,@Modified,@ModifiedBy,@TaxIds,@MenuCategoryId{values})";
		private const string SelectByQuery = @"SELECT [MenuItems].[MenuItemId],[MenuItems].[MenuItemVersionId],[MenuItems].[Name],[MenuItems].[Modified],[MenuItems].[ModifiedBy],[MenuItems].[TaxIds],[MenuItems].[MenuCategoryId],[RecipieItems].[ItemId],[RecipieItems].[ItemVersionId],[RecipieItems].[IsDeleted],[RecipieItems].[CategoryId] FROM [MenuItems] INNER JOIN [RecipieItems] ON [MenuItems].[MenuItemId] = [RecipieItems].[ItemId] ";
		private const string WhereQueryByMenuItemVersionId = @"WHERE [MenuItems].[MenuItemVersionId] = @MenuItemVersionId{andTenantId:[MenuItems]} ";
		private const string AndWithFilterData = "AND [RecipieItems].[IsDeleted] = @IsDeleted";

		public MenuItemVersionRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public void Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
			DataAccessService.InsertObject(menuItem, InsertQuery);
		}
		public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
		{
			await DataAccessService.InsertObjectAsync(menuItem, InsertQuery);
		}

		public YumaPos.Server.Infrastructure.DataObjects.MenuItem GetByMenuItemVersionId(System.Guid menuItemVersionId, bool? isDeleted = false)
		{
			object parameters = new { menuItemVersionId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuItemVersionId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters);
			return result.FirstOrDefault();
		}
		public async Task<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetByMenuItemVersionIdAsync(System.Guid menuItemVersionId, bool? isDeleted = false)
		{
			object parameters = new { menuItemVersionId, isDeleted };
			var sql = SelectByQuery + WhereQueryByMenuItemVersionId;
			if (isDeleted.HasValue)
			{
				sql = sql + AndWithFilterData;
			}
			var result = (await DataAccessService.GetAsync<YumaPos.Server.Infrastructure.DataObjects.MenuItem>(sql, parameters));
			return result.FirstOrDefault();
		}



	}
}