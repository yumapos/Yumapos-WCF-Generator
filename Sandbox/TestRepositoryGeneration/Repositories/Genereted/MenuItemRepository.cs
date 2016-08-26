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
	 public partial class MenuItemRepository : RepositoryBase,IMenuItemRepository
{
#region Constructor

public MenuItemRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService)
{
_menuItemCashRepository = new MenuItemCashRepository(dataAccessService);
_menuItemVersionRepository = new MenuItemVersionRepository(dataAccessService);
}


#endregion
#region Fields

private MenuItemCashRepository _menuItemCashRepository;
private MenuItemVersionRepository _menuItemVersionRepository;


#endregion
#region Properties



#endregion
#region Methods

/*
public IEnumerable<YumaPos.Server.Infrastructure.DataObjects.MenuItem> GetAll(bool? isDeleted = false)
{
return _menuItemCashRepository.GetAll();
}

*/
/*
public Guid Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
menuItem.Modified = DateTimeOffset.Now;
menuItem.ItemVersionId = _menuItemVersionRepositoryInsert(menuItem);
_menuItemCashRepository.Insert(menuItem);
return menuItem.ItemVersionId;
}

*/
/*
public YumaPos.Server.Infrastructure.DataObjects.MenuItem GetBy(System.Guid MenuCategoryId, bool? isDeleted = false)
{
return _menuItemCashRepository GetBy(System.Guid MenuCategoryId);
}

*/
/*
public Guid Update(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem;
{
menuItem.Modified = DateTimeOffset.Now;
menuItem.ItemVersionId = _menuItemVersionRepositoryInsert(menuItem);
_menuItemCashRepository.Update(menuItem);
return menuItem.ItemVersionId;
}

*/
/*
public Guid Remove(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem;
{
menuItem.IsDeleted = true;
_menuItemCashRepository.Update(menuItem);

*/


#endregion
}
}
