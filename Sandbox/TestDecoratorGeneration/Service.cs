using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TestDecoratorGeneration
{
    public partial class Service : IService, IDisposable
    {
        private string _methodName;
        private object[] _args;
        private bool _isInitialized;

        public Service()
        {
            DecoratedComponent = new ServiceDecorator();
        }

        protected ISerializationService SerializationService;

        protected SerializationOptions SerializationOptions;

        public void Dispose(){}

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

        private async Task OnExitAsync(object res = null)
        {
        }

        private void OnExit(object res = null)
        {
        }

        private void OnException(Exception exception)
        {
        }

        private void OnFinally()
        {
        }
    }

    [Flags]
    public enum SerializationOptions
    {
        None = 0,
        WithTypes = 1,
        PreserveObjectReferences = 2
    }

    public interface ISerializationService
    {
        string Serialize(object toSerialize, SerializationOptions options);

        T Deserialize<T>(string str, SerializationOptions options);
    }
}