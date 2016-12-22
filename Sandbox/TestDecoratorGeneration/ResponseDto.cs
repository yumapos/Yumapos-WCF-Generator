using System.Runtime.Serialization;

namespace TestDecoratorGeneration
{
    [DataContract]
    public class ResponseDto
    {
        [DataMember]
        public string Context { get; set; }
    }
}