using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VersionedRepositoryGeneration.Interfaces.Repositories;
using VersionedRepositoryGeneration.Models;

namespace VersionedRepositoryGeneration.Repositories.Services
{
    class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void AddUser(User user)
        {
            _userRepository.Insert(user);
        }
    }
}
