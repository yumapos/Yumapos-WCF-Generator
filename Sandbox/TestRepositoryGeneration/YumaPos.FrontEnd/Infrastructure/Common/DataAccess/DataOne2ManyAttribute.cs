using System;

namespace YumaPos.FrontEnd.Infrastructure.Common.DataAccess
{
    internal class DataOne2ManyAttribute : Attribute
    {
        public string OneToManyEntytyType { get; set; }
        public string EntityKey { get; set; }
    }
}