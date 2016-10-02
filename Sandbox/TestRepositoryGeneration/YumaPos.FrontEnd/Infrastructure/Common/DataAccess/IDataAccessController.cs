using System;
using YumaPos.FrontEnd.Infrastructure.DataObjects;

namespace YumaPos.FrontEnd.Infrastructure.Common.DataAccess
{
    public interface IDataAccessController
    {
        bool ReadOnlyMode { get; set; }

        Tenant Tenant { get; set; }

        Guid? StoreId { get; set; }
        Guid? EmployeeId { get; set; }
    }
}