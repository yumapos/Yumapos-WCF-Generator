using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedRepositoryGeneration.Models;

namespace VersionedRepositoryGeneration.Interfaces.Repositories
{
    interface IMenuItemRepository : IRepository<MenuItem>
    {
        Guid Insert(MenuItem menuItem);

        Guid Update(MenuItem menuItem);

        Guid Remove(MenuItem menuItem);

        MenuItem GetByMenuItemId(System.Guid menuItemId, bool? isDeleted);

        IEnumerable<MenuItem> GetAll(bool? isDeleted);
    }
}
