using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.Infrastructure;
using TestRepositoryGeneration.RepositoryInterfaces;

namespace TestRepositoryGeneration.DataObjects.BaseRepositories
{
    /// <summary>
    ///     Repository interface <see cref="IAvailabilityRepository"/>
    ///     Repository <see cref="AvailabilityRepository"/>
    /// </summary>
    [DataAccess(TableName = "Availability")]
    public class Availability
    {
        [Key]
        public Guid AvailabilityId { get; set; }
        public Guid? Id { get; set; }
        public DateTime? TimeFrom { get; set; }
        public DateTime TimeTo { get; set; }
        public int Count { get; set; }
        public int? Number { get; set; }
    }
}
