using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    public class Service : IService, IDisposable
    {
        public Service(){}

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
        public void Stop()
        {
            
        }

        public Guid GetGuid()
        {
            return Guid.Empty;
        }

        public int GetInt()
        {
            return 0;
        }

        public void Dispose(){}
    }
}