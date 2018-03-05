using Neo.Compiler.AVM;
using Neo.Compiler.MSIL;
using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace Neo.Compiler
{
    public class Program
    {
        //Console.WriteLine("helo ha:"+args[0]); //普通输出
        //Console.WriteLine("<WARN> 这是一个严重的问题。");//警告输出，黄字
        //Console.WriteLine("<WARN|aaaa.cs(1)> 这是ee一个严重的问题。");//警告输出，带文件名行号
        //Console.WriteLine("<ERR> 这是一个严重的问题。");//错误输出，红字
        //Console.WriteLine("<ERR|aaaa.cs> 这是ee一个严重的问题。");//错误输出，带文件名
        //Console.WriteLine("SUCC");//输出这个表示编译成功
        //控制台输出约定了特别的语法
        public static void Main(string[] args)
        {

            //set console
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            var log = new DefLogger();
            log.Log("Neo.Compiler.MSIL console app v" + Assembly.GetEntryAssembly().GetName().Version + " [DEBUGGER SUPPORT]");
            if (args.Length == 0)
            {
                log.Log("need one param for DLL filename.");
                return;
            }


            string filename = args[0];
            log.Log("Trying to compile " + filename);

            var extension = Path.GetExtension(filename);
            string filepdb = filename.Replace(extension, ".pdb");

            // fix necessary when debugging the compiler via VS
            var path = Path.GetDirectoryName(filename);
            if (!string.IsNullOrEmpty(path))
            {
                try
                {
                    Directory.SetCurrentDirectory(path);
                }
                catch
                {
                    log.Log("Could not find path: " + path);
                    Environment.Exit(-1);
                }
            }

            if (!File.Exists(filename))
            {
                log.Log("Could not find file: " + filename);
                Environment.Exit(-1);
            }

            switch (extension)
            {
                case ".dll":
                    {
                        if (AVMCompiler.Execute(filename, filepdb, log))
                        {
                            log.Log("SUCC");
                        }

                        break;
                    }

                case ".cs":
                    {
                        if (CSharpCompiler.Execute(filename, log))
                        {
                            log.Log("SUCC");
                        }

                        break;
                    }

                default:
                    {
                        log.Log("Invalid extension: " + extension);
                        Environment.Exit(-1);
                        break;
                    }
            }
            
        }
    }
}
