using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFGenerator.MappingsGenerator.Infrastructure
{
    public enum KindOfProperty
    {
        AttributeClass = 1,
        CollectionAttributeClasses = 2,
        FunctionAttribute = 3,
        None = 9
    }
}
