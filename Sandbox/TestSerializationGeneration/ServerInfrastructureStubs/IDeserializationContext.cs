using TestSerializationGeneration;

namespace YumaPos.FrontEnd.Infrastructure.Persistence
{
    public interface IDeserializationContext
    {
        IMonetarySettings MonetarySettings { get; }
        T Resolve<T>(IBoDo dataObject) where T : class;
        bool TryResolveKeyed<T>(IBoDo dataObject, object key, out T result) where T : class;
    }
}