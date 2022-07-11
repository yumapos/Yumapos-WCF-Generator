using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;
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
    }

    [DataContract]
    public class ItemListResponseDto
    {
        [DataMember]
        public Task<IEnumerable<ItemDto>> Value { get; set; }
        [DataMember]
        public virtual IList<ResponseErrorDto> Errors { get; set; }

        [DataMember]
        public string Context { get; set; }
        [DataMember]
        public CommandPostprocessingType? PostprocessingType { get; set; }

        public List<ResponseErrorDto> Warnings { get; set; }
    }
}