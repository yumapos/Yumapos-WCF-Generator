using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Infrastructure;

namespace TestRepositoryGeneration.DataObjects.BaseRepositories
{
    [DataAccess(TableName = "EmployeesInRolesSchedule", Identity = true, IsDeleted = false)]
    public class EmployeesInRolesSchedule
    {
        [Key]
        public Guid ScheduleId { get; set; }
        public Guid RoleId { get; set; }
        public Guid UserId { get; set; }
        public Guid StoreId { get; set; }
        public int BusinessDayNumber { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        [DbIgnore]
        public bool PositionIsActive { get; set; }
        public bool IsDeleted { get; set; }
    }
}
