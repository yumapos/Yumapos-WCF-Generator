using System.Collections.Generic;

namespace WCFGenerator.DecoratorGeneration.Infrastructure
{
    internal class DecoratorInfo
    {
        public DecoratorInfo()
        {
            RequiredNamespaces = new List<string>();
        }

        /// <summary>
        ///     Type name
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        ///     Namespace of repository class
        /// </summary>
        public string DecoratorNamespace { get; set; }

        /// <summary>
        ///     Full type name
        /// </summary>
        public string ClassFullName
        {
            get
            {
                return DecoratorNamespace + "." + ClassName;
            }
        }

        /// <summary>
        ///     Required namespaces
        /// </summary>
        public List<string> RequiredNamespaces { get; set; }

        /// <summary>
        ///     Implemented Interfaces
        /// </summary>
        public List<string> ImplementedInterfaces { get; set; }

        /// <summary>
        ///     List of method implementation info
        /// </summary>
        public List<MethodImplementationInfo> MethodImplementationInfo { get; set; }
        
        /// <summary>
        ///      List of methods name in castom part decorator class
        /// </summary>
        public List<MethodInfo> CustomRepositoryMethodNames { get; set; }
    } 
}