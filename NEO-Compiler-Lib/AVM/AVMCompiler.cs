using Neo.Compiler.MSIL;
using System;
using System.IO;
using System.Text;

namespace Neo.Compiler.AVM
{
    public class AVMCompiler
    {
        //Console.WriteLine("helo ha:"+args[0]); //普通输出
        //Console.WriteLine("<WARN> 这是一个严重的问题。");//警告输出，黄字
        //Console.WriteLine("<WARN|aaaa.cs(1)> 这是ee一个严重的问题。");//警告输出，带文件名行号
        //Console.WriteLine("<ERR> 这是一个严重的问题。");//错误输出，红字
        //Console.WriteLine("<ERR|aaaa.cs> 这是ee一个严重的问题。");//错误输出，带文件名
        //Console.WriteLine("SUCC");//输出这个表示编译成功
        //控制台输出约定了特别的语法
        public static bool Execute(string filename, string filepdb, ILogger log)
        {
            string onlyname = System.IO.Path.GetFileNameWithoutExtension(filename);

            ILModule mod = new ILModule();
            System.IO.Stream fs = null;
            System.IO.Stream fspdb = null;

            //open file
            try
            {
                fs = System.IO.File.OpenRead(filename);

                if (System.IO.File.Exists(filepdb))
                {
                    fspdb = System.IO.File.OpenRead(filepdb);
                }

            }
            catch (Exception err)
            {
                log.Log("Open File Error:" + err.ToString());
                return false;
            }

            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            var curPath = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(exePath);

            //load module
            try
            {
                mod.LoadModule(fs, fspdb);
           }
            catch (Exception err)
            {
                log.Log("LoadModule Error:" + err.ToString());
                return false;
            }
            byte[] bytes = null;
            bool bSucc = false;
            string jsonstr = null;
            string debugmapstr = null;
            //convert and build
            try
            {
                var conv = new ModuleConverter(log);

                NeoModule am = conv.Convert(mod);

                Directory.SetCurrentDirectory(curPath);

                bytes = am.Build();
                log.Log("convert succ");


                try
                {
                    var outjson = FuncExport.Export(am, bytes);
                    StringBuilder sb = new StringBuilder();
                    outjson.ConvertToStringWithFormat(sb, 0);
                    jsonstr = sb.ToString();
                    log.Log("gen abi succ");
                }
                catch (Exception err)
                {
                    log.Log("gen abi Error:" + err.ToString());
                }

                try
                {
                    var outjson = DebugInfo.ExportDebugInfo(onlyname, am);
                    StringBuilder sb = new StringBuilder();
                    outjson.ConvertToStringWithFormat(sb, 0);
                    debugmapstr = sb.ToString();
                    log.Log("gen debug map succ");
                }
                catch (Exception err)
                {
                    log.Log("gen debug map Error:" + err.ToString());
                }

            }
            catch (Exception err)
            {
                log.Log("Convert Error:" + err.ToString());
                return false;
            }
            //write bytes
            try
            {

                string bytesname = onlyname + ".avm";

                System.IO.File.Delete(bytesname);
                System.IO.File.WriteAllBytes(bytesname, bytes);
                log.Log("write:" + bytesname);
                bSucc = true;
            }
            catch (Exception err)
            {
                log.Log("Write Bytes Error:" + err.ToString());
                return false;
            }
            try
            {

                string abiname = onlyname + ".abi.json";

                System.IO.File.Delete(abiname);
                System.IO.File.WriteAllText(abiname, jsonstr);
                log.Log("write:" + abiname);
                bSucc = true;
            }
            catch (Exception err)
            {
                log.Log("Write abi Error:" + err.ToString());
                return false;
            }
            try
            {
                string debugname = onlyname + ".debug.json";

                System.IO.File.Delete(debugname);
                System.IO.File.WriteAllText(debugname, debugmapstr);
                log.Log("write:" + debugname);
                bSucc = true;
            }
            catch (Exception err)
            {
                log.Log("Write abi Error:" + err.ToString());
                return false;
            }
            try
            {
                fs.Dispose();
                if (fspdb != null)
                    fspdb.Dispose();
            }
            catch
            {

            }

            return bSucc;
        }
    }
}
