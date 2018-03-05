using Neo.Emulator.Dissambler;
using System;
using System.IO;

namespace NEO_CLI_Disassembler
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine($"{AppDomain.CurrentDomain.FriendlyName} <filename.avm>");
                Environment.Exit(-1);
            }

            var fileName = args[0];

            if (!File.Exists(fileName))
            {
                Console.WriteLine($"{fileName} was not found");
                Environment.Exit(-1);
            }

            var bytes = File.ReadAllBytes(fileName);
            var avm_asm = NeoDisassembler.Disassemble(bytes);

            var output = avm_asm.ToString();
            Console.WriteLine(output);
        }

    }
}
