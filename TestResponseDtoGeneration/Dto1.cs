using System;
using TestResponseDtoGeneration;

namespace Dto1Namespace
{
    [GenerateResponseDto]
    public class Dto1
    {
        public Guid Id { get; set; }
        public string City { get; set; }
    }
}
