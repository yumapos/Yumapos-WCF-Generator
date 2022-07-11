using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Infrastructure;

namespace TestRepositoryGeneration.DataObjects.BaseRepositories
{
    [DataAccess(IsDeleted = false, HasSyncState = true, FilterKey1 = "StoreId")]
    public class Store
    {
        [Key]
        public Guid StoreId { get; set; }
        public string Name { get; set; }

        // SyncState is optional filed, can be hidden if used ResetSyncState
        public bool SyncState { get; set; }
    }
}
