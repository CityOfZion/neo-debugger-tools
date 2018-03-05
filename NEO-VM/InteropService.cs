using System;
using System.Collections.Generic;

namespace Neo.VM
{
    public class InteropCall
    {
        public Func<ExecutionEngine, bool> handler;
        public double gasCost;
    }

    public class InteropService
    {
        public const double defaultGasCost = 0.001;

        private Dictionary<string, InteropCall> dictionary = new Dictionary<string, InteropCall>();

        public InteropService()
        {
            Register("System.ExecutionEngine.GetScriptContainer", GetScriptContainer, defaultGasCost);
            Register("System.ExecutionEngine.GetExecutingScriptHash", GetExecutingScriptHash, defaultGasCost);
            Register("System.ExecutionEngine.GetCallingScriptHash", GetCallingScriptHash, defaultGasCost);
            Register("System.ExecutionEngine.GetEntryScriptHash", GetEntryScriptHash, defaultGasCost);
        }

        public InteropCall FindCall(string method)
        {
            if (!dictionary.ContainsKey(method)) return null;
            return dictionary[method];
        }

        public void Register(string method, Func<ExecutionEngine, bool> handler, double gasCost)
        {
            var call = new InteropCall();
            call.handler = handler;
            call.gasCost = gasCost;
            dictionary[method] = call;
        }

        internal bool Invoke(string method, ExecutionEngine engine)
        {
            if (!dictionary.ContainsKey(method)) return false;
            return dictionary[method].handler(engine);
        }

        private static bool GetScriptContainer(ExecutionEngine engine)
        {
            engine.EvaluationStack.Push(StackItem.FromInterface(engine.ScriptContainer));
            return true;
        }

        private static bool GetExecutingScriptHash(ExecutionEngine engine)
        {
            engine.EvaluationStack.Push(engine.CurrentContext.ScriptHash);
            return true;
        }

        private static bool GetCallingScriptHash(ExecutionEngine engine)
        {
            engine.EvaluationStack.Push(engine.CallingContext.ScriptHash);
            return true;
        }

        private static bool GetEntryScriptHash(ExecutionEngine engine)
        {
            engine.EvaluationStack.Push(engine.EntryContext.ScriptHash);
            return true;
        }
    }
}
