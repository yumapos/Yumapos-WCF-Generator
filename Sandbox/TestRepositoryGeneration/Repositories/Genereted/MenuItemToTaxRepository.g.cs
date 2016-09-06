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
using YumaPos.Server.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;
using YumaPos.Server.Data.Sql;



namespace TestRepositoryGeneration
{
public partial class MenuItemToTaxRepository : RepositoryBase, IMenuItemToTaxRepository
{
private MenuItemToTaxCacheRepository _menuItemToTaxCacheRepository;
private MenuItemToTaxVersionRepository _menuItemToTaxVersionRepository;


public MenuItemToTaxRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService)
{
_menuItemToTaxCacheRepository = new MenuItemToTaxCacheRepository(dataAccessService);
_menuItemToTaxVersionRepository = new MenuItemToTaxVersionRepository(dataAccessService);
}

public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax> GetAll(Boolean? isDeleted = false)
{
return _menuItemToTaxCacheRepository.GetAll(isDeleted);
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>> GetAllAsync(Boolean? isDeleted = false)
{
return await _menuItemToTaxCacheRepository.GetAllAsync(isDeleted);
}

public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax> GetByItemId(Guid  itemId, Boolean? isDeleted = false)
{
return _menuItemToTaxCacheRepository.GetByItemId(itemId, isDeleted);
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>> GetByItemIdAsync(Guid  itemId, Boolean? isDeleted = false)
{
return await _menuItemToTaxCacheRepository.GetByItemIdAsync(itemId, isDeleted);
}
public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax> GetByTaxId(Guid  taxId, Boolean? isDeleted = false)
{
return _menuItemToTaxCacheRepository.GetByTaxId(taxId, isDeleted);
}
public async Task<IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax>> GetByTaxIdAsync(Guid  taxId, Boolean? isDeleted = false)
{
return await _menuItemToTaxCacheRepository.GetByTaxIdAsync(taxId, isDeleted);
}

public void Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.Modified = DateTimeOffset.Now;
menuItemToTax.TaxVersionId = _menuItemToTaxVersionRepository.Insert(menuItemToTax);
_menuItemToTaxCacheRepository.Insert(menuItemToTax);
}
public async Task InsertAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.Modified = DateTimeOffset.Now;
menuItemToTax.TaxVersionId = await _menuItemToTaxVersionRepository.InsertAsync(menuItemToTax);
await _menuItemToTaxCacheRepository.InsertAsync(menuItemToTax);
}

public void UpdateByItemId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.Modified = DateTimeOffset.Now;
menuItemToTax.TaxVersionId = _menuItemToTaxVersionRepository.Insert(menuItemToTax);
_menuItemToTaxCacheRepository.UpdateByItemId(menuItemToTax);
}
public async Task UpdateByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.Modified = DateTimeOffset.Now;
menuItemToTax.TaxVersionId = await _menuItemToTaxVersionRepository.InsertAsync(menuItemToTax);
await _menuItemToTaxCacheRepository.UpdateByItemIdAsync(menuItemToTax);
}
public void UpdateByTaxId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.Modified = DateTimeOffset.Now;
menuItemToTax.TaxVersionId = _menuItemToTaxVersionRepository.Insert(menuItemToTax);
_menuItemToTaxCacheRepository.UpdateByTaxId(menuItemToTax);
}
public async Task UpdateByTaxIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.Modified = DateTimeOffset.Now;
menuItemToTax.TaxVersionId = await _menuItemToTaxVersionRepository.InsertAsync(menuItemToTax);
await _menuItemToTaxCacheRepository.UpdateByTaxIdAsync(menuItemToTax);
}

public void RemoveByItemId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.IsDeleted = true;
_menuItemToTaxCacheRepository.UpdateByItemId(menuItemToTax);
}
public async Task RemoveByItemIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.IsDeleted = true;
await _menuItemToTaxCacheRepository.UpdateByItemIdAsync(menuItemToTax);
}
public void RemoveByItemId(Guid  itemId)
{
var result = _menuItemToTaxCacheRepository.GetByItemId(itemId);
foreach (var item in result)
{
item.IsDeleted = true;
UpdateByItemId(item);
}
}
public async Task RemoveByItemIdAsync(Guid  itemId)
{
var result = await _menuItemToTaxCacheRepository.GetByItemIdAsync(itemId);
foreach (var item in result)
{
item.IsDeleted = true;
await UpdateByItemIdAsync(item);
}
}
public void RemoveByTaxId(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.IsDeleted = true;
_menuItemToTaxCacheRepository.UpdateByTaxId(menuItemToTax);
}
public async Task RemoveByTaxIdAsync(YumaPos.Server.Infrastructure.DataObjects.MenuItemToTax menuItemToTax)
{
menuItemToTax.IsDeleted = true;
await _menuItemToTaxCacheRepository.UpdateByTaxIdAsync(menuItemToTax);
}
public void RemoveByTaxId(Guid  taxId)
{
var result = _menuItemToTaxCacheRepository.GetByTaxId(taxId);
foreach (var item in result)
{
item.IsDeleted = true;
UpdateByTaxId(item);
}
}
public async Task RemoveByTaxIdAsync(Guid  taxId)
{
var result = await _menuItemToTaxCacheRepository.GetByTaxIdAsync(taxId);
foreach (var item in result)
{
item.IsDeleted = true;
await UpdateByTaxIdAsync(item);
}
}


}
}
