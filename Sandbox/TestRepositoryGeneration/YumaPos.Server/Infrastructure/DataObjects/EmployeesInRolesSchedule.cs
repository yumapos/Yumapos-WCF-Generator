using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YumaPos.FrontEnd.Infrastructure.Common.DataAccess;

namespace TestRepositoryGeneration.Models
{
    [DataAccess(TableName = "EmployeesInRolesSchedule", FilterKey1 = "UserId", FilterKey2 = "UserId,RoleId,StoreId")]
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
    }
}
