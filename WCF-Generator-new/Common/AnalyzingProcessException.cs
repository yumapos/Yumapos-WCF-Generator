using System;

namespace WCFGenerator.Common
{
    internal class AnalyzingProcessException : Exception
    {
        public AnalyzingProcessException(string message) : base(message)
        {
        }

        public AnalyzingProcessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    public static partial class Extensions
    {
        public static void ThrowAnalyzingException(this IGeneration generator, string message, Exception innerException = null)
        {
            var msg = $"Error occured on analyzation step in {generator.GetType().Name} : {message}";
            if (innerException == null)
            {
                throw new AnalyzingProcessException(msg);
            }
            throw new AnalyzingProcessException(msg, innerException);
        }
    }
}
