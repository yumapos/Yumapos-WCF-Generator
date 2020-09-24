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
        [DataMember]
        public string Context { get; set; }

        [DataMember]
        public CommandPostprocessingType? PostprocessingType { get; set; }

        public object Errors { get; set; }
        public object ServerInfo { get; set; }
        public object Value { get; set; }
    }
}