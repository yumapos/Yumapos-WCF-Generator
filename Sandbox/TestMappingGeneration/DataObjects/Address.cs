using System;
using System.Collections.Generic;
using System.Text;
using TestMappingGeneration.Enums;
using TestMappingGeneration.Infrastructure;

namespace TestMappingGeneration.DataObjects
{
    [Map]
    public class Address : BaseAddress
    {
        public Guid Id { get; set; }
        public int Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string ZipCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        [MapIgnore]
        public bool IsDeleted { get; set; }
    }
}
