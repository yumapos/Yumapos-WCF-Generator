using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    public class Service : IService, IDisposable
    {
        public Service(){}

        public int Id { get; set; }

        public async Task Open()
        {
            
        }

        public async Task<bool> AddItem2(Guid id, string name)
        {
            return true;
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

        public async Task<string> GetSystemSettings(string[] listOfSystemSettings)
        {
            return "";
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