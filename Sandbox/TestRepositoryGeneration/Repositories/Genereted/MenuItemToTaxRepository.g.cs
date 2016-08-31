﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using YumaPos.Server.Infrastructure.Repositories
using YumaPos.Server.Data.Sql;



namespace TestRepositoryGeneration
{
public partial class MenuItemToTaxRepository : RepositoryBase,IMenuItemToTaxRepository
{
private MenuItemToTaxСacheRepository _menuItemToTaxСacheRepository;
private MenuItemToTaxVersionRepository _menuItemToTaxVersionRepository;


public MenuItemToTaxRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService)
{
_menuItemToTaxСacheRepository = new MenuItemToTaxСacheRepository(dataAccessService);
_menuItemToTaxVersionRepository = new MenuItemToTaxVersionRepository(dataAccessService);
}

/*
public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax> GetAll(Boolean? isDeleted = false)
{
return _menuItemToTaxСacheRepository.GetAll(Boolean? isDeleted = false);
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>> GetAllAsync(Boolean? isDeleted = false)
{
return await _menuItemToTaxСacheRepository.GetAllAsync(Boolean? isDeleted = false);
}

*/
/*
public void Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.Modified = DateTimeOffset.Now;
menuItemToTax. = _menuItemToTaxVersionRepository.Insert(menuItemToTax);
_menuItemToTaxСacheRepository.Insert(menuItemToTax);
}
public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.Modified = DateTimeOffset.Now;
menuItemToTax. = _menuItemToTaxVersionRepository.InsertAsync(menuItemToTax);
await _menuItemToTaxСacheRepository.InsertAsync(menuItemToTax);
}

*/

}
}
