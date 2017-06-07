using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    public class ServiceDecorator : IService, IDisposable
    {
       public void Dispose()
        {
        }

        public async Task Open()
        {
        }

        public async Task<ResponseDto> AddItem(Guid id, string name)
        {
            return new ResponseDto();
        }

        public async Task<IEnumerable<ItemDto>> GetItems()
        {
            return new List<ItemDto>();
        }
    }
}