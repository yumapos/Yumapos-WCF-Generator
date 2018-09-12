using System;
using System.Collections.Generic;
using System.Text;
using TestMappingGeneration.Infrastructure;

namespace TestMappingGeneration.DataObjects
{
    [Map]
    public class ClassWithoutPair
    {
        public string Property1 { get; set; }
    }
}
