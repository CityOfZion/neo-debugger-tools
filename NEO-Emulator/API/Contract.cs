using Neo.VM;
using System;

namespace Neo.Emulator.API
{
    public class Contract
    {
        [Syscall("Neo.Contract.GetScript")]
        public static bool GetScript(ExecutionEngine engine)
        {
            // Contract
            // returns byte[] 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Contract.GetStorageContext")]
        public static bool GetStorageContext(ExecutionEngine engine)
        {
            // Contract
            // returns StorageContext 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Contract.Create", 500)]
        public static bool Create(ExecutionEngine engine)
        {
            //byte[] script, byte[] parameter_list, byte return_type, bool need_storage, string name, string version, string author, string email, string description
            // returns Contract 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Contract.Migrate", 500)]
        public static bool Migrate(ExecutionEngine engine)
        {
            //byte[] script, byte[] parameter_list, byte return_type, bool need_storage, string name, string version, string author, string email, string description
            // returns Contract 
            throw new NotImplementedException();
        }

        [Syscall("Neo.Contract.Destroy")]
        public static bool Destroy(ExecutionEngine engine)
        {
            // returns nothing
            throw new NotImplementedException();
        }
    }
}
