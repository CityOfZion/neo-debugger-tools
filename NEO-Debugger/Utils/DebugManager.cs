using Neo.Debugger.Data;
using Neo.Debugger.Forms;
using Neo.Debugger.Models;
using Neo.Emulator;
using Neo.Emulator.API;
using Neo.Emulator.Dissambler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Neo.Debugger.Forms.MainForm;

namespace Neo.Debugger.Utils
{
    public class DebugManager
    {
        #region Public Members

        //Logging event handler
        public event DebugManagerLogEventHandler SendToLog;
        public delegate void DebugManagerLogEventHandler(object sender, DebugManagerLogEventArgs e);

        //Public props
        public TestSuite Tests { get { return _tests; } }
        public NeoEmulator Emulator
        {
            get
            {
                return _emulator;
            }
        }
        public Blockchain Blockchain
        {
            get
            {
                return EmulatorLoaded ? _emulator.blockchain : null;
            }
        }
        public double UsedGasCost
        {
            get
            {
                return _emulator.GetUsedGas();
            }
        }
        public DebugMode Mode
        {
            get
            {
                return _mode;
            }
        }
        public bool ResetFlag
        {
            get
            {
                return _resetFlag;
            }
        }
        public DebuggerState.State State
        {
            get
            {
                return _state.state;
            }
        }
        public int Offset
        {
            get
            {
                return _state.offset;
            }
        }
        public int CurrentLine
        {
            get
            {
                return _currentLine;
            }
        }
        public SourceLanguage Language
        {
            get
            {
                return _language;
            }
        }
        public Dictionary<DebugMode, string> DebugContent
        {
            get
            {
                return _debugContent;
            }
        }
        public string AvmFileName
        {
            get
            {
                return Path.GetFileName(_avmFilePath);
            }
        }
        public ABI ABI
        {
            get
            {
                return _aBI;
            }
        }
        public string ContractName
        {
            get
            {
                return _contractName;
            }
        }

        //Public Load state properties
        public bool AvmFileLoaded
        {
            get
            {
                return _avmFileLoaded;
            }
        }

        public bool EmulatorLoaded
        {
            get
            {
                return _emulator != null;
            }
        }

        public bool BlockchainLoaded
        {
            get
            {
                return Blockchain != null;
            }
        }

        public bool MapLoaded
        {
            get
            {
                return _map != null;
            }
        }

        public bool SmartContractDeployed
        {
            get
            {
                return Blockchain != null && _contractAddress != null;
            }
        }

        public bool IsSteppingOrOnBreakpoint
        {
            get
            {
                return (_currentLine > 0 && (_state.state != DebuggerState.State.Running || _state.state == DebuggerState.State.Break));
            }
        }

        #endregion

        //Settings
        private Settings _settings;

        //File load flag
        private bool _avmFileLoaded;

        //Debugger State
        private bool _resetFlag;
        private int _currentLine;
        private DebugMode _mode;
        private DebuggerState _state;
        private SourceLanguage _language;

        //Debugging Emulator and Content
        private NeoEmulator _emulator { get; set; }
        private Dictionary<DebugMode, string> _debugContent { get; set; }
        private ABI _aBI { get; set; }
        private NeoMapFile _map { get; set; }
        private AVMDisassemble _avmAsm { get; set; }

        //Context
        private string _contractName { get; set; }
        private byte[] _contractByteCode { get; set; }
        private Address _contractAddress
        {
            get
            {
                return _emulator.blockchain.FindAddressByName(_contractName);
            }
        }

        //Tests
        private TestSuite _tests { get; set; }
        
        //File paths
        private string _avmFilePath { get; set; }
        private string _oldMapFilePath
        {
            get
            {
                return _avmFilePath.Replace(".avm", ".neomap");
            }
        }

        private string _mapFilePath {
            get
            {
                return _avmFilePath.Replace(".avm", ".debug.json");
            }
        }

        private string _abiFilePath {
            get
            {
                return _avmFilePath.Replace(".avm", ".abi.json");
            }
        }

        private string _blockchainFilePath {
            get
            {
                return _avmFilePath.Replace(".avm", ".chain");
            }
        }

        public string srcFileName;

        public DebugManager(Settings settings)
        {
            _settings = settings;
        }

        public bool LoadAvmFile(string avmPath)
        {
            //Decide what we need to open
            if (!String.IsNullOrEmpty(avmPath)) //use the explicit file provided
                _avmFilePath = avmPath;
            else if (!String.IsNullOrEmpty(_settings.lastOpenedFile)) //fallback to last opened
                _avmFilePath = _settings.lastOpenedFile;
            else
                return false; //We don't know what to open, just let the user specify with another call

            //Housekeeping - let's find out what files we have and make sure we're good
            if (!File.Exists(_avmFilePath))
            {
                Log("File not found. " + avmPath);
                return false;
            }            
            else if (File.Exists(_oldMapFilePath))
            {
                Log("Old map file format found.  Please recompile your avm with the latest compiler.");
                return false;
            }

            _debugContent = new Dictionary<DebugMode, string>();
            _mode = DebugMode.Assembly;
            _language = SourceLanguage.Other;
            _contractName = Path.GetFileNameWithoutExtension(_avmFilePath);
            _contractByteCode = File.ReadAllBytes(_avmFilePath);
            _map = new NeoMapFile();
            try
            {
                _avmAsm = NeoDisassembler.Disassemble(_contractByteCode);
            }
            catch (DisassembleException e)
            {
                Log($"Disassembler Error: {e.Message}");
                return false;
            }

            if (File.Exists(_abiFilePath))
            {
                _aBI = new ABI(_abiFilePath);
            }
            else
            {
                Log($"Warning: {_abiFilePath} was not found. Please recompile your AVM with the latest compiler.");
            }

            //We always should have the assembly content
            _debugContent[DebugMode.Assembly] = _avmAsm.ToString();

            //Let's see if we have source code we can map
            if (File.Exists(_mapFilePath))
            {
                _map.LoadFromFile(_mapFilePath, _contractByteCode);
            }
            else
            {
                _map = null;
                Log($"Warning: Could not find {_mapFilePath}");
                //return false;
            }

            if (_map != null && _map.Entries.Any())
            {
                srcFileName = _map.Entries.FirstOrDefault().url;
                if (string.IsNullOrEmpty(srcFileName))
                    throw new Exception("Error: Could not load the debug map correctly, no source file specified in map.");
                if (!File.Exists(srcFileName))
                    throw new Exception($"Error: Could not load the source code, check that this file exists: {srcFileName}");

                _debugContent[DebugMode.Source] = File.ReadAllText(srcFileName);
                _language = LanguageSupport.DetectLanguage(srcFileName);
                _mode = DebugMode.Source;
            }

            //Save the settings
            _settings.lastOpenedFile = avmPath;
            _settings.Save();
            _avmFileLoaded = true;

            //Force a reset now that we're loaded
            _resetFlag = true;

            return true;
        }

        public bool LoadEmulator()
        {
            //Create load the emulator
            Blockchain blockchain = new Blockchain();
            blockchain.Load(_blockchainFilePath);
            _emulator = new NeoEmulator(blockchain);

            _emulator.SetProfilerFilenameSource(srcFileName, _debugContent[DebugMode.Source]);

            return true;
        }

        public bool LoadContract()
        {
            if (String.IsNullOrEmpty(_contractName) || _contractByteCode == null || _contractByteCode.Length == 0)
            {
                return false;
            }

            var address = _emulator.blockchain.FindAddressByName(_contractName);
            if (address == null)
            {
                address = _emulator.blockchain.DeployContract(_contractName, _contractByteCode);
                Log($"Deployed contract {_contractName} on virtual blockchain at address {address.keys.address}.");
            }
            else
            {
                address.byteCode = _contractByteCode;
                Log($"Updated bytecode for {_contractName} on virtual blockchain.");
            }

            _emulator.SetExecutingAddress(_contractAddress);
            return true;
        }

        public bool LoadTests()
        {
            _tests = new TestSuite(_avmFilePath);
            return true;
        }

        public int ResolveLine(int ofs)
        {
            try
            {
                switch (_mode)
                {
                    case DebugMode.Source:
                        {
                            var line = _map.ResolveLine(ofs);
                            _emulator.SetProfilerLineno(line - 1);
                            return line - 1;
                        }

                    case DebugMode.Assembly:
                        {
                            var line = _avmAsm.ResolveLine(ofs);
                            return line + 1;
                        }

                    default:
                        {
                            return -1;
                        }
                }
            }
            catch
            {
                return -1;
            }
        }

        public int ResolveOffset(int line)
        {
            try
            {
                switch (_mode)
                {
                    case DebugMode.Source:
                        {
                            var ofs = _map.ResolveOffset(line + 1);
                            _emulator.SetProfilerLineno(line + 1);
                            return ofs;
                        }

                    case DebugMode.Assembly:
                        {
                            var ofs = _avmAsm.ResolveOffset(line);
                            return ofs;
                        }

                    default: return -1;
                }
            }
            catch
            {
                return -1;
            }
        }

        public void ToggleDebugMode()
        {
            if (_mode == DebugMode.Assembly)
                _mode = DebugMode.Source;
            else
                _mode = DebugMode.Assembly;
        }

        public List<int> GetBreakPointLineNumbers()
        {
            List<int> breakpointLineNumbers = new List<int>();
            if (_emulator == null)
                return breakpointLineNumbers;

            foreach (var ofs in _emulator.Breakpoints)
            {
                var line = ResolveLine(ofs);
                if (line >= 0)
                    breakpointLineNumbers.Add(line);
            }

            return breakpointLineNumbers;
        }

        public bool AddBreakpoint(int lineNumber)
        {
            var ofs = ResolveOffset(lineNumber);
            if (ofs < 0)
                return false;

            _emulator.SetBreakpointState(ofs, true);

            return true;
        }

        public bool RemoveBreakpoint(int lineNumber)
        {
            var ofs = ResolveOffset(lineNumber);
            if (ofs < 0)
                return false;

            _emulator.SetBreakpointState(ofs, false);

            return true;
        }

        public void Run()
        {
            if (_resetFlag)
                Reset();

            _state = _emulator.Run();
            UpdateState();
        }

        public void Step()
        {
            if (_resetFlag)
                Reset();

            //STEP
            _state = Emulator.Step();
            UpdateState();
        }

        public void UpdateState()
        {
            _currentLine = ResolveLine(_state.offset);
            _emulator.SetProfilerLineno(_currentLine);
            switch (_state.state)
            {
                case DebuggerState.State.Finished:
                    _resetFlag = true;
                    _emulator.blockchain.Save(_blockchainFilePath);
                    break;
                case DebuggerState.State.Exception:
                    _resetFlag = true;
                    break;
                case DebuggerState.State.Break:
                    break;
            }
        }

        public void Reset()
        {
            _currentLine = -1;
            _emulator.SetProfilerLineno(_currentLine);
            _resetFlag = false;
        }

        public void Log(string message)
        {
            SendToLog?.Invoke(this, new DebugManagerLogEventArgs
            {
                Error = false,
                Message = message
            });
        }

        public bool SetDebugParameters(DebugParameters debugParams)
        {
            //Save all the params for settings later
            _settings.lastPrivateKey = debugParams.PrivateKey;
            _settings.lastParams.Clear();
            foreach (var param in debugParams.DefaultParams)
                _settings.lastParams.Add(param.Key, param.Value);
            _settings.Save();

            //Set the emulator context
            _emulator.checkWitnessMode = debugParams.WitnessMode;
            _emulator.currentTrigger = debugParams.TriggerType;
            _emulator.timestamp = debugParams.Timestamp;
            if (debugParams.Transaction.Count > 0)
            {
                var transaction = debugParams.Transaction.First();
                _emulator.SetTransaction(transaction.Key, transaction.Value);
            }

            _emulator.Reset(debugParams.ArgList);
            Reset();
            return true;
        }
    }
}
