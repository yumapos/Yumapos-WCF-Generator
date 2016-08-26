using System;

namespace YumaPos.FrontEnd.Infrastructure.Common.DataAccess
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class DataFilterAttribute : Attribute
    {
        public object DefaultValue { get; set; }
    }
}