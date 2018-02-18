using Neo.Emulator.Dissambler;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Neo.Compiler.AVM
{
    public static class DebugInfo
    {
        public static MyJson.JsonNode_Object ExportDebugInfo(string avmName, NeoModule module)
        {
            var outjson = new MyJson.JsonNode_Object();

            var debugMap = new List<DebugMapEntry>();
            DebugMapEntry currentDebugEntry = null;

            var fileMap = new Dictionary<string, int>();

            List<byte> bytes = new List<byte>();
            foreach (var c in module.total_Codes.Values)
            {
                if (c.debugcode != null && c.debugline > 0 && c.debugline < 2000)
                {
                    currentDebugEntry = new DebugMapEntry();
                    currentDebugEntry.startOfs = debugMap.Count > 0 ? bytes.Count : 0;
                    currentDebugEntry.endOfs = currentDebugEntry.startOfs;
                    currentDebugEntry.url = c.debugcode;
                    currentDebugEntry.line = c.debugline;

                    if (!fileMap.ContainsKey(c.debugcode))
                    {
                        fileMap[c.debugcode] = fileMap.Count + 1;
                    }

                    debugMap.Add(currentDebugEntry);
                }
                else
                if (currentDebugEntry != null)
                {
                    currentDebugEntry.endOfs = bytes.Count;
                }
                bytes.Add((byte)c.code);
                if (c.bytes != null)
                    for (var i = 0; i < c.bytes.Length; i++)
                    {
                        bytes.Add(c.bytes[i]);
                    }

            }

            var hash = CalculateMD5(bytes.ToArray());

            string compilerName = System.AppDomain.CurrentDomain.FriendlyName.ToLowerInvariant();
            var version = Assembly.GetEntryAssembly().GetName().Version.ToString();

            var avmInfo = new MyJson.JsonNode_Object();
            avmInfo.Add("name", new MyJson.JsonNode_ValueString(avmName));
            avmInfo.Add("hash", new MyJson.JsonNode_ValueString(hash));

            var compilerInfo = new MyJson.JsonNode_Object();
            compilerInfo.Add("name", new MyJson.JsonNode_ValueString(compilerName));
            compilerInfo.Add("version", new MyJson.JsonNode_ValueString(version));

            var fileInfo = new MyJson.JsonNode_Array();
            foreach (var entry in fileMap)
            {
                var fileEntry = new MyJson.JsonNode_Object();
                fileEntry.Add("id", new MyJson.JsonNode_ValueNumber(entry.Value));
                fileEntry.Add("url", new MyJson.JsonNode_ValueString(entry.Key));
                fileInfo.AddArrayValue(fileEntry);
            }

            var mapInfo = new MyJson.JsonNode_Array();
            foreach (var entry in debugMap)
            {
                if (!fileMap.ContainsKey(entry.url))
                {
                    continue;
                }

                var fileID = fileMap[entry.url];

                var mapEntry = new MyJson.JsonNode_Object();
                mapEntry.Add("start", new MyJson.JsonNode_ValueNumber(entry.startOfs));
                mapEntry.Add("end", new MyJson.JsonNode_ValueNumber(entry.endOfs));
                mapEntry.Add("file", new MyJson.JsonNode_ValueNumber(fileID));
                mapEntry.Add("line", new MyJson.JsonNode_ValueNumber(entry.line));
                mapInfo.AddArrayValue(mapEntry);
            }

            outjson["avm"] = avmInfo;
            outjson["compiler"] = compilerInfo;
            outjson["files"] = fileInfo;
            outjson["map"] = mapInfo;

            return outjson;
        }

        private static string CalculateMD5(byte[] inputBytes)
        {
            // step 1, calculate MD5 hash from input
            var md5 = System.Security.Cryptography.MD5.Create();

            byte[] hash = md5.ComputeHash(inputBytes);

            // step 2, convert byte array to hex string
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

    }
}
