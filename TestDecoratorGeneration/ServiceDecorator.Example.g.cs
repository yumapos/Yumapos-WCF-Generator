using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    public partial class ServiceDecorator
    {
        public async Task Open()
        {
            try
            {
                await Init("Open", new object[] {});
                OnEntry();
                await DecoratedComponent.Open();
                OnExit();
            }
            catch (Exception e)
            {
                OnException(e);
            }
            finally
            {
                OnFinally();
            }
        }

        public async Task<ResponseDto> AddItem(Guid id, string name)
        {
            try
            {
                await Init("AddItem", new object[] { id, name });
                OnEntry();
                var res = await DecoratedComponent.AddItem(id, name);
            }
            catch (Exception e)
            {
                OnException(e);
            }
            return null;
        }

        public async Task<IEnumerable<ItemDto>> GetItems()
        {
            await Init("GetItems", new object[] {});
            OnEntry();
            try
            {
                return await DecoratedComponent.GetItems();
            }
            catch (Exception e)
            {
                OnException(e);
                throw;
            }
        }

        
        
    }
}