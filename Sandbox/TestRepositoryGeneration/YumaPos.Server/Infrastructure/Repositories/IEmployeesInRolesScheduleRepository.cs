using System;
using System.Collections.Generic;
using TestRepositoryGeneration.Models;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IEmployeesInRolesScheduleRepository : IRepository<EmployeesInRolesSchedule>
    {
        Guid Insert(EmployeesInRolesSchedule item);
        void RemoveByUserIdAndRoleIdAndStoreId(Guid userId, Guid roleId, Guid storeId);
        void RemoveByUserIdAndRoleIdAndStoreId(EmployeesInRolesSchedule item);
        void UpdateByUserIdAndRoleIdAndStoreId(EmployeesInRolesSchedule item);
    }
}
