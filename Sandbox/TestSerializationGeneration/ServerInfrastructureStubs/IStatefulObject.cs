using YumaPos.FrontEnd.Infrastructure.Persistence;

namespace TestSerializationGeneration
{
    public interface IStatefulObject
    {
        void SetDataObject(IBoDo value, IDeserializationContext context);
        IBoDo GetDataObject(IBoDo childBodo = null);
        void ResetDataObject();
    }
}