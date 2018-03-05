using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public class TransactionAttribute : IApiInterface
    {
        [Syscall("Neo.Attribute.GetUsage")]
        public static bool GetUsage(ExecutionEngine engine)
        {
            // TransactionAttribute
            // returns byte 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Attribute.GetData")]
        public static bool GetData(ExecutionEngine engine)
        {
            // TransactionAttribute
            // returnsbyte[]
            throw new NotImplementedException();
        }
    }
}
