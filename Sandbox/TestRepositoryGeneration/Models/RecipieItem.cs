using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedRepositoryGeneration.Models.Attributes;

namespace VersionedRepositoryGeneration.Models
{
   // [DataAccess]
    public class RecipieItem
    {
        public Guid ItemId { get; set; }

        public Guid ItemVersionId { get; set; }

        public bool IsDeleted { get; set; }

    }
}
