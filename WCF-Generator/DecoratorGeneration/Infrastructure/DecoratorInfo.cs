using System.Collections.Generic;

namespace WCFGenerator.DecoratorGeneration.Infrastructure
{
    internal class DecoratorInfo
    {
        public DecoratorInfo()
        {
            RequiredNamespaces = new List<string>();
            MethodInfos = new List<MethodInfo>();
            RequiredNamespaces = new List<string>();
        }

        public string Namespace { get; set; }

        // Decorated class
        public string DecoratedClassTypeShortName { get; set; }
        public string DecoratedClassTypeFullName { get; set; }

        // Decorator class
        public string DecoratorClassTypeShortName { get; set; }
        public List<MethodInfo> MethodInfos { get; set; }
        public List<string> RequiredNamespaces { get; set; }

        public bool OnEntryExist { get; set; }
        public bool OnExitExist { get; set; }
        public bool OnExceptionExist { get; set; }
        public bool OnFinallyExist { get; set; }
    } 
}