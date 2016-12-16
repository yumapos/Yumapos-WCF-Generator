using System;
using System.Runtime.Serialization;

namespace TestDecoratorGeneration
{
    [DataContract]
    public class ItemDto
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}