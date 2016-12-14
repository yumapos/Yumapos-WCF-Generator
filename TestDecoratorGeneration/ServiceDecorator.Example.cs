using System;
using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    public partial class ServiceDecoratorExample : TestDecoratorGeneration.IService, IDisposable
    {
        private string _methodName;
        private object[] _args;
        private bool _isInitialized;

        public ServiceDecoratorExample()
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
            OnEntry();
            try
            {
                DecoratedComponent.Dispose();
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

        private async Task Init(string methodName, object[] args)
        {
            if (!_isInitialized)
            {
                _methodName = methodName;
                _args = args;
                _isInitialized = true;
            }
        }

        private void OnEntry()
        {
            
        }

        private void OnExit()
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