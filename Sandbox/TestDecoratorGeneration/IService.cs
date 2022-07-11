using System;
using System.ServiceModel;
using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        Task<ResponseDto> Open();

        [OperationContract]
        Task<ResponseDto> AddItem(Guid id, string name);

        [OperationContract]
        Task<ItemListResponseDto> GetListOfItems();
    }

   
}