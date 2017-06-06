using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    public class Service : IService, IDisposable
    {
        public Service(){}

        public int Id { get; set; }

        [IgnoreDecoration]
        public async Task Open()
        {
            
        }
        [IgnoreDecoration]
        public async Task<bool> AddItem2(Guid id, string name)
        {
            return true;
        }
        [IgnoreDecoration]
        public async Task<ResponseDto> AddItem(Guid id, string name)
        {
            return new ResponseDto();
        }
        [IgnoreDecoration]
        public async Task<IEnumerable<ItemDto>> GetItems()
        {
            return new List<ItemDto>();
        }
        [IgnoreDecoration]
        public void Stop()
        {
            
        }

        [IgnoreDecoration]
        public async Task<string> GetSystemSettings(string[] listOfSystemSettings)
        {
            return "";
        }
        [IgnoreDecoration]
        public Guid GetGuid()
        {
            return Guid.Empty;
        }
        [IgnoreDecoration]
        public int GetInt()
        {
            return 0;
        }


        public void Dispose(){}
    }
}