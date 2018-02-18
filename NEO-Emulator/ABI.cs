using LunarParser.JSON;
using System.Collections.Generic;
using System.IO;

namespace Neo.Emulator
{
    public class AVMInput
    {
        public string name;
        public string type;
    }

    public class AVMFunction
    {
        public string name;
        public string returnType;
        public AVMInput[] inputs;
    }

    public class ABI
    {
        public Dictionary<string, AVMFunction> functions = new Dictionary<string, AVMFunction>();
        public AVMFunction entryPoint { get; private set; }
        public readonly string fileName;

        public ABI(string fileName)
        {
            this.fileName = fileName;

            var json = File.ReadAllText(fileName);
            var root = JSONReader.ReadFromString(json);

            var fn = root.GetNode("functions");
            foreach (var child in fn.Children) {
                var f = new AVMFunction();
                f.name = child.GetString("name");
                f.returnType = child.GetString("returnType");

                var p = child.GetNode("parameters");
                if (p != null && p.ChildCount > 0)
                {
                    f.inputs = new AVMInput[p.ChildCount];
                    for (int i=0; i<f.inputs.Length; i++)
                    {
                        var input = new AVMInput();
                        input.name = p[i].GetString("name");
                        input.type = p[i].GetString("type");
                        f.inputs[i] = input;
                    }
                }
                else
                {
                    f.inputs = null;
                }

                functions[f.name] = f;
            }

            entryPoint = functions[root.GetString("entrypoint")];
        }
    }
}
