using System;

namespace TestRepositoryGeneration.Infrastructure
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DataAccessAttribute : Attribute
    {
        public bool Identity { get; set; }

        public string TableName { get; set; }

        public string PostgresTableName { get; set; }

        public string TableVersion { get; set; }

        public string FilterKey1 { get; set; }

        public string FilterKey2 { get; set; }

        public string FilterKey3 { get; set; }

        public bool IsDeleted { get; set; }

    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class DataRepositoryAttribute : Attribute
    {
        public bool Identity { get; set; }

        public string TableName { get; set; }

        public string PostgresTableName { get; set; }

        public string TableVersion { get; set; }

        public string FilterKey1 { get; set; }

        public string FilterKey2 { get; set; }

        public string FilterKey3 { get; set; }

        public bool IsDeleted { get; set; }
    }
}