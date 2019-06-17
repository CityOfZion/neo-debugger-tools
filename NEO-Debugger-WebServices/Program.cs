// NOTE - For this project to run it is necessary to download Electron and copy its contents to folder "Output/electron"
using LunarLabs.Parser;
using LunarLabs.Parser.JSON;
using LunarLabs.WebServer.Core;
using LunarLabs.WebServer.HTTP;
using LunarLabs.WebServer.Templates;
using Neo.Debugger.Core.Data;
using Neo.Debugger.Core.Models;
using Neo.Debugger.Core.Utils;
using Neo.Debugger.Shell;
using Neo.Emulation;
using Neo.Emulation.Utils;
using System;
using System.Collections.Generic;
using System.IO;


namespace Neo.Debugger.Electron
{
    public class FileEntry
    {
        public string id;
        public string name;
        public string path;
        public string url;
        public bool active;
        public string content;

        public HashSet<int> breakpoints = new HashSet<int>();
    }

    public class Backend
    {
        private string activeDocumentID;
        private Dictionary<string, FileEntry> projectFiles = new Dictionary<string, FileEntry>();

        private DebugManager _debugger;
        private DebuggerShell _shell;

        private int docAllocID = 100;

        private FileEntry LoadFile(string filePath, string content = null)
        {
            if (content == null)
            {
                if (File.Exists(filePath))
                {
                    content = File.ReadAllText(filePath);
                }
                else
                {
                    return null;
                }
            }

            var entry = new FileEntry() { path = filePath, name = Path.GetFileName(filePath), active = false, id = docAllocID.ToString(), content = content };

            projectFiles[entry.id] = entry;

            if (activeDocumentID == null)
            {
                activeDocumentID = entry.id;
            }

            docAllocID++;

            return entry;
        }

        private bool LoadContract(string targetPath)
        {
            targetPath = targetPath.Replace("\\", "/");
            projectFiles.Clear();

            activeDocumentID = null;

            var extension = Path.GetExtension(targetPath);
            if (extension != ".avm")
            {
                var language = LanguageSupport.DetectLanguage(targetPath);
                if (language == SourceLanguage.Other)
                {
                    return false;
                }

                var mainEntry = LoadFile(targetPath);
                if (mainEntry == null)
                {
                    return false;
                }

                var sourceCode = mainEntry.content;

                var avmFilePath = targetPath.Replace(extension, ".avm");

                if (!File.Exists(avmFilePath))
                {
                    if (!_shell.Debugger.CompileContract(sourceCode, language, avmFilePath))
                    {
                        return false;
                    }
                }

                if (!_shell.Debugger.LoadContract(avmFilePath))
                {
                    return false;
                }

                LoadFile(avmFilePath, _shell.Debugger.avmDisassemble.ToString());
            }

            return true;
        }

        private Dictionary<string, object> GetContext()
        {
            var context = new Dictionary<string, object>();

            foreach (var entry in projectFiles.Values)
            {
                entry.active = (entry.id == activeDocumentID);
            }

            context["files"] = this.projectFiles.Values;

            context["activeDocumentID"] = activeDocumentID;

            return context;
        }

        public Backend(ServerSettings serverSettings)
        {
            var settings = new DebuggerSettings();

            var curPath = Directory.GetCurrentDirectory();

            if (!settings.compilerPaths.ContainsKey(SourceLanguage.CSharp))
            {
                settings.compilerPaths[SourceLanguage.CSharp] = curPath + "/compilers";
            }

            if (!settings.compilerPaths.ContainsKey(SourceLanguage.Python))
            {
                settings.compilerPaths[SourceLanguage.Python] = curPath + "/compilers";
            }

            _debugger = new DebugManager(settings);
            _shell = new DebuggerShell(_debugger);

            // initialize a logger
            //var log = new SynkServer.Core.Logger();

            var server = new HTTPServer(serverSettings);

            // instantiate a new site, the second argument is the file path where the public site contents will be found
            //var site = new Site("../public");

            var templateEngine = new TemplateEngine(server, "../views");

            LoadContract(curPath + "/contracts/DefaultContract.cs");

            server.Get("/", (request) =>
            {
                var context = GetContext();

                return templateEngine.Render(context, new string[] { "index" });
            });

            server.Post("/switch", (request) =>
            {
                var code = request.args["code"];
                projectFiles[this.activeDocumentID].content = code;

                this.activeDocumentID = request.args["id"];

                var context = GetContext();

                return "ok";
            });

            server.Get("/content", (request) =>
            {
                var content = projectFiles[activeDocumentID].content;
                return content;
            });

            server.Get("/breakpoint/list", (request) =>
            {
                var breakpoints = projectFiles[activeDocumentID].breakpoints;
                var node = DataNode.CreateArray();
                foreach (var line in breakpoints)
                {
                    var item = DataNode.CreateValue(line);
                    node.AddNode(item);
                }
                return node;
            });

            server.Post("/breakpoint/add", (request) =>
            {
                int line = int.Parse(request.args["line"]);

                var file = projectFiles[activeDocumentID];
                if (_debugger.AddBreakpoint(line, file.path))
                {
                    var breakpoints = file.breakpoints;
                    breakpoints.Add(line);

                    return "ok";
                }

                return "fail";
            });

            server.Post("/breakpoint/remove", (request) =>
            {
                int line = int.Parse(request.args["line"]);

                var file = projectFiles[activeDocumentID];
                var breakpoints = file.breakpoints;

                breakpoints.Remove(line);

                _debugger.RemoveBreakpoint(line, file.path);

                return "ok";
            });

            server.Post("/compile", (request) =>
            {
                var code = request.args["code"];
                request.session.SetString("code", code);

                if (Compile(serverSettings, code))
                {
                    return "OK";
                }

                return "FAIL";
            });

            server.Post("/shell", (request) =>
            {
                var input = request.args["input"];
                var output = DataNode.CreateObject();

                var lines = DataNode.CreateArray("lines");
                output.AddNode(lines);

                if (!_shell.Execute(input, (type, text) =>
                {
                    lines.AddValue(text);
                }))
                {
                    output.AddValue("Invalid command");
                }

                string filePath;
                var curLine = _shell.Debugger.ResolveLine(_shell.Debugger.State.offset, true, out filePath);

                output.AddField("state", _shell.Debugger.State.state);
                output.AddField("offset", _shell.Debugger.State.offset);
                output.AddField("line", curLine);
                output.AddField("path", filePath);

                if (_shell.Debugger.State.state == Emulation.DebuggerState.State.Finished)
                {
                    var val = _debugger.Emulator.GetOutput();

                    _debugger.Blockchain.Save();

                    var methodName = _debugger.Emulator.currentMethod;
                    var hintType = !string.IsNullOrEmpty(methodName) && _debugger.ABI != null && _debugger.ABI.functions.ContainsKey(methodName) ? _debugger.ABI.functions[methodName].returnType : Emulator.Type.Unknown;

                    var temp = FormattingUtils.StackItemAsString(val, false, hintType);
                    output.AddField("result", temp);
                    output.AddField("gas", _debugger.Emulator.usedGas);
                }

                var json = JSONWriter.WriteToString(output);
                return json;
            });

            server.Run();
        }

        public static bool Compile(ServerSettings settings, string code)
        {
            return false;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var settings = ServerSettings.Parse(args);
            settings.Port = 7799;

            var backend = new Backend(settings);
        }
    }

}
