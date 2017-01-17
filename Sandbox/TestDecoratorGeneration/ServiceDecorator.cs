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
                OnExit(null);
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
                OnExit(null);
            }
        }
        private async Task<ICommandExecutionResult> OnEntryAsync(string methodName, object[] args)
        {
            OnEntry(methodName, args);

            return new CommandExecutionResult();
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

        private  void OnExit(object res = null)
        {
            
        }

        
        private async Task OnExitAsync(object res = null)
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