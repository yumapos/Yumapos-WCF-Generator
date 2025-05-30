namespace WCFGenerator.RepositoriesGeneration.Infrastructure
{
    internal class ParameterInfo
    {
        public ParameterInfo(string name, string typeName, bool needGeneratePeriod, string defaultValue = null)
        {
            Name = name;
            TypeName = typeName;
            DefaultValue = defaultValue;
            NeedGeneratePeriod = needGeneratePeriod;
            if (typeName.EndsWith("?"))
            {
                IsNullable = true;
                TypeName = typeName.Trim('?'); // filter key can not be nullable Guid, Int...
            }
        }

        public string Name { get; set; }
        public string TypeName { get; set; }
        public string DefaultValue { get; set; }
        public bool NeedGeneratePeriod { get; set; }
        public bool IsNullable { get; set; }
    }
}
