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
        // All public properties should be included on generation
        public string Property { get; set; }

        // Private non public properties should not be included on generation
        private IDictionary<string, string> PrivatePropertyWithInvalidType { get; set; }

        // All non public properties should be included on generation if have attribute SerializeInclude
        [SerializeInclude] 
        private string SerializeIncludeProperty { get; set; }

        // All properties should not be included on generation if have attribute SerializeIgnore
        [SerializeIgnore]
        public string SerializeIgnoreProperty { get; set; }

        // Uncomment this property for get error on generation with invalide type of property
        // public Dictionary<string, string> PublicPropertyWithInvalidType { get; set; }

        // Invalide type error will skipped for properties with attribute SerializeIgnore
        [SerializeIgnore]
        public Dictionary<string, string> IgnoredPropertyWithInvalidType { get; set; }
    }
}