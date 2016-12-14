using System.Collections.Generic;

namespace WCFGenerator.DecoratorGeneration.Infrastructure
{
    internal class MethodInfo
    {
        /// <summary>
        ///     Name of method
        /// </summary>
        public string Name { get; set; }

        public bool IsAsync { get; set; }

        /// <summary>
        ///     Method parameters
        /// </summary>
        public List<ParameterInfo> Parameters { get; set; }

        /// <summary>
        ///     Type of returned value
        /// </summary>
        public string ReturnType { get; set; }
    }

    internal class ParameterInfo
    {
        /// <summary>
        ///     Name of parameter
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Type of parameter
        /// </summary>
        public string Type { get; set; }
    }
}