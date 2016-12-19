using System;
using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    public partial class ServiceDecorator : TestDecoratorGeneration.IService, IDisposable
    {
        private string _methodName;
        private object[] _args;
        private bool _isInitialized;

        public ServiceDecorator()
        {
            try
            {
                DecoratedComponent = new Service();
            }
            catch (Exception e)
            {
                OnException(e);
                Dispose();
            }
            finally
            {
                OnExit();
            }
        }

        public void Dispose()
        {
            try
            {
                if(DecoratedComponent != null)
                {
                    DecoratedComponent.Dispose();
                }
            }
            catch (Exception e)
            {
                OnException(e);
            }
            finally
            {
                OnExit();
            }
        }
        private async Task OnEntryAsync(string methodName, object[] args)
        {
            OnEntry(methodName, args);
        }

        private void OnEntry(string methodName, object[] args)
        {
            if (!_isInitialized)
            {
                _methodName = methodName;
                _args = args;
                _isInitialized = true;
            }
        }

        private  void OnExit()
        {
            
        }

        private async Task OnExitAsync()
        {

        }

        private void OnException(Exception exception)
        {
            
        }

        private void OnFinally()
        {
            
        }
    }
}