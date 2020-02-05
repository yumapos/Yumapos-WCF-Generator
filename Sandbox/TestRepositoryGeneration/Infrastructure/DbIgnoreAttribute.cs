using System;

namespace TestRepositoryGeneration.Infrastructure
{
    public class DbIgnoreAttribute : Attribute
    {
        public bool IgnoreOnUpdate { get; set; }
    }
}