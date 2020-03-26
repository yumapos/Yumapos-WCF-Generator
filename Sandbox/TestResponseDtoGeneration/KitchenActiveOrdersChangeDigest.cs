using System;
using System.Collections.Generic;
using System.Text;
using TestResponseDtoGeneration;

namespace Dto1Namespace
{
    [GenerateResponseDto]
    public class KitchenActiveOrdersChangeDigest
    {
        public DateTime DigestDate { get; set; }
        public IEnumerable<Guid> AddedItems { get; set; }
        public IEnumerable<Guid> UpdatedItems { get; set; }
        public IEnumerable<Guid> DeletedItems { get; set; }
    }
}
