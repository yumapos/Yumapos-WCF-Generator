using System.Collections.Generic;

namespace WCFGenerator.DecoratorGeneration.Infrastructure
{
    internal class MethodInfo
    {
        /// <summary>
        ///     Name of method
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Method parameters
        /// </summary>
        public List<ParameterInfo> Parameters { get; set; }

        /// <summary>
        ///     Type of returned value
        /// </summary>
        public string ReturnTypeName { get; set; }

        /// <summary>
        ///     Type of returned value is nullable
        /// </summary>
        public bool ReturnTypeIsNullble { get; set; }

        /// <summary>
        ///     Method is asynchronous
        /// </summary>
        public bool IsAsync { get; set; }

        /// <summary>
        ///     Pattern for use result of OnEntry method
        /// </summary>
        public string OnEntryResultMap { get; set; }

        /// <summary>
        ///    Type name of parameter generic type
        /// </summary>
        public string GetTaskRetunTypeName()
        {
            //TODO think about any way for get return type
            if (ReturnTypeName == null || !ReturnTypeName.StartsWith("System.Threading.Tasks.Task")) return null;
            var ret = ReturnTypeName.Replace("System.Threading.Tasks.Task", "").TrimStart('<').TrimEnd('>');
            if (ret.Contains("<"))
            {
                ret += '>';
            }
            return ret;
        }
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