using System;

namespace TestRepositoryGeneration.Infrastructure
{
    public class DbIgnoreAttribute : Attribute
    {
        public string DbType { get; set; }
    }
}