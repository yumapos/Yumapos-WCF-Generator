using System;

namespace Generator.Repository.Infrastructure.Attributes
{
    /// <summary>
    ///    The attribute marks DO classes  for which you need to enable the automatic flag SyncState reset mechanism 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class HasSyncState : Attribute
    {
    }
}
