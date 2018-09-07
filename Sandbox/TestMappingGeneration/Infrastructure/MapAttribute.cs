using System;
using System.Collections.Generic;
using System.Text;

namespace TestMappingGeneration.Infrastructure
{
    public class MapAttribute : Attribute
    {
        public string Name { get; set; }
        public string FunctionTo { get; set; }
        public string FunctionFrom { get; set; }

    }

    public class MapIgnoreAttribute : Attribute { }
}
