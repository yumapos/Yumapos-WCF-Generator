using System;

namespace WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes
{
    public class DbIgnoreAttribute : Attribute
    {
        public bool IgnoreOnUpdate { get; set; }
    }
}