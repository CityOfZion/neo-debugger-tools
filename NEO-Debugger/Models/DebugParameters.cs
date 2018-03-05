using LunarParser;
using Neo.Emulator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Debugger.Models
{
    public class DebugParameters
    {
        public DebugParameters()
        {
            DefaultParams = new Dictionary<string, string>();
            Transaction = new Dictionary<byte[], BigInteger>();
        }

        public CheckWitnessMode WitnessMode { get; set; }

        public TriggerType TriggerType { get; set; }

        public Dictionary<string, string> DefaultParams { get; set; }

        public Dictionary<byte[], BigInteger> Transaction { get; set; }

        public string PrivateKey { get; set; }

        public DataNode ArgList { get; set; }

        public uint Timestamp { get; set; }
    }
}
