using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neo.Emulator.Profiler
{
    public class SourceFileLine
    {
        public string _filename;
        public int _lineno;

        public SourceFileLine(string filename, int lineno)
        {
            _filename = filename;
            _lineno = lineno;
        }
    }

    public class SourceStmtInfo
    {
        public SourceFileLine _filelineo;
        public string _sourceStmt;
        public int[] _stmtOpcodeCount = new int[256];
        public double[] _stmtOpcodeCost = new double[256];
    }

    public class ProfilerContext
    {
        public string _filename;
        public string[] _source = { "" };
        public int _lineno;
        public string _sourceString = "";

        public string[] opcodeNames = new string[256];
        public double[] opcodeCosts = new double[256];
        public bool[] opcodeUsed = new bool[256];
        public Dictionary<string, SourceStmtInfo> dictStmtInfo;
        public int[] totalTallyByOpcode = new int[256];
        public double[] totalCostByOpcode = new double[256];

        public ProfilerContext()
        {
            dictStmtInfo = new Dictionary<string, SourceStmtInfo>();
        }

        public void SetFilenameSource(string filename, string source)
        {
            _filename = filename;
            if (!String.IsNullOrEmpty(source))
            {
                _source = source.Split('\n');
            }
        }

        public void SetLineno(int lineno)
        {
            if (lineno >= 0)
            {
                _lineno = lineno;
                _sourceString = _source[lineno];
            }
        }

        public void TallyOpcode(Neo.VM.OpCode opcode, double opCost)
        {
            SourceFileLine sfl = new SourceFileLine(_filename, _lineno);

            string key = _filename + ":" + _lineno.ToString();
            if (dictStmtInfo.Keys.Contains(key))
            {
                SourceStmtInfo ssi;
                dictStmtInfo.TryGetValue(key, out ssi);
                ssi._stmtOpcodeCount[(int)opcode]++;
                ssi._stmtOpcodeCost[(int)opcode] = ssi._stmtOpcodeCount[(int)opcode] * opcodeCosts[(int)opcode];
            }
            else
            {
                SourceStmtInfo ssi = new SourceStmtInfo();
                ssi._filelineo = sfl;
                ssi._sourceStmt = _sourceString;
                ssi._stmtOpcodeCount[(int)opcode] = 1;
                dictStmtInfo.Add(key, ssi);
            }

            if (!opcodeUsed[(int)opcode])
            {
                opcodeCosts[(int)opcode] = opCost;
                opcodeNames[(int)opcode] = opcode.ToString();
                opcodeUsed[(int)opcode] = true;
            }
        }

        public void DumpCSV()
        {
            string csvfilename = _filename.Replace(".cs", "_cs") + ".csv";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(csvfilename))
            {
                file.WriteLine();
                file.WriteLine("\"Tally by Opcode\"");
                file.WriteLine();

                file.Write("\"" + "" + "\"");
                file.Write(",\"" + "" + "\"");
                file.Write(",\"" + "Opcode (Hex) >" + "\"");
                for (int opcode = 0; opcode < 256; opcode++)
                {
                    if (opcodeUsed[opcode])
                    {
                        file.Write(",\"0x" + opcode.ToString("X2") + "\"");
                    }
                }
                file.WriteLine();
                file.Write("\"" + "" + "\"");
                file.Write(",\"" + "" + "\"");
                file.Write(",\"" + "Opcode >" + "\"");
                for (int opcode = 0; opcode < 256; opcode++)
                {
                    if (opcodeUsed[opcode])
                    {
                        file.Write(",\"" + opcodeNames[opcode] + "\"");
                    }
                }
                file.WriteLine();
                file.Write("\"" + "" + "\"");
                file.Write(",\"" + "" + "\"");
                file.Write(",\"" + "Cost >" + "\"");
                for (int opcode = 0; opcode < 256; opcode++)
                {
                    if (opcodeUsed[opcode])
                    {
                        file.Write(",\"" + opcodeCosts[opcode].ToString() + "\"");
                    }
                    totalTallyByOpcode[opcode] = 0;
                }
                file.WriteLine();

                foreach (SourceStmtInfo ssi in dictStmtInfo.Values)
                {
                    file.Write("\"" + ssi._filelineo._filename + "\"");
                    file.Write(",\"" + ssi._filelineo._lineno.ToString() + "\"");
                    file.Write(",\"" + ssi._sourceStmt.Replace("\"", "''") + "\"");
                    for (int opcode = 0; opcode < 256; opcode++)
                    {
                        if (opcodeUsed[opcode])
                        {
                            file.Write(",\"" + ssi._stmtOpcodeCount[opcode].ToString() + "\"");

                            totalTallyByOpcode[opcode] += ssi._stmtOpcodeCount[opcode];
                        }
                    }
                    file.WriteLine();
                }
                int totalOpcodeTally = 0;
                file.Write("\"" + "" + "\"");
                file.Write(",\"" + "" + "\"");
                file.Write(",\"" + "Tally by Opcode>" + "\"");
                for (int opcode = 0; opcode < 256; opcode++)
                {
                    if (opcodeUsed[opcode])
                    {
                        file.Write(",\"" + totalTallyByOpcode[opcode].ToString() + "\"");
                        totalOpcodeTally += totalTallyByOpcode[opcode];
                    }
                }
                file.WriteLine();
                file.Write("\"" + "" + "\"");
                file.Write(",\"" + "" + "\"");
                file.Write(",\"" + "Total Tally>" + "\"");
                file.Write(",\"" + totalOpcodeTally.ToString() + "\"");
                file.WriteLine();

                file.WriteLine();
                file.WriteLine("\"Costs by Opcode\"");
                file.WriteLine();

                file.Write("\"" + "" + "\"");
                file.Write(",\"" + "" + "\"");
                file.Write(",\"" + "Opcode (Hex) >" + "\"");
                for (int opcode = 0; opcode < 256; opcode++)
                {
                    if (opcodeUsed[opcode])
                    {
                        file.Write(",\"0x" + opcode.ToString("X2") + "\"");
                    }
                }
                file.WriteLine();
                file.Write("\"" + "" + "\"");
                file.Write(",\"" + "" + "\"");
                file.Write(",\"" + "Opcode >" + "\"");
                for (int opcode = 0; opcode < 256; opcode++)
                {
                    if (opcodeUsed[opcode])
                    {
                        file.Write(",\"" + opcodeNames[opcode] + "\"");
                    }
                }
                file.WriteLine();
                file.Write("\"" + "" + "\"");
                file.Write(",\"" + "" + "\"");
                file.Write(",\"" + "Cost >" + "\"");
                for (int opcode = 0; opcode < 256; opcode++)
                {
                    if (opcodeUsed[opcode])
                    {
                        file.Write(",\"" + opcodeCosts[opcode].ToString() + "\"");
                    }
                    totalCostByOpcode[opcode] = 0;
                }
                file.WriteLine();

                foreach (SourceStmtInfo ssi in dictStmtInfo.Values)
                {
                    file.Write("\"" + ssi._filelineo._filename + "\"");
                    file.Write(",\"" + ssi._filelineo._lineno.ToString() + "\"");
                    file.Write(",\"" + ssi._sourceStmt.Replace("\"", "''") + "\"");
                    for (int opcode = 0; opcode < 256; opcode++)
                    {
                        if (opcodeUsed[opcode])
                        {
                            file.Write(",\"" + ssi._stmtOpcodeCost[opcode].ToString() + "\"");

                            totalCostByOpcode[opcode] += ssi._stmtOpcodeCost[opcode];
                        }
                    }
                    file.WriteLine();
                }
                double totalOpcodeCost = 0;
                file.Write("\"" + "" + "\"");
                file.Write(",\"" + "" + "\"");
                file.Write(",\"" + "Costs by Opcode>" + "\"");
                for (int opcode = 0; opcode < 256; opcode++)
                {
                    if (opcodeUsed[opcode])
                    {
                        file.Write(",\"" + totalCostByOpcode[opcode].ToString() + "\"");
                        totalOpcodeCost += totalCostByOpcode[opcode];
                    }
                }
                file.WriteLine();
                file.Write("\"" + "" + "\"");
                file.Write(",\"" + "" + "\"");
                file.Write(",\"" + "Total Cost>" + "\"");
                file.Write(",\"" + totalOpcodeCost.ToString() + "\"");
                file.WriteLine();
            }

        }
    }
}
