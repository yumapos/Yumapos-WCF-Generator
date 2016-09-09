using System;
using System.Collections.Generic;
using TestRepositoryGeneration.Models;
using YumaPos.FrontEnd.Infrastructure.Repositories;
using YumaPos.Server.Infrastructure.DataObjects;

namespace YumaPos.Server.Infrastructure.Repositories
{
    interface IEmployeesInRolesScheduleRepository : IRepository<EmployeesInRolesSchedule>
    {
        void Insert(EmployeesInRolesSchedule item);
        void UpdateByUserIdAndRoleIdAndStoreId(Guid userId, Guid roleId, Guid storeId);
    }
}
