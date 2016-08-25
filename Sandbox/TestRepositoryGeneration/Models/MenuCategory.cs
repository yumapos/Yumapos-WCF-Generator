﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VersionedRepositoryGeneration.Models
{
    class MenuCategory
    {
        public Guid MenuCategoryId { get; set; }

        public Guid MenuCategoryVersionId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; } //EmployeeId 

        public bool IsDeleted { get; set; }
    }
}
