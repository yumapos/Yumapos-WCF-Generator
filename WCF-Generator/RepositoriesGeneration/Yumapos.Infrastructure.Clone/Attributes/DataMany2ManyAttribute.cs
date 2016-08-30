using System;

namespace WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes
{
    internal class DataMany2ManyAttribute : Attribute
    {
        public object Type { get; set; }
        public string Name { get; set; }
    }
}