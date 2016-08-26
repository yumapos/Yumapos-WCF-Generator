﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YumaPos.Server.Infrastructure.Repositories;


namespace YumaPos.Server.Data.Sql.Menu
{
	 public partial class MenuItemCashRepository : RepositoryBase,IMenuItemRepository
{
#region Constructor

public MenuItemCashRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }

#endregion
#region Fields

#region Fields

internal const string Fields = "Name, Modified, ModifiedBy, TaxesId, MenuCategoryId{columns}"
internal const string Values = "Name, Modified, ModifiedBy, TaxesId, MenuCategoryId{values}"
private const string SelectAllQueryMenuItem = "SELECT [Name],[Modified],[ModifiedBy],[TaxesId],[MenuCategoryId]FROM [MenuItem] {whereTenantId:}";
private const string InsertQuery = "INSERT INTO MenuItems ([Name], [Modified], [ModifiedBy], [TaxesId], [MenuCategoryId]{columns}) OUTPUT INSERTED.MenuCategoryId VALUES (@Name, @Modified, @ModifiedBy, @TaxesId, @,  ;";
private const string SelectQuery = "SELECT [Name],[Modified],[ModifiedBy],[TaxesId],[MenuCategoryId]FROM [MenuItems] ";
private const string DeleteQuery = "DELETE FROM MenuItems";
private const string WhereQueryByMenuCategoryId = "" WHERE [MenuItems].[MenuCategoryId] = @menuCategoryId{andTenantId:[MenuItems]} ";";
private const string UpdateQueryByMenuCategoryId = "
        private const string UpdateQueryByMenuCategoryId = "UPDATE [MenuItems] SET [Name] = @Name, [Modified] = @Modified, [ModifiedBy] = @ModifiedBy, [TaxesId] = @TaxesId, FROM [MenuItems] ";";


#endregion
#region Properties



#endregion
#region Methods

/*
		 public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetAllAsync()
		 {
			 var result = await DataAccessService.GetAsync< YumaPos.Server.Infrastructure.DataObjects.MenuItem> (SelectAllQueryMenuItem, null
			 return result.ToList();
		 }
		 public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetAll()
		 {
			 var result = DataAccessService.Get<YumaPos.Server.Infrastructure.DataObjects.MenuItem> (SelectAllQueryMenuItem, null
			 return result.ToList();
		 }

*/
/*
		 public async Task <Guid>InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemmenuItem)
		 {
			 var result = await DataAccessService.InsertObjectAsync(menuItem,InsertQuery);
			 return (Guid)result;
		 }
		 public GuidInsert(YumaPos.Server.Infrastructure.DataObjects.MenuItemmenuItem)
		 {
			 var result = DataAccessService.InsertObject(menuItem,InsertQuery);
			 return (Guid)result;
		 }

*/
/*

*/
/*

*/
/*

*/


#endregion
}
}
