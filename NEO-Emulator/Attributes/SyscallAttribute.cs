using Neo.VM;
using System;

namespace Neo.Emulator
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public class SyscallAttribute : Attribute
    {
        public string Method { get; }
        public double gasCost { get; }

        public SyscallAttribute(string method, double gasCost = InteropService.defaultGasCost)
        {
            this.Method = method;
            this.gasCost = gasCost;
        }
    }
}
