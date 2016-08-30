﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using YumaPos.Server.Data.Sql;


namespace TestRepositoryGeneration
{
public partial class MenuItemVersionRepository : RepositoryBase
{
public MenuItemVersionRepository(YumaPos.FrontEnd.Infrastructure.Configuration.IDataAccessService dataAccessService) : base(dataAccessService) { }
public Guid Insert(YumaPos.Server.Infrastructure.DataObjects.MenuItem menuItem)
{
var InsertQuery = @"DECLARE @TempPKTable TABLE ( );
DECLARE @TempPK ;
INSERT INTO RecipieItems([ItemId],[ItemVersionId],[IsDeleted],[CategoryId])
OUTPUT INSERTED. INTO @TempPKTable
VALUES (@ItemId,@ItemVersionId,@IsDeleted,@CategoryId)
SELECT @TempPK =  FROM @TempPKTable
INSERT INTO MenuItemVersionRepository([Name],[Modified],[ModifiedBy],[TaxesId],[MenuCategoryId])
VALUES (@Name,@Modified,@ModifiedBy,@TaxesId,@MenuCategoryId)
SELECT  FROM @TempPKTable
";
return (Guid)DataAccessService.InsertObject(menuItem, InsertQuery);
}

}
}
