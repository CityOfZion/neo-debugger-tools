using Neo.Emulation;
using Neo.Emulation.API;
using Neo.Debugger.Core.Data;
using Neo.Debugger.Core.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Neo.Debugger.Dissambler;
using Neo.Debugger.Profiler;
using Neo.Lux.Utils;
using NEO_Emulator.SmartContractTestSuite;
using NeoDebuggerCore.Utils;

namespace Neo.Debugger.Core.Utils
{
    public class DebugManager
    {
        #region Public Members

        //Logging event handler
        public event DebugManagerLogEventHandler SendToLog;
        public delegate void DebugManagerLogEventHandler(object sender, DebugManagerLogEventArgs e);

        //Public props
        public SmartContractTestSuite Tests { get { return _tests; } }
        public Emulator Emulator
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

        public decimal UsedGasCost
        {
            get
            {
                return _emulator.usedGas;
            }
        }

        public bool ResetFlag
        {
            get
            {
                return _resetFlag;
            }
        }
        public DebuggerState Info
        {
            get
            {
                return _state;
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

        public const string inputAVMPath = "Input.avm";

        public string AvmFilePath
        {
            get
            {
                return _avmFilePath;
            }
        }

        public ABI ABI
        {
            get
            {
                return _ABI;
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

        public bool IsMapLoaded
        {
            get
            {
                return _map != null;
            }
        }

        public NeoMapFile Map => _map;

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
                return (_state.state == DebuggerState.State.Exception || _state.state == DebuggerState.State.Break
                    || _map != null && _map.Entries != null ? _map.Entries.Select(x => x.line).Contains(_currentLine + 1) : false) 
                    && _state.state != DebuggerState.State.Finished;
            }
        }

        public bool IsCompiled
        {
            get
            {
                return _isCompiled;
            }
            set
            {
                _isCompiled = value;
            }
        }

        public string CurrentFilePath
        {
            get { return _currentFilePath; }
            set
            {
                _currentFilePath = value;
            }
        }
        private string _currentFilePath;

        #endregion

        //Settings
        public DebuggerSettings Settings { get; private set; }

        //File load flag
        private bool _avmFileLoaded;

        //Debugger State
        private bool _resetFlag;
        private int _currentLine;
        private DebuggerState _state;
        public static readonly string TempContractName = "TempContract";

        //Debugging Emulator and Content
        private Emulator _emulator { get; set; }
        private Dictionary<string, string> _debugContent = new Dictionary<string, string>();
        private ABI _ABI { get; set; }
        private NeoMapFile _map { get; set; }
        private bool _isCompiled { get; set; }
		private Dictionary<byte[], AVMDisassemble> _disassembles = new Dictionary<byte[], AVMDisassemble>(new ByteArrayComparer());

		public AVMDisassemble avmDisassemble { get; private set; }

        //Profiler context
        public ProfilerContext profiler { get; private set; }

        //Context
        private string _contractName { get; set; }
        private byte[] _contractByteCode { get; set; }
        private Account _contractAddress
        {
            get
            {
                return _emulator.blockchain.FindAddressByName(_contractName);
            }
        }

        //Tests
        private SmartContractTestSuite _tests { get; set; }

        //File paths
        private string _avmFilePath { get; set; }
        private string _oldMapFilePath
        {
            get
            {
                return _avmFilePath.Replace(".avm", ".neomap");
            }
        }

        private string _mapFilePath
        {
            get
            {
                return _avmFilePath.Replace(".avm", ".debug.json");
            }
        }

        private string _abiFilePath
        {
            get
            {
                return _avmFilePath.Replace(".avm", ".abi.json");
            }
        }

        private string _blockchainFilePath;

		public DebugManager() : this(new DebuggerSettings())
		{
		}

        public DebugManager(DebuggerSettings settings)
        {
            Settings = settings;
            this.profiler = new ProfilerContext();
        }

        public void Clear()
        {
            _isCompiled = false;
            _map = null;
            _avmFileLoaded = false;
            _avmFilePath = null;
        }

        public string GetContentFor(string path)
        {
            if (_debugContent.ContainsKey(path))
            {
                return _debugContent[path];
            }

            throw new ArgumentException("Invalid path: " + path);
        }

        public void SetContentFor(string path, string content)
        {
            _debugContent[path] = content;
        }

        public bool LoadAvmFile(string avmPath)
        {
            //Decide what we need to open
            if (!String.IsNullOrEmpty(avmPath)) //use the explicit file provided
                _avmFilePath = avmPath;
            else if (!String.IsNullOrEmpty(Settings.lastOpenedFile)) //fallback to last opened
                _avmFilePath = Settings.lastOpenedFile;
            else
                return false; //We don't know what to open, just let the user specify with another call

            //Housekeeping - let's find out what files we have and make sure we're good
            if (!File.Exists(_avmFilePath))
            {
                Log("File not found. " + avmPath);
                return false;
            }

            _debugContent.Clear();

            _contractName = Path.GetFileNameWithoutExtension(_avmFilePath);
            _contractByteCode = File.ReadAllBytes(_avmFilePath);
            _map = new NeoMapFile();
            try
            {
                avmDisassemble = NeoDisassembler.Disassemble(_contractByteCode);
                _disassembles[_contractByteCode] = avmDisassemble;
            }
            catch (DisassembleException e)
            {
                Log($"Disassembler Error: {e.Message}");
                return false;
            }

            if (File.Exists(_abiFilePath))
            {
                _ABI = new ABI(_abiFilePath);
            }
            else
            {
                _ABI = new ABI();
                Log($"Warning: {_abiFilePath} was not found. Please recompile your AVM with the latest compiler.");
            }

            //Let's see if we have source code we can map
            if (File.Exists(_mapFilePath))
            {
                _map.LoadFromFile(_mapFilePath, _contractByteCode);
            }
            else
            {
                _map = null;

                if (File.Exists(_oldMapFilePath))
                {
                    Log("Old map file format found.  Please recompile your avm with the latest compiler.");
                }
                else
                {
                    Log($"Warning: Could not find {_mapFilePath}");
                }
            }

            if (_map != null)
            {
                foreach (var entry in _map.FileNames)
                {
                    if (string.IsNullOrEmpty(entry))
                    {
                        continue;
                    }

                    if (!File.Exists(entry))
                    {
                        Log($"Warning: Could not load the source code, check that this file exists: {entry}");
                        continue;
                    }

                    var sourceCode = File.ReadAllText(entry);
                    _debugContent[entry] = sourceCode;
                }
            }

            //We always should have the assembly content
            _debugContent[_avmFilePath] = avmDisassemble.ToString();

            //Save the settings
            Settings.lastOpenedFile = avmPath;
            Settings.Save();
            _avmFileLoaded = true;

            //Force a reset now that we're loaded
            _resetFlag = true;

            _currentFilePath = avmPath;

            return true;
        }

        public bool LoadContract(string avmFilePath, string chainPath = null)
        {
            if (!LoadAvmFile(avmFilePath))
                return false;

            if (!LoadEmulator(chainPath))
                return false;

            if (!DeployContract())
                return false;

            LoadTests();

            this.IsCompiled = true;

            if (IsMapLoaded && Map.FileNames.Any())
            {
                this.CurrentFilePath = Map.FileNames.FirstOrDefault();
            }

            return true;
        }

        public bool LoadEmulator(string chainPath)
        {
            if (string.IsNullOrEmpty(chainPath))
            {
                chainPath = _avmFilePath.Replace(".avm", ".chain.json");
            }

            _blockchainFilePath = chainPath;

            if (_emulator == null || _blockchainFilePath != _emulator.blockchain.fileName)
            {
                //Create load the emulator
                var blockchain = new Blockchain();
                blockchain.Load(_blockchainFilePath);

                _emulator = new Emulator(blockchain);
            }

            _emulator.OnStep = OnEmulatorStep;

            return true;
        }

        private void OnEmulatorStep(Emulator.EmulatorStepInfo info)
        {
            int lineNumber;
            string filePath;
            string sourceCode;
            try
            {
                lineNumber = ResolveLine(info.offset, true, out filePath);
                sourceCode = GetContentFor(filePath);

                filePath = Path.GetFileName(filePath);
            }
            catch
            {
                lineNumber = 0;
                filePath = null;
                sourceCode = "";
            }

            if (string.IsNullOrEmpty(filePath))
            {
                filePath = "Unknown";
            }

            if (lineNumber >= 0)
            {
                this.profiler.TallyOpcode(info.opcode, info.gasCost, lineNumber, filePath, sourceCode, info.sysCall);
            }
        }

        public bool DeployContract()
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
            if (!address.byteCode.SequenceEqual(_contractByteCode))
            {
                address.byteCode = _contractByteCode;
                Log($"Updated bytecode for {_contractName} on virtual blockchain.");
            }

            _emulator.SetExecutingAccount(_contractAddress);
            return true;
        }

        public bool LoadTests()
        {
            try
            {
                _tests = new SmartContractTestSuite(_avmFilePath);
                return true;
            }
            catch(Exception ex)
            {
                Log("Error loading JSON tests. " + ex.Message);
                return false;
            }
        }


        public int ResolveLine(int ofs, bool useMap, out string filePath)
        {
            if (useMap)
            {
                var line = _map.ResolveLine(ofs, out filePath);
                return line - 1;
            }
            else
            {
                AVMDisassemble disasm;

                var executingBytecode = _emulator.GetExecutingByteCode();
                if (executingBytecode == null)
                {
                    throw new Exception("Cannot resolve line");
                }

                if (executingBytecode.SequenceEqual(_emulator.ContractByteCode))
                {
                    filePath = AvmFilePath;
                }
                else
                {
                    filePath = inputAVMPath;
                }

                if (_disassembles.ContainsKey(executingBytecode))
                {
                    disasm = _disassembles[executingBytecode];
                }
                else
                {
                    disasm = NeoDisassembler.Disassemble(executingBytecode);
                    _disassembles[executingBytecode] = disasm;
                    _debugContent[filePath] = disasm.ToString();
                }

                var line = disasm.ResolveLine(ofs);

                return line;
            }
        }

        public int ResolveOffset(int line, string filePath)
        {
            try
            {
                if (filePath == _avmFilePath)
                {
                    var ofs = avmDisassemble.ResolveOffset(line);
                    return ofs;
                }
                else
                {
                    var ofs = _map.ResolveStartOffset(line + 1, filePath);
                    return ofs;
                }
            }
            catch
            {
                return -1;
            }
        }

        public List<int> GetBreakPointLineNumbers(string fileName)
        {
            List<int> breakpointLineNumbers = new List<int>();
            if (_emulator == null)
                return breakpointLineNumbers;

            bool useMap = fileName != AvmFilePath;
            foreach (var ofs in _emulator.Breakpoints)
            {
                string temp;
                var line = ResolveLine(ofs, useMap, out temp);

                if (temp != fileName)
                {
                    continue;
                }

                if (line >= 0)
                {
                    breakpointLineNumbers.Add(line);
                }
            }

            return breakpointLineNumbers;
        }



        public class Breakpoint
        {
            public string filePath;
            public int lineNumber;
            public int offset;
        }

        public IEnumerable<Breakpoint> Breakpoints => _breakpoints;
        private List<Breakpoint> _breakpoints = new List<Breakpoint>();

        // NOTE line Number starts at zero not 1, so make sure if using a "logical/visual line number" you subtract 1 from it before passing it here
        public bool AddBreakpoint(int lineNumber, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = _currentFilePath;
            }

            var ofs = ResolveOffset(lineNumber, fileName);
            if (ofs < 0)
                return false;

            _emulator.SetBreakpointState(ofs, true);

            _breakpoints.Add(new Breakpoint() { filePath = fileName, lineNumber = lineNumber, offset = ofs });

            return true;
        }

        public bool RemoveBreakpoint(int lineNumber, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = _currentFilePath;
            }

            _breakpoints.RemoveAll(x => x.lineNumber == lineNumber && x.filePath == fileName);

            var ofs = ResolveOffset(lineNumber, fileName);
            if (ofs < 0)
                return false;

            _emulator.SetBreakpointState(ofs, false);

            return true;
        }

        public bool HasBreakpoint(int lineNumber, string fileName = null)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = _currentFilePath;
            }

            foreach (var entry in _breakpoints)
            {
                if (entry.lineNumber == lineNumber && entry.filePath == fileName)
                {
                    return true;
                }
            }

            return false;
        }

        public void Run()
        {
            if (_resetFlag)
                Reset();

            _state = _emulator.Run();
            UpdateState();
        }

		public List<object> RunSequence(string sequenceName)
		{
			var resultList = new List<object>();
			var testSequence = _tests.sequences[sequenceName];
			var debugParams = new DebugParameters();
			foreach (var testItem in testSequence.Items)
			{
                //TODO: Refactoring required 
				debugParams.PrivateKey = testItem.TestPrivateKey;
                var keyPair = DebuggerUtils.GetKeyFromString(debugParams.PrivateKey);
                if (keyPair != null)
                {
                    Runtime.invokerKeys = keyPair;
                }
                debugParams.WitnessMode = CheckWitnessMode.Default;
				var testCase = _tests.cases[testItem.TestName];
				debugParams.ArgList = testCase.args;
				debugParams.TriggerType = TriggerType.Application;
				ConfigureDebugParameters(debugParams);
				Run();
				var result = _emulator.GetOutput();
				resultList.Add(result);
			}
			return resultList;
		}

        public void Step()
        {
            if (_resetFlag)
                Reset();

            //STEP
            _state = Emulator.Step();
            UpdateState();

            if (_resetFlag)
                Reset();
        }

        public void UpdateState()
        {
            var useMap = (_currentFilePath != AvmFilePath && _currentFilePath != inputAVMPath);

            try
            {
                _currentLine = ResolveLine(_state.offset, useMap, out _currentFilePath);
            }
            catch
            {
                // ignore
            }

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

        public bool ConfigureDebugParameters(DebugParameters debugParams)
        {
            //Save all the params for settings later
            Settings.lastPrivateKey = debugParams.PrivateKey;
            Settings.lastParams.Clear();
            foreach (var param in debugParams.DefaultParams)
                Settings.lastParams.Add(param.Key, param.Value);
            Settings.Save();

            //Set the emulator context
            _emulator.checkWitnessMode = debugParams.WitnessMode;
            _emulator.currentTrigger = debugParams.TriggerType;
            _emulator.timestamp = debugParams.Timestamp;
            if (debugParams.Transaction.Count > 0)
            {
                var transaction = debugParams.Transaction.First();
                _emulator.SetTransaction(transaction.Key, transaction.Value);
            }

            try
            {
                var inputs = debugParams.ArgList;
                byte[] loaderScript = debugParams.RawScript;

                if (loaderScript == null)
                {
                    loaderScript = _emulator.GenerateLoaderScriptFromInputs(inputs, this.ABI);
                }

                var methodName = inputs != null && inputs.ChildCount > 0 ? inputs[0].Value : null;

                _emulator.Reset(loaderScript, this.ABI, methodName);
            }
            catch (Exception ex)
            {
                Log("Error during configuration. " + ex.Message);
                return false;
            }

            Reset();
            return true;
        }

        public bool CompileContract(string sourceCode, SourceLanguage language, string outputFile = null)
        {
            var compiler = NeonCompiler.GetInstance(Settings);
            compiler.SendToLog += Compiler_SendToLog;

            var extension = LanguageSupport.GetExtension(language);

            var sourceFile = TempContractName + extension;

            string fileName;

            Directory.CreateDirectory(Settings.path);
            if (outputFile == null)
            {
                fileName = Path.Combine(Settings.path, sourceFile);
            }
            else
            {
                fileName = Path.Combine(Settings.path, outputFile.Replace(".avm", extension));
            }

            var avmPath = fileName.Replace(extension, ".avm");

            string abiFile = avmPath.Replace(".avm", ".abi.json");
            string debugMapFile = avmPath.Replace(".avm", ".debug.json");

            try
            {
                File.Delete(abiFile);
                File.Delete(debugMapFile);
            }
            catch(Exception ex)
            {
                Log("Error during deletion. " + ex.Message);
            }

            bool success = compiler.CompileContract(sourceCode, fileName, language);

            if (success)
            {
                _avmFilePath = avmPath;

                /*if (outputFile != null)
                {
                    File.Copy(_avmFilePath, outputFile, true);

                    if (File.Exists(abiFile))
                    {
                        File.Copy(abiFile, outputFile.Replace(".avm", ".abi.json"), true);
                    }

                    if (File.Exists(debugMapFile))
                    {
                        File.Copy(debugMapFile, outputFile.Replace(".avm", ".debug.json"), true);
                    }
                }*/
            }

            _isCompiled = success;
            return success;
        }

        private void Compiler_SendToLog(object sender, CompilerLogEventArgs e)
        {
            Log("Compiler: " + e.Message);
        }

        public void LoadAssignmentsFromContent(string path)
        {
            var content = this.GetContentFor(path);
            var lang = LanguageSupport.DetectLanguage(path);
            var assignments = InspectorSupport.ParseAssigments(content, lang);
            foreach (var entry in assignments)
            {
                try
                {
                    var ofs = this.Map.ResolveEndOffset(entry.Key, path);
                    this.Emulator.AddAssigment(ofs, entry.Value.name, entry.Value.type);
                }
                catch(Exception ex)
                {
                    Log("Error loading assignments. " + ex.Message);
                    continue;
                }
            }
        }

    }
}
