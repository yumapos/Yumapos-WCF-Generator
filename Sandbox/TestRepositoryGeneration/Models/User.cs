using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionedRepositoryGeneration.Models
{
    public class User
    {
        public Guid UserId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Created { get; set; }

        public bool IsDeleted { get; set; }
    }
}
