using System;


namespace YumaPos.FrontEnd.Infrastructure.DataObjects.PosFdat.Taxes
{
    class Tax
    {
        public Guid TaxId { get; set; }

        public Guid TaxVersionId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset Modified { get; set; }

        public Guid ModifiedBy { get; set; } //EmployeeId 

        public bool IsDeleted { get; set; }
    }
}
