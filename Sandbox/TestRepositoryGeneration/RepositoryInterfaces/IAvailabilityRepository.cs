using System.Collections.Generic;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface IAvailabilityRepository : IRepository<Availability>
    {
        Task InsertManyAsync(IEnumerable<Availability> availability);
    }
}
