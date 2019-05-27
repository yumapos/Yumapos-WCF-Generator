using System;
using System.Collections.Generic;
using System.Text;

namespace TestResponseDtoGeneration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class GenerateResponseDtoAttribute : Attribute
    {
    }
}
