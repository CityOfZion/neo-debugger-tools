using Neo.VM;
using System;

namespace Neo.Emulator
{
    public class SmartContract
    {
        [OpCode(OpCode.SHA1)]
        protected static byte[] Sha1(byte[] data)
        {
            throw new NotImplementedException();
        }

        [OpCode(OpCode.SHA256)]
        protected static byte[] Sha256(byte[] data)
        {
            throw new NotImplementedException();
        }


        [OpCode(OpCode.HASH160)]
        protected static byte[] Hash160(byte[] data)
        {
            throw new NotImplementedException();
        }


        [OpCode(OpCode.HASH256)]
        protected static byte[] Hash256(byte[] data)
        {
            throw new NotImplementedException();
        }


        [OpCode(OpCode.CHECKSIG)]
        protected static bool VerifySignature(byte[] signature, byte[] pubkey)
        {
            throw new NotImplementedException();
        }


        [OpCode(OpCode.CHECKMULTISIG)]
        protected static bool VerifySignatures(byte[][] signature, byte[][] pubkey)
        {
            throw new NotImplementedException();
        }

    }
}
