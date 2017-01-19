using System;

namespace TestDecoratorGeneration
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class IgnoreDecorationAttribute : Attribute
    {
        
    }
}
