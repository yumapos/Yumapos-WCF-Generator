using System;
using System.Diagnostics;

namespace TestWcfClientGenerator
{
    public static class StringExtensions
    {
        /// <summary>
        /// Determines whether the string is null, empty or all whitespace.
        /// </summary>
        [DebuggerStepThrough]
        public static bool IsEmpty(this string value)
        {
            return string.IsNullOrWhiteSpace(value);
        }
    }
}