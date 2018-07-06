using System;

namespace TestRepositoryGeneration.Infrastructure
{
    public interface IDataAccessController
    {
        Guid? EmployeeId { get; set; }
        Tenant Tenant { get; set; }
    }
}