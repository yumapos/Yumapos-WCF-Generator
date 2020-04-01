using System.Threading.Tasks;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    public interface IRepository<T>
    {
    }

    public interface IOfflineSerializeRepository<T>
    {
        Task InsertAsync(T obj);
    }
}
