using System;
using SynkServer.Core;
using System.IO;
using Microsoft.CodeAnalysis;
using Mono.Cecil;
using SynkServer.HTTP;
using SynkServer.Templates;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp;

namespace NEO.Ide
{
    class Backend
    {
        static void Main(string[] args)
        {
            // initialize a logger
            var log = new SynkServer.Core.Logger();

            // either parse the settings from the program args or initialize them manually
            var settings = ServerSettings.Parse(args);

            var server = new HTTPServer(log, settings);

            var templateEngine = new TemplateEngine(server, "../views");

            // instantiate a new site, the second argument is the file path where the public site contents will be found
            var site = new Site(server, "../public");

            var base_code = File.ReadAllText(settings.path + "/../DefaultContract.cs");        

            site.Get("/", (request) =>
            {
                var context = new Dictionary<string, object>();
                context["code"] = request.session.Get<string>("code", base_code);

                return templateEngine.Render(site, context, new string[] { "index" });
            });

            site.Post("/compile", (request) =>
            {
                var code = request.args["code"];
                request.session.Set("code", code);

                if (Compile(settings, code))
                {
                    return "OK";
                }

                return "FAIL";
            });

            server.Run();
        }

        public static bool Compile(ServerSettings settings, string code)
        {
            var MaxLanguageVersion = Enum
            .GetValues(typeof(LanguageVersion))
            .Cast<LanguageVersion>()
            .Max();

            var options = new CSharpParseOptions(kind: SourceCodeKind.Regular, languageVersion: MaxLanguageVersion);
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(code, options);

            
            var trustedAssembliesPaths = ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")).Split(Path.PathSeparator);
            var neededAssemblies = new[]
            {
            "mscorlib",
            "System.Runtime",
            "System.Core",
            "System.Numerics",
            "Neo.SmartContract.Framework",
            };

            var references = trustedAssembliesPaths
                .Where(p => neededAssemblies.Contains(Path.GetFileNameWithoutExtension(p)))
                .Select(p => MetadataReference.CreateFromFile(p))
                .ToList();

            var assemblyName = "Test.dll";

            var compileOptions = new CSharpCompilationOptions(
                           OutputKind.DynamicallyLinkedLibrary,
                           optimizationLevel: OptimizationLevel.Debug,
                           allowUnsafe: true);

            Compilation compilation = CSharpCompilation.Create(assemblyName, options: compileOptions, references: references, syntaxTrees: new [] { syntaxTree});

            var stream = new MemoryStream();
            var emitResult = compilation.Emit(stream);

            if (emitResult.Success)
            {
                stream.Seek(0, SeekOrigin.Begin);
                AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(stream);
                return true;
            }
            else
            {
                foreach (var d in emitResult.Diagnostics)
                {
                    var lineSpan = d.Location.GetLineSpan();
                    var startLine = lineSpan.StartLinePosition.Line;
                    Console.WriteLine("Line {0}: {1}", startLine, d.GetMessage());
                }
                return false;
            }
        }

    }
}
