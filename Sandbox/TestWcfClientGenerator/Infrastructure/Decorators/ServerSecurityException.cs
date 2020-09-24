using System;
using TestDecoratorGeneration;

namespace TestWcfClientGenerator
{
    public class ServerSecurityException : Exception
    {
        public ServerSecurityException(string responseContext, CommandPostprocessingType? responsePostprocessingType, object errors, object serverInfo)
        {
            throw new NotImplementedException();
        }

        public object Value { get; set; }
    }
}