using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedRepositoryGeneration.Models;

namespace VersionedRepositoryGeneration.Interfaces.Repositories
{
    interface ITaxRepository
    {
        Guid Insert(Tax tax);

        Guid Update(Tax tax);

        Guid Remove(Tax tax);

        Tax GetByTaxId(System.Guid taxId, bool? isDeleted = false);
    }
}
