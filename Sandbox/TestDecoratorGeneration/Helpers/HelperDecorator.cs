using System;
using System.Threading.Tasks;

namespace TestDecoratorGeneration.Helpers
{
    public partial class HelperDecorator
    {
        public HelperDecorator()
        {
            DecoratedComponent = new Helper();
        }

        private void OnEntry(string canihelpyou, object[] objects)
        {
            throw new System.NotImplementedException();
        }

        private void OnExit(bool ret)
        {
            throw new System.NotImplementedException();
        }

        private void OnException(Exception exception)
        {
            throw new NotImplementedException();
        }

        private void OnFinally()
        {
            throw new NotImplementedException();
        }

        private Task OnEntryAsync(string gettask, object[] objects)
        {
            throw new NotImplementedException();
        }

        private Task OnExitAsync()
        {
            throw new NotImplementedException();
        }
    }
}