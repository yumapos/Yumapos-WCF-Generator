﻿//------------------------------------------------------------------------------
// <auto-generated>
//	 This code was generated from a template.
//
//	 Manual changes to this file may cause unexpected behavior in your application.
//	 Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Threading.Tasks;
using YumaPos.Server.Data.Sql;



namespace TestRepositoryGeneration
{
	internal class MenuItemToTaxVersionRepository : RepositoryBase
	{
		private const string InsertQuery = @"INSERT INTO MenuItemToTaxVersionRepository([ItemId],[ItemVersionId],[Modified],[ModifiedBy],[TaxId],[TaxVersionId],[IsDeleted])
OUTPUT INSERTED.TaxVersionIdVALUES (@ItemId,@ItemVersionId,@Modified,@ModifiedBy,@TaxId,@TaxVersionId,@IsDeleted)";
		public MenuItemToTaxVersionRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
		public Guid Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			var res = DataAccessService.InsertObject(menuItemToTax, InsertQuery);
			return (Guid)res;
		}
		public async Task<Guid> InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
		{
			var res = await DataAccessService.InsertObjectAsync(menuItemToTax, InsertQuery);
			return (Guid)res;
		}

	}
}
