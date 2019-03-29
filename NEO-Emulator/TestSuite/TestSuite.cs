using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LunarLabs.Parser;
using LunarLabs.Parser.JSON;
using Neo.Emulation;

namespace NEO_Emulator.SmartContractTestSuite
{
	public class SmartContractTestSuite
	{
		public Dictionary<string, TestCase> cases = new Dictionary<string, TestCase>();
		public Dictionary<string, TestSequence> sequences = new Dictionary<string, TestSequence>();

		public SmartContractTestSuite()
		{

		}

		public SmartContractTestSuite(string fileName)
		{
			fileName = fileName.Replace(".avm", ".test.json");

			if (File.Exists(fileName))
			{
				var json = File.ReadAllText(fileName);
				LoadFromJson(json);
			}
		    else
            {
                if (cases.Count == 0) // if there are no test cases, create tests "name" and "symbol"
                {
                    var nullParam = DataNode.CreateArray();
                    nullParam.AddValue(0);
                    var nameParams = DataNode.CreateArray("params");
                    nameParams.AddValue("name");
                    nameParams.AddNode(nullParam);
                    cases.Add("NEP-5_name", new TestCase("NEP-5_name", "Main", nameParams));
                    var symbolParams = DataNode.CreateArray("params");
                    symbolParams.AddValue("symbol");
                    symbolParams.AddNode(nullParam);
                    cases.Add("NEP-5_symbol", new TestCase("NEP-5_symbol", "Main", symbolParams));
                }

                var json = GenerateJson();
                File.WriteAllText(fileName, json);
            }
		}
		
		public void LoadFromJson(string json)
		{
			var root = JSONReader.ReadFromString(json);

			var casesNode = root["cases"];
			foreach (var node in casesNode.Children)
			{
				var entry = TestCase.FromNode(node);
				cases[entry.name] = entry;
			}

			if (root.HasNode("sequences"))
			{
				var sequencesNode = root["sequences"];
				foreach (var node in sequencesNode.Children)
				{
					var entry = new TestSequence(node);
					sequences[entry.SequenceName] = entry;
				}
				
			}
		}
        
        public string GenerateJson()
        {
            var node = DataNode.CreateObject();

            var testCases = DataNode.CreateArray("cases");
            foreach (var test in cases)
            {
                var tcase = DataNode.CreateObject();
                tcase.AddField("name", test.Value.name);
                tcase.AddField("method", test.Value.method);
                tcase.AddNode(test.Value.args);

                testCases.AddNode(tcase);
            }
            node.AddNode(testCases);

            var testSequences = DataNode.CreateArray("sequences");
            foreach (var test in sequences)
            {
                var tcase = DataNode.CreateObject();
                tcase.AddField("sequenceName", test.Value.SequenceName);
                tcase.AddField("resetBlockchain", test.Value.ResetBlockchain);
                var items = DataNode.CreateArray("items");
                foreach (var item in test.Value.Items)
                {
                    var sequenceItem = DataNode.CreateObject();
                    if (item.TestPrivateKey != null)
                    {
                        sequenceItem.AddField("testPrivateKey", item.TestPrivateKey);
                    }
                    sequenceItem.AddField("testName", item.TestName);
                    if(item.Result != null)
                    {
                        sequenceItem.AddNode(item.Result);
                    }

                    items.AddNode(sequenceItem);
                }
                tcase.AddNode(items);

                testSequences.AddNode(tcase);
            }
            node.AddNode(testSequences);

            var json = JSONWriter.WriteToString(node);

            return json;
        }
	}
}
