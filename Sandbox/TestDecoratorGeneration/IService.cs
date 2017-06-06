using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    [ServiceContract]
    public interface IService
    {
        //[OperationContract]
        //Task Open();

        //[OperationContract]
        //Task<ResponseDto> AddItem(Guid id, string name);

        //[OperationContract]
        //Task<IEnumerable<ItemDto>> GetItems();
    }
}