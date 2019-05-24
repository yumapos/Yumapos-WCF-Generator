using System;
using System.Collections.Generic;
using TestResponseDtoGeneration;

namespace Dto2Namespace.Dto2Subnamespace
{
    [GenerateResponseDto]
    public class Dto2
    {
        public Guid StoreId { get; set; }
        public string Name { get; set; }
    }
}
