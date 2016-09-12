using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class ParameterInfo
    {
        public ParameterInfo(string name, string typeName, string defaultValue = null)
        {
            Name = name;
            TypeName = typeName.Trim('?'); // filter key can not be nullable Guid, Int...
            DefaultValue = defaultValue;
        }

        public string Name { get; set; }
        public string TypeName { get; set; }
        public string DefaultValue { get; set; }
    }
}
