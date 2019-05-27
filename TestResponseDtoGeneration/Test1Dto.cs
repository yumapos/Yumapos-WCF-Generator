using System;
using TestResponseDtoGeneration;

namespace Dto1Namespace
{
    [GenerateResponseDto]
    public class Test1Dto
    {
        public Guid Id { get; set; }
        public string City { get; set; }
    }
}
