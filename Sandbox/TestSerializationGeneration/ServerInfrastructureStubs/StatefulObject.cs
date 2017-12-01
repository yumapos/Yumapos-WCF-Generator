using YumaPos.FrontEnd.Infrastructure.Persistence;

namespace TestSerializationGeneration
{
    public abstract class StatefulObject : IStatefulObject
    {
        protected IBoDo _BoDo;

        public virtual void SetDataObject(IBoDo value, IDeserializationContext context)
        {
            throw new System.NotImplementedException();
        }

        public virtual IBoDo GetDataObject(IBoDo boDoFirst = null)
        {
            return boDoFirst;
        }
        public void ResetDataObject()
        {
            _BoDo = null;
        }

        protected virtual IBoDo BoDoInstance { get; set; }
    }
}