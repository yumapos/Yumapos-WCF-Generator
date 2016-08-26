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
using YumaPos.Server.Data.Sql;
using YumaPos.Server.Infrastructure.Repositories;


namespace TestRepositoryGeneration
{
public class MenuItemRepository : RepositoryBase,IMenuItemRepository
{
public MenuItemRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService)
{
_menuItemСacheRepository = new MenuItemСacheRepository(dataAccessService);
_menuItemVersionRepository = new MenuItemVersionRepository(dataAccessService);
}

private MenuItemСacheRepository _menuItemСacheRepository;
private MenuItemVersionRepository _menuItemVersionRepository;


public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetAll(bool? isDeleted = false)
{
return _menuItemСacheRepository.GetAll();
}

public Guid Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
menuItem.Modified = DateTimeOffset.Now;
menuItem.ItemVersionId = _menuItemVersionRepository.Insert(menuItem);
_menuItemСacheRepository.Insert(menuItem);
return menuItem.ItemVersionId;
}

/*
public YumaPos.Server.Infrastructure.DataObjects.MenuItem GetBy(System.Guid MenuCategoryId, bool? isDeleted = false)
{
return _menuItemСacheRepository GetBy(System.Guid MenuCategoryId);
}

*/
/*
public Guid Update(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem;
{
menuItem.Modified = DateTimeOffset.Now;
menuItem.ItemVersionId = _menuItemVersionRepositoryInsert(menuItem);
_menuItemСacheRepository.Update(menuItem);
return menuItem.ItemVersionId;
}

*/
/*
public Guid Remove(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem;
{
menuItem.IsDeleted = true;
_menuItemСacheRepository.Update(menuItem);

*/

}
}
