using Neo.Emulation;
using Neo.Emulation.Utils;
using System;

namespace Neo.Debugger.Shell
{
    public static class ShellRunner
    {
        public static void UpdateState(DebuggerShell Shell, Action<ShellMessageType, string> output)
        {
            var state = Shell.Debugger.State;

            output(ShellMessageType.Default, $"VM state: {state.state}");
            output(ShellMessageType.Default, $"Instruction pointer: {state.offset}");

            switch (state.state)
            {
                case DebuggerState.State.Finished:
                    {
                        var val = Shell.Debugger.Emulator.GetOutput();

                        Shell.Debugger.Blockchain.Save();

                        var methodName = Shell.Debugger.Emulator.currentMethod;
                        var hintType = !string.IsNullOrEmpty(methodName) && Shell.Debugger.ABI != null && Shell.Debugger.ABI.functions.ContainsKey(methodName) ? Shell.Debugger.ABI.functions[methodName].returnType : Emulator.Type.Unknown;

                        output(ShellMessageType.Success, "Result: " + FormattingUtils.StackItemAsString(val, false, hintType));
                        output(ShellMessageType.Default, "GAS used: " + Shell.Debugger.Emulator.usedGas);

                        break;
                    }

                case DebuggerState.State.Break:
                    {
                        string filePath;
                        int lineNumber;
                        lineNumber = Shell.Debugger.ResolveLine(state.offset, true, out filePath);

                        lineNumber++;
                        output(ShellMessageType.Default, $"Breakpoint hit, line {lineNumber} in {filePath}");

                        int count = 0;

                        foreach (var entry in Shell.Debugger.Emulator.Variables)
                        {
                            if (count == 0)
                            {
                                output(ShellMessageType.Default, $"Variable values:");
                            }

                            var val = FormattingUtils.StackItemAsString(entry.value, true, entry.type);
                            output(ShellMessageType.Default, $"\t{entry.name} = {val}");
                            count++;
                        }
                        break;
                    }
            }
        }
    }

    public class RunCommand : Command
    {
        public override string Name => "run";
        public override string Help => "Continues running an invoke until end or breakpoint";

        public override void Execute(string[] args, Action<ShellMessageType, string> output)
        {
            if (Shell.Debugger.State.state == DebuggerState.State.Running || Shell.Debugger.State.state == DebuggerState.State.Break)
            {
                output(ShellMessageType.Default, "Resuming invoke.");
                Shell.Debugger.Run();
                ShellRunner.UpdateState(Shell, output);
            }
            else
            {
                output(ShellMessageType.Error, "Start a transaction with invoke first.");
            }
        }
    }

    public class StepCommand : Command
    {
        public override string Name => "step";
        public override string Help => "Executes a single instruction then break";

        public override void Execute(string[] args, Action<ShellMessageType, string> output)
        {
            if (Shell.Debugger.State.state == DebuggerState.State.Running || Shell.Debugger.State.state == DebuggerState.State.Break)
            {
                output(ShellMessageType.Default, "Stepping invoke.");

                string startFile;
                var startLine = Shell.Debugger.ResolveLine(Shell.Debugger.State.offset, true, out startFile);

                string currentFile;
                int currentLine;
                do
                {
                    Shell.Debugger.Step();
                    currentLine = Shell.Debugger.ResolveLine(Shell.Debugger.State.offset, true, out currentFile);
                } while (currentFile == startFile && currentLine == startLine);

                ShellRunner.UpdateState(Shell, output);
            }
            else
            {
                output(ShellMessageType.Error, "Start a transaction with invoke first.");
            }
        }
    }

}
