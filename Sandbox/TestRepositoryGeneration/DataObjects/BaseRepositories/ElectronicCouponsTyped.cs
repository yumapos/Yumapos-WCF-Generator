﻿using System;
using System.ComponentModel.DataAnnotations;
using TestRepositoryGeneration.CustomRepositories.BaseRepositories;
using TestRepositoryGeneration.Infrastructure;
using TestRepositoryGeneration.RepositoryInterfaces;

namespace TestRepositoryGeneration.DataObjects.BaseRepositories
{
    /// <summary>
    ///     Repository interface <see cref="IElectronicCouponsTypedRepository"/>
    ///     Repository <see cref="ElectronicCouponsTypedRepository"/>
    /// </summary>
    [DataRepository(TableName = "ElectronicCouponsTyped", IsDeleted = false, Identity = false)]
    [DataArchive(TableName = "archive.electronic_coupons_typed", IsDeleted = false, Identity = false)]
    public class ElectronicCouponsTyped : ElectronicCoupon
    {
        [Key]
        public int ElectronicCouponsId { get; set; }
        public Guid ElectronicCouponsPresetId { get; set; }
        public bool IsPromotionalCampaign { get; set; }
    }
}
