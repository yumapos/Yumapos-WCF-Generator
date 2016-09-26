using System;

namespace YumaPos.Server.Infrastructure.DataObjects
{
    public class MenuCategory
    {
        public Guid MenuCategoryId { get; set; }

        public Guid MenuCategoryVersionId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; } //EmployeeId 

        public bool IsDeleted { get; set; }
    }
}
