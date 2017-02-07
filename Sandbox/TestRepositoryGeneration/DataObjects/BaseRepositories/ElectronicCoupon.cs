using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.CustomRepositories.BaseRepositories;
using TestRepositoryGeneration.Infrastructure;
using TestRepositoryGeneration.RepositoryInterfaces;

namespace TestRepositoryGeneration.DataObjects.BaseRepositories
{
    /// <summary>
    ///     Repository interface <see cref="IElectronicCouponRepository"/>
    ///     Repository <see cref="ElectronicCouponRepository"/>
    /// </summary>
    [DataRepository(IsDeleted = true, Identity = true)]
    public class ElectronicCoupon
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PrintText { get; set; }
        public Guid? ImageId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool? IsDeleted { get; set; }
        public int? LimitPerOrder { get; set; }
        public int? Priority { get; set; }
        public short? MaxTimesPerCustomer { get; set; }
        public bool IsActive { get; set; }
    }
}