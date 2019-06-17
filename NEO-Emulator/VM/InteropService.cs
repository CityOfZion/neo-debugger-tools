using Neo.Emulation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Neo.VM
{
    public class InteropCall
    {
        public string name;
        public Func<ExecutionEngine, bool> handler;
        public decimal gasCost;
    }

    public class InteropService : IInteropService
    {
        public const decimal defaultGasCost = 0.001m;

        private Dictionary<string, InteropCall> dictionary = new Dictionary<string, InteropCall>();

        public IEnumerable<InteropCall> Calls => dictionary.Values;

        public InteropService()
        {
            Register("System.ExecutionEngine.GetScriptContainer", GetScriptContainer, defaultGasCost);
            Register("System.ExecutionEngine.GetExecutingScriptHash", GetExecutingScriptHash, defaultGasCost);
            Register("System.ExecutionEngine.GetCallingScriptHash", GetCallingScriptHash, defaultGasCost);
            Register("System.ExecutionEngine.GetEntryScriptHash", GetEntryScriptHash, defaultGasCost);

            var assembly = typeof(VMUtils).Assembly;
            var methods = assembly.GetTypes()
                                  .SelectMany(t => t.GetMethods())
                                  .Where(m => m.GetCustomAttributes(typeof(SyscallAttribute), false).Length > 0)
                                  .ToArray();

            foreach (var method in methods)
            {
                var attr = (SyscallAttribute)method.GetCustomAttributes(typeof(SyscallAttribute), false).FirstOrDefault();

                this.Register(attr.Method, (engine) => { return (bool)method.Invoke(null, new object[] { engine }); }, attr.gasCost);
            }
        }

        public InteropCall FindCall(string method)
        {
            if (!dictionary.ContainsKey(method)) return null;
            return dictionary[method];
        }

        public void Register(string method, Func<ExecutionEngine, bool> handler, decimal gasCost)
        {
            var call = new InteropCall();
            call.handler = handler;
            call.gasCost = gasCost;
            call.name = method;
            dictionary[method] = call;
        }

        internal bool Invoke(string method, ExecutionEngine engine)
        {
            if (!dictionary.ContainsKey(method)) return false;
            return dictionary[method].handler(engine);
        }

        private static bool GetScriptContainer(ExecutionEngine engine)
        {
            var context = engine.CurrentContext;
            context.EvaluationStack.Push(StackItem.FromInterface(engine.ScriptContainer));
            return true;
        }

        private static bool GetExecutingScriptHash(ExecutionEngine engine)
        {
            var context = engine.CurrentContext;
            context.EvaluationStack.Push(engine.CurrentContext.ScriptHash.ToArray());
            return true;
        }

        private static bool GetCallingScriptHash(ExecutionEngine engine)
        {
            var context = engine.CurrentContext;
            context.EvaluationStack.Push(engine.CallingContext.ScriptHash.ToArray());
            return true;
        }

        private static bool GetEntryScriptHash(ExecutionEngine engine)
        {
            var context = engine.CurrentContext;
            context.EvaluationStack.Push(engine.EntryContext.ScriptHash.ToArray());
            return true;
        }

        public bool Invoke(byte[] method, ExecutionEngine engine)
        {
            return Invoke(System.Text.Encoding.ASCII.GetString(method), engine);
        }
    }
}
