using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using YumaPos.Shared.API.ResponseDtos;

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

        public IEnumerable<ResponseErrorDto> Errors { get; set; }
        public IEnumerable<ResponseErrorDto> Warnings { get; set; }
        public object ServerInfo { get; set; }
        public object Value { get; set; }
    }
}