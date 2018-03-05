using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public class Account
    {
        [Syscall("Neo.Account.GetScriptHash")]
        public static bool ScriptHash(ExecutionEngine engine)
        {
            //return byte[] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Account.GetVotes")]
        public static bool GetVotes(ExecutionEngine engine)
        {
            //byte[][] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Account.SetVotes", 1)]
        public static bool SetVotes(ExecutionEngine engine)
        {
            //byte[][] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Account.GetBalance")]
        public static bool GetBalance(ExecutionEngine engine)
        {
            //byte[] asset_id
            //return long
            throw new NotImplementedException();
        }
    }
}
