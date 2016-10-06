namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class RepositoryFieldInfo
    {
        public RepositoryFieldInfo(string typeName, string name, string interfaceName = null)
        {
            Name = name;
            InterfaceName = interfaceName;
            TypeName = typeName;
            InitNew = interfaceName == null;

        }

        public string Name { get; set; }
        public string InterfaceName { get; set; }
        public string TypeName { get; set; }

        /// <summary>
        ///     If true - use new(...), false - get from constructor parameters  
        /// </summary>
        public bool InitNew { get; set; }
    }
}