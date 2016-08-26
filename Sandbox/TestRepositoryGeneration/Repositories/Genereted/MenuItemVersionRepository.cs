﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;

namespace YumaPos.Server.Data.Sql.Menu
{
public partial class MenuItemVersionRepository : RepositoryBase
{
#region Constructor

public MenuItemVersionRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }

#endregion
#region Methods

public Guid Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var InsertQuery = @"INSERT INTO MenuItemVersionRepository([Name],[Modified],[ModifiedBy],[TaxesId],[MenuCategoryId])
OUTPUT INSERTED.VALUES (@Name,@Modified,@ModifiedBy,@TaxesId,@MenuCategoryId)";
return (Guid)DataAccessService.InsertObject(menuItem, InsertQuery);
}


#endregion
}
}
