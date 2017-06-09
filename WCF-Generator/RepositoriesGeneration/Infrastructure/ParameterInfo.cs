namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class ParameterInfo
    {
        public ParameterInfo(string name, string typeName, bool needGeneratePeriod, string defaultValue = null)
        {
            Name = name;
            TypeName = typeName.Trim('?'); // filter key can not be nullable Guid, Int...
            DefaultValue = defaultValue;
            NeedGeneratePeriod = needGeneratePeriod;
        }

        public string Name { get; set; }
        public string TypeName { get; set; }
        public string DefaultValue { get; set; }
        public bool NeedGeneratePeriod { get; set; }
    }
}
