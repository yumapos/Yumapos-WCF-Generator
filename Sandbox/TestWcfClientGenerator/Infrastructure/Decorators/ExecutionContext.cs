using System;
using System.Collections.Generic;
using System.Linq;

namespace TestWcfClientGenerator
{
    public class ExecutionContext
    {
        public ExecutionContext(IDictionary<string, object> values)
        {
            Values = values;
        }

        public Guid ActionId { get; set; }
        public Guid? OrderId { get; set; }

        public IDictionary<string, object> Values { get; private set; }

        public bool IsEmpty()
        {
            return !Values.Any() && OrderId == null;
        }

        public void Fill(IDictionary<string, object> values)
        {
            Values = values;
        }

        public ExecutionContext Copy()
        {
            var ret = new ExecutionContext(new Dictionary<string, object>(Values));
            ret.ActionId = ActionId;
            ret.OrderId = OrderId;
            return ret;
        }
    }
}