using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionedRepositoryGeneration.Models
{
    class MenuItemToTax
    {
        public Guid ItemId { get; set; }

        public Guid ItemVersionId { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; } 

        public Guid TaxId { get; set; }

        public Guid TaxVersionId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
