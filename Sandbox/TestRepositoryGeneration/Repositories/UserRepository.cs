using System.Collections.Generic;
using System.Linq;
using VersionedRepositoryGeneration.Interfaces;
using VersionedRepositoryGeneration.Interfaces.Repositories;
using VersionedRepositoryGeneration.Models;

namespace VersionedRepositoryGeneration.Repositories
{
    class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository(IDataAccessService dataAccessService) : base(dataAccessService) {}

        public void Insert(User user)
        {
            var InsertQuery = "INSERT INTO [dbo].[Users] ([UserId],[Name],[Created],[IsDeleted]) " +
                              "VALUES (@UserId, @Name, @Created, @IsDeleted)";

            DataAccessService.InsertObject(user, InsertQuery);
        }

        public User GetByUserId(System.Guid userId, bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [UserId],[Name],[Created],[IsDeleted] FROM[dbo].[Users] Where [UserId] = @userId";

            var result = DataAccessService.Get<User>(SelectQuery, new { userId });
            return result.FirstOrDefault();
        }

        public IEnumerable<User> GetAll(bool? isDeleted = false)
        {
            var SelectQuery = "SELECT [UserId],[Name],[Created],[IsDeleted] FROM[dbo].[Users]";

            var result = DataAccessService.Get<User>(SelectQuery, null);
            return result.ToList();
        }

    }
}
