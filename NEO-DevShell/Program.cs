using System;

namespace NEO_DevShell
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("-NEO Developer Shell- version 0.1");
            var shell = new Shell();

            if (args.Length>0)
            {
                shell.Execute("load " + args[0]);
            }

            while (true)
            {
                Console.Write(">");
                var input = Console.ReadLine();

                if (string.IsNullOrEmpty(input))
                    continue;

                if (!shell.Execute(input))
                {
                    Console.WriteLine("Unknown command. Type HELP for list of commands.");
                }
            }
        }
    }
}
