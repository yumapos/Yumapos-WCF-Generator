using System.Collections.Generic;

namespace TestSerializationGeneration
{
    public partial class TestClassDo
    {
        [MigrationIgnore]
        public string MigrationIgnoreProperty { get; set; }
    }

    public abstract partial class TestClass : IStatefulObject
    {
        public string Property { get; set; }

        public Dictionary<string, string> PropertyWithInvalidType { get; set; }

        [SerializeIgnore]
        public string SerializeIgnoreProperty { get; set; }

        [SerializeInclude]
        public string SerializeIncludeProperty { get; set; }
    }
}