using System;
using System.Threading.Tasks;
using TestRepositoryGeneration.DataObjects;
using TestRepositoryGeneration.DataObjects.BaseRepositories;

namespace TestRepositoryGeneration.RepositoryInterfaces
{
    interface IEmployeesInRolesScheduleRepository : IRepository<EmployeesInRolesSchedule>
    {
        Task<Guid> InsertAsync(EmployeesInRolesSchedule item);
        Task<EmployeesInRolesSchedule> GetByScheduleIdAsync(Guid scheduleId, bool? isDeleted);
    }
}
