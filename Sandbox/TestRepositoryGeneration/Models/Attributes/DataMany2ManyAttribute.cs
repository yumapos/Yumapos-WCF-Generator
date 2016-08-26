using System;

namespace YumaPos.FrontEnd.Infrastructure.Common.DataAccess
{
    internal class DataMany2ManyAttribute : Attribute
    {
        public object Type { get; set; }
        public string Name { get; set; }
    }
}