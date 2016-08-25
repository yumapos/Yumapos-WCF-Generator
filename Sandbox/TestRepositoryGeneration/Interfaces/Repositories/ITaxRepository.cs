using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedRepositoryGeneration.Models;

namespace VersionedRepositoryGeneration.Interfaces.Repositories
{
    interface IUserRepository
    {
        void Insert(User user);

        User GetByUserId(System.Guid userId, bool? isDeleted);

        IEnumerable<User> GetAll(bool? isDeleted);
    }
}
