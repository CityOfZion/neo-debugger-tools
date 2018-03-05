using System;

namespace Neo.Emulator
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class NonemitAttribute : Attribute
    {
    }
}
