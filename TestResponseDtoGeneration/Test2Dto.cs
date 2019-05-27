using System;
using System.Collections.Generic;
using TestResponseDtoGeneration;

namespace Dto2Namespace.Dto2Subnamespace
{
    [GenerateResponseDto]
    public class Test2Dto
    {
        public Guid StoreId { get; set; }
        public string Name { get; set; }
    }
}
