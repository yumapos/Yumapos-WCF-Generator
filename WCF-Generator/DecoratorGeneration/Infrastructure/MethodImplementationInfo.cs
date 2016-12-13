namespace WCFGenerator.DecoratorGeneration.Infrastructure
{
    internal class MethodImplementationInfo : MethodInfo
    {
        public MethodImplementationInfo()
        {
            RequiresImplementation = false;
        }

        /// <summary>
        ///     Requires of Implementation 
        /// </summary>
        public bool RequiresImplementation { get; set; }

    }

    internal class MethodInfo
    {
        /// <summary>
        ///     Name of method
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Type of returned value
        /// </summary>
        public string ReturnType { get; set; }
    }
}