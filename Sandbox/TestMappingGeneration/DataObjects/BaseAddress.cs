using System;
using System.Collections.Generic;
using System.Text;
using TestMappingGeneration.Infrastructure;

namespace TestMappingGeneration.DataObjects
{
    [Map]
    public class BaseAddress
    {
        public string City { get; set; }
        public virtual string State { get; set; }
        public virtual string Street { get; set; }
        public string PropertyFromBaseAddress { get; set; }
    }
}
