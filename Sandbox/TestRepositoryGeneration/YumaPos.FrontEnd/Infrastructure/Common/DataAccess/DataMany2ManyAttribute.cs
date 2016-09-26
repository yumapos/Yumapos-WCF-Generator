using System;

namespace YumaPos.FrontEnd.Infrastructure.Common.DataAccess
{
    internal class DataMany2ManyAttribute : Attribute
    {
        public string EntityType { get; set; }
        public string ManyToManyEntytyType { get; set; }
    }
}