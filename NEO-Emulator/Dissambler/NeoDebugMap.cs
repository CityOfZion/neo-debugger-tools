using System;
using System.Collections.Generic;
using System.IO;
using LunarParser.JSON;
using Neo.Cryptography;

namespace Neo.Emulator.Dissambler
{
    public class DebugMapEntry
    {
        public string url;
        public int line;

        public int startOfs;
        public int endOfs;

        public override string ToString()
        {
            return "Line "+this.line+" at "+url;
        }
    }

    public class NeoMapFile
    {
        private List<DebugMapEntry> _entries = new List<DebugMapEntry>();
        public IEnumerable<DebugMapEntry> Entries { get { return _entries; } }

        public string contractName { get; private set; }

        public void LoadFromFile(string path, byte[] bytes)
        {
            if (!File.Exists(path))
            {
                throw new FileNotFoundException();
            }

            var json = File.ReadAllText(path);
            var root = JSONReader.ReadFromString(json);

            var avmInfo = root["avm"];
            if (avmInfo != null)
            {
                var curHash = bytes.MD5().ToLowerInvariant();
                var oldHash = avmInfo.GetString("hash").ToLowerInvariant();

                if (curHash != oldHash)
                {
                    throw new Exception("Hash mismatch, please recompile the code to get line number info");
                }

                this.contractName = avmInfo.GetString("name");
            }
            else
            {
                this.contractName = Path.GetFileNameWithoutExtension(path);
            }

            var files = new Dictionary<int, string>();
            var fileNode = root["files"];
            foreach (var temp in fileNode.Children)
            {
                files[temp.GetInt32("id")] = temp.GetString("url");
            }

            _entries = new List<DebugMapEntry>();
            var mapNode = root["map"];
            foreach (var temp in mapNode.Children)
            {
                int fileID = temp.GetInt32("file");

                if (!files.ContainsKey(fileID))
                {
                    throw new Exception("Error loading map file, invalid file entry");
                }

                var entry = new DebugMapEntry();
                entry.startOfs = temp.GetInt32("start");
                entry.endOfs = temp.GetInt32("end");
                entry.line = temp.GetInt32("line");
                entry.url = files[fileID];
                _entries.Add(entry);
            }
        }

        /// <summary>
        /// Calculates the source code line that maps to the specificed script offset.
        /// </summary>
        public int ResolveLine(int ofs)
        {
            foreach (var entry in this.Entries)
            {
                if (ofs >= entry.startOfs && ofs <= entry.endOfs)
                {
                    return entry.line;
                }
            }

            throw new Exception("Offset cannot be mapped");
        }

        /// <summary>
        /// Calculates the script offset that maps to the specificed source code line 
        /// </summary>
        public int ResolveOffset(int line)
        {
            foreach (var entry in this.Entries)
            {
                if (entry.line == line)
                {
                    return entry.startOfs;
                }
            }

            throw new Exception("Line cannot be mapped");
        }

    }

}
