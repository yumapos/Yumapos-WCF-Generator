﻿using System;

namespace WCFGenerator.RepositoriesGeneration.Yumapos.Infrastructure.Clone.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    internal sealed class DataAccessAttribute : Attribute
    {
        public string PrimaryKey { get; set; }

        public string TableName { get; set; }

        public string TableVersion { get; set; }

        public string FilterKey1 { get; set; }

        public string FilterKey2 { get; set; }

        public string FilterKey3 { get; set; }

        public bool IsDeleted { get; set; }
    }
}