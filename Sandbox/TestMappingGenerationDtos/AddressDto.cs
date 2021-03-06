﻿using System;
using TestMappingGeneration.Enums;
using TestMappingGeneration.Infrastructure;

namespace TestMappingGenerationDtos
{
    [Map]
    public class AddressDto
    {
        public Guid Id { get; set; }
        public CountryISOCodes2 Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Street { get; set; }
        public string Building { get; set; }
        public string ZipCode { get; set; }
        public decimal? Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime SomeDateTime { get; set; }
        public DateTimeOffset SomeTime { get; set; }
        public DateTime SomeTimeOffset { get; set; }
        public DateTimeOffset SomeTimeNullable { get; set; }
        public DateTime? SomeTimeOffsetNullable { get; set; }

    }
}
