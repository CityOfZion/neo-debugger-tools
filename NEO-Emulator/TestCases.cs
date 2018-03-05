using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunarParser;
using LunarParser.JSON;

namespace Neo.Emulator
{
    public class TestCase
    {
        public readonly string name;
        public readonly string method;
        public readonly DataNode args;

        public TestCase(string name, string method, DataNode args)
        {
            this.name = name;
            this.method = method;
            this.args = args;
        }

        public static TestCase FromNode(DataNode node)
        {
            var name = node.GetString("name");
            var method = node.GetString("method", null);
            var args = new List<string>();
            var argNode = node.GetNode("params");
            return new TestCase(name, method, argNode);
        }
    }

    public class TestSuite
    {
        public Dictionary<string, TestCase> cases = new Dictionary<string, TestCase>();

        public TestSuite(string fileName)
        {
            fileName = fileName.Replace(".avm", ".test.json");

            if (File.Exists(fileName))
            {
                var json = File.ReadAllText(fileName);
                var root = JSONReader.ReadFromString(json);

                var casesNode = root["cases"];
                foreach (var node in casesNode.Children)
                {
                    var entry = TestCase.FromNode(node);
                    cases[entry.name] = entry;
                }
            }

        }
    }
}
