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
	internal class MenuItems2TaxesVersionRepository : RepositoryBase
	{
		private const string InsertQuery = @"INSERT INTO [MenuItemToTaxVersions]([MenuItems2Taxess].[MenuItemId],[MenuItems2Taxess].[MenuItemVersionId],[MenuItems2Taxess].[Modified],[MenuItems2Taxess].[ModifiedBy],[MenuItems2Taxess].[TaxId],[MenuItems2Taxess].[TaxVersionId],[MenuItems2Taxess].[IsDeleted])
VALUES (@MenuItemId,@MenuItemVersionId,@Modified,@ModifiedBy,@TaxId,@TaxVersionId,@IsDeleted)";

		public MenuItems2TaxesVersionRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public void Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItems2Taxes menuItems2Taxes)
		{
			DataAccessService.InsertObject(menuItems2Taxes, InsertQuery);
		}
		public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItems2Taxes menuItems2Taxes)
		{
			await DataAccessService.InsertObjectAsync(menuItems2Taxes, InsertQuery);
		}


	}
}