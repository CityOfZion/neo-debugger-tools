using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public class Validator
    {
        [Syscall("Neo.Validator.Register", 1000)]
        public static bool Register(ExecutionEngine engine)
        {
            //byte[] pubkey
            //returns Validator 
            throw new NotImplementedException();
        }
    }
}
