using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using ScintillaNET;
using Neo.Debugger.Utils;
using System.Text;
using System.Collections.Generic;
using Neo.Emulator.API;
using Neo.Emulator.Utils;
using Neo.Emulator.Dissambler;
using Neo.Emulator;

namespace Neo.Debugger.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        public enum DebugMode
        {
            Assembly,
            Source
        }

        private Scintilla TextArea;

        private NeoEmulator debugger;
        private AVMDisassemble avm_asm;
        private int currentLine = -1;
        private NeoMapFile map = null;
        private bool shouldReset = true;
        private DebugMode debugMode;
        private SourceLanguageKind sourceLanguage = SourceLanguageKind.Other;
        private DebuggerState debugState;
        private Dictionary<DebugMode, string> debugContent = new Dictionary<DebugMode, string>();

        public static string targetAVMPath;

        public static ABI abi;

        private Settings settings;

        private void MainForm_Load(object sender, EventArgs e)
        {
            // CREATE CONTROL
            TextArea = new ScintillaNET.Scintilla();
            TextPanel.Controls.Add(TextArea);

            // BASIC CONFIG
            TextArea.Dock = System.Windows.Forms.DockStyle.Fill;
            TextArea.TextChanged += (this.OnTextChanged);

            // INITIAL VIEW CONFIG
            TextArea.WrapMode = WrapMode.None;
            TextArea.IndentationGuides = IndentView.LookBoth;

            // STYLING
            InitColors();
            InitSyntaxColoring();

            // NUMBER MARGIN
            InitNumberMargin();

            // BOOKMARK MARGIN
            InitBookmarkMargin();

            // CODE FOLDING MARGIN
            InitCodeFolding();

            // DRAG DROP
            InitDragDropFile();

            settings = new Settings(Application.UserAppDataPath);

            // DEFAULT FILE
            if (string.IsNullOrEmpty(targetAVMPath))
            {
                targetAVMPath = settings.lastOpenedFile;
            }

            LoadDataFromFile(targetAVMPath);

            // INIT HOTKEYS
            InitHotkeys();

            SetupEmulator();
        }

        private void SetupEmulator()
        {
            Emulator.API.Runtime.OnLogMessage = this.SendLogToPanel;
        }

        private void InitColors()
        {
            TextArea.SetSelectionBackColor(true, IntToColor(0x114D9C));
        }

        private void InitHotkeys()
        {

            // register the hotkeys with the form
            HotKeyManager.AddHotKey(this, OpenSearch, Keys.F, true);
            HotKeyManager.AddHotKey(this, OpenFindDialog, Keys.F, true, false, true);
            HotKeyManager.AddHotKey(this, OpenReplaceDialog, Keys.R, true);
            HotKeyManager.AddHotKey(this, OpenReplaceDialog, Keys.H, true);
            HotKeyManager.AddHotKey(this, Uppercase, Keys.U, true);
            HotKeyManager.AddHotKey(this, Lowercase, Keys.L, true);
            HotKeyManager.AddHotKey(this, ZoomIn, Keys.Oemplus, true);
            HotKeyManager.AddHotKey(this, ZoomOut, Keys.OemMinus, true);
            HotKeyManager.AddHotKey(this, ZoomDefault, Keys.D0, true);
            HotKeyManager.AddHotKey(this, CloseSearch, Keys.Escape);

            // remove conflicting hotkeys from scintilla
            TextArea.ClearCmdKey(Keys.Control | Keys.F);
            TextArea.ClearCmdKey(Keys.Control | Keys.R);
            TextArea.ClearCmdKey(Keys.Control | Keys.H);
            TextArea.ClearCmdKey(Keys.Control | Keys.L);
            TextArea.ClearCmdKey(Keys.Control | Keys.U);

            TextArea.ClearCmdKey(Keys.F5);
            TextArea.ClearCmdKey(Keys.F6);
            TextArea.ClearCmdKey(Keys.F10);
            TextArea.ClearCmdKey(Keys.F12);

            HotKeyManager.AddHotKey(this, RunDebugger, Keys.F5);
            HotKeyManager.AddHotKey(this, OpenStorage, Keys.F6);
            HotKeyManager.AddHotKey(this, StepDebugger, Keys.F10);
            HotKeyManager.AddHotKey(this, ToggleDebuggerSource, Keys.F12);
        }

        private void InitSyntaxColoring()
        {

            // Configure the default style
            TextArea.StyleResetDefault();
            TextArea.Styles[Style.Default].Font = "Consolas";
            TextArea.Styles[Style.Default].Size = 10;
            TextArea.Styles[Style.Default].BackColor = IntToColor(0x212121);
            TextArea.Styles[Style.Default].ForeColor = IntToColor(0xFFFFFF);
            TextArea.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            TextArea.Styles[Style.Cpp.Identifier].ForeColor = IntToColor(0xD0DAE2);
            TextArea.Styles[Style.Cpp.Comment].ForeColor = IntToColor(0xBD758B);
            TextArea.Styles[Style.Cpp.CommentLine].ForeColor = IntToColor(0x40BF57);
            TextArea.Styles[Style.Cpp.CommentDoc].ForeColor = IntToColor(0x2FAE35);
            TextArea.Styles[Style.Cpp.Number].ForeColor = IntToColor(0xD69D85);
            TextArea.Styles[Style.Cpp.String].ForeColor = IntToColor(0xD69D85);
            TextArea.Styles[Style.Cpp.Character].ForeColor = IntToColor(0xE95454);
            TextArea.Styles[Style.Cpp.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            TextArea.Styles[Style.Cpp.Operator].ForeColor = IntToColor(0xE0E0E0);
            TextArea.Styles[Style.Cpp.Regex].ForeColor = IntToColor(0xD69D85);
            TextArea.Styles[Style.Cpp.CommentLineDoc].ForeColor = IntToColor(0x77A7DB);
            TextArea.Styles[Style.Cpp.Word].ForeColor = IntToColor(0x48A8EE);
            TextArea.Styles[Style.Cpp.Word2].ForeColor = IntToColor(0xF98906);
            TextArea.Styles[Style.Cpp.CommentDocKeyword].ForeColor = IntToColor(0xB3D991);
            TextArea.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = IntToColor(0xFF0000);
            TextArea.Styles[Style.Cpp.GlobalClass].ForeColor = IntToColor(0x48A8EE);

            TextArea.Lexer = Lexer.Cpp;
        }

        private void OnTextChanged(object sender, EventArgs e)
        {

        }


        #region Numbers, Bookmarks, Code Folding

        /// <summary>
        /// the background color of the text area
        /// </summary>
        private const int BACK_COLOR = 0x2A211C;

        /// <summary>
        /// default text color of the text area
        /// </summary>
        private const int FORE_COLOR = 0xB7B7B7;

        /// <summary>
        /// change this to whatever margin you want the line numbers to show in
        /// </summary>
        private const int NUMBER_MARGIN = 1;

        /// <summary>
        /// change this to whatever margin you want the bookmarks/breakpoints to show in
        /// </summary>
        private const int BOOKMARK_MARGIN = 2;
        private const int BREAKPOINT_MARKER = 2;
        private const int BREAKPOINT_BG = 3;
        private const int STEP_BG = 4;

        /// <summary>
        /// change this to whatever margin you want the code folding tree (+/-) to show in
        /// </summary>
        private const int FOLDING_MARGIN = 3;

        /// <summary>
        /// set this true to show circular buttons for code folding (the [+] and [-] buttons on the margin)
        /// </summary>
        private const bool CODEFOLDING_CIRCULAR = true;

        private void InitNumberMargin()
        {

            TextArea.Styles[Style.LineNumber].BackColor = IntToColor(BACK_COLOR);
            TextArea.Styles[Style.LineNumber].ForeColor = IntToColor(FORE_COLOR);
            TextArea.Styles[Style.IndentGuide].ForeColor = IntToColor(FORE_COLOR);
            TextArea.Styles[Style.IndentGuide].BackColor = IntToColor(BACK_COLOR);

            var nums = TextArea.Margins[NUMBER_MARGIN];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            TextArea.MarginClick += TextArea_MarginClick;
        }

        private void InitBookmarkMargin()
        {

            //TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));

            var margin = TextArea.Margins[BOOKMARK_MARGIN];
            margin.Width = 20;
            margin.Sensitive = true;
            margin.Type = MarginType.Symbol;
            margin.Mask = (1 << BREAKPOINT_MARKER);
            //margin.Cursor = MarginCursor.Arrow;

            var marker = TextArea.Markers[BREAKPOINT_MARKER];
            marker.Symbol = MarkerSymbol.Circle;
            marker.SetBackColor(IntToColor(0xFF003B));
            marker.SetForeColor(IntToColor(0x000000));
            marker.SetAlpha(100);

            marker = TextArea.Markers[BREAKPOINT_BG];
            marker.Symbol = MarkerSymbol.Background;
            marker.SetBackColor(Color.Red);

            marker = TextArea.Markers[STEP_BG];
            marker.Symbol = MarkerSymbol.Background;
            marker.SetBackColor(IntToColor(0xCC000));
        }

        private void InitCodeFolding()
        {

            TextArea.SetFoldMarginColor(true, IntToColor(BACK_COLOR));
            TextArea.SetFoldMarginHighlightColor(true, IntToColor(BACK_COLOR));

            // Enable code folding
            TextArea.SetProperty("fold", "1");
            TextArea.SetProperty("fold.compact", "1");

            // Configure a margin to display folding symbols
            TextArea.Margins[FOLDING_MARGIN].Type = MarginType.Symbol;
            TextArea.Margins[FOLDING_MARGIN].Mask = Marker.MaskFolders;
            TextArea.Margins[FOLDING_MARGIN].Sensitive = true;
            TextArea.Margins[FOLDING_MARGIN].Width = 20;

            // Set colors for all folding markers
            for (int i = 25; i <= 31; i++)
            {
                TextArea.Markers[i].SetForeColor(IntToColor(BACK_COLOR)); // styles for [+] and [-]
                TextArea.Markers[i].SetBackColor(IntToColor(FORE_COLOR)); // styles for [+] and [-]
            }

            // Configure folding markers with respective symbols
            TextArea.Markers[Marker.Folder].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlus : MarkerSymbol.BoxPlus;
            TextArea.Markers[Marker.FolderOpen].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinus : MarkerSymbol.BoxMinus;
            TextArea.Markers[Marker.FolderEnd].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CirclePlusConnected : MarkerSymbol.BoxPlusConnected;
            TextArea.Markers[Marker.FolderMidTail].Symbol = MarkerSymbol.TCorner;
            TextArea.Markers[Marker.FolderOpenMid].Symbol = CODEFOLDING_CIRCULAR ? MarkerSymbol.CircleMinusConnected : MarkerSymbol.BoxMinusConnected;
            TextArea.Markers[Marker.FolderSub].Symbol = MarkerSymbol.VLine;
            TextArea.Markers[Marker.FolderTail].Symbol = MarkerSymbol.LCorner;

            // Enable automatic folding
            TextArea.AutomaticFold = (AutomaticFold.Show | AutomaticFold.Click | AutomaticFold.Change);

        }

        private void TextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == BOOKMARK_MARGIN)
            {
                // Do we have a marker for this line?
                const uint mask = (1 << BREAKPOINT_MARKER);

                var line = TextArea.Lines[TextArea.LineFromPosition(e.Position)];
                var ofs = ResolveOffset(line.Index);

                if ((line.MarkerGet() & mask) > 0)
                {
                    // Remove existing bookmark
                    line.MarkerDelete(BREAKPOINT_MARKER);
                    line.MarkerDelete(BREAKPOINT_BG);

                    if (ofs >= 0) // should always be true
                    {
                        debugger.SetBreakpointState(ofs, false);
                    }
                }
                else
                {

                    // check if was possible to resolve this line to a valid offset in the script
                    if (ofs >= 0)
                    {
                        // Add bookmark
                        line.MarkerAdd(BREAKPOINT_MARKER);
                        line.MarkerAdd(BREAKPOINT_BG);

                        // enable a breakpoint in the VM
                        debugger.SetBreakpointState(ofs, true);
                    }
                }
            }
        }

        #endregion

        #region Drag & Drop File

        public void InitDragDropFile()
        {

            TextArea.AllowDrop = true;
            TextArea.DragEnter += delegate (object sender, DragEventArgs e)
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    e.Effect = DragDropEffects.Copy;
                else
                    e.Effect = DragDropEffects.None;
            };
            TextArea.DragDrop += delegate (object sender, DragEventArgs e)
            {

                // get file drop
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {

                    Array a = (Array)e.Data.GetData(DataFormats.FileDrop);
                    if (a != null)
                    {
                        string path = a.GetValue(0).ToString();

                        LoadDataFromFile(path);
                    }
                }
            };

        }

        private byte[] contractBytecode;
        private string contractName;

        public void LoadDataFromFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            if (!File.Exists(path))
            {
                MessageBox.Show("Can't find '" + (String.IsNullOrEmpty(path) ? "(null)" : path + "'"), this.Text + " - " + Environment.CurrentDirectory);
            }
            else
            {
                MainForm.targetAVMPath = path;

                this.contractName = Path.GetFileNameWithoutExtension(path);

                this.contractBytecode = File.ReadAllBytes(path);

                var oldMapFileName = path.Replace(".avm", ".neomap");
                var newMapFileName = path.Replace(".avm", ".debug.json");

                debugMode = DebugMode.Assembly;
                sourceLanguage = SourceLanguageKind.Other;

                if (File.Exists(newMapFileName))
                {
                    map = new NeoMapFile();
                    map.LoadFromFile(newMapFileName, contractBytecode);

                    this.contractName = map.contractName;
                }
                else
                {
                    if (File.Exists(oldMapFileName))
                    {
                        MessageBox.Show("Warning: The file format of debug map changed. Please recompile your AVM with the latest compiler.");
                    }
                    map = null;
                }

                string abiFilename = path.Replace(".avm", ".abi.json");
                if (File.Exists(abiFilename))
                {
                    abi = new ABI(abiFilename);
                }
                else
                {
                    MessageBox.Show($"Error: {abiFilename} was not found. Please recompile your AVM with the latest compiler.");
                    return;
                }

                this.debugger = null;
                this.avm_asm = NeoDisassembler.Disassemble(contractBytecode);

                if (map != null && map.Entries.Any())
                {
                    var srcFile = map.Entries.FirstOrDefault().url;

                    if (string.IsNullOrEmpty(srcFile))
                    {
                        MessageBox.Show("Error: Could not load the debug map correct, no file entries.");
                        return;
                    }

                    if (!File.Exists(srcFile))
                    {
                        MessageBox.Show("Error: Could not load the source code, check that this file exists:"+srcFile);
                        return;
                    }

                    FileName.Text = srcFile;

                    sourceLanguage = LanguageSupport.DetectLanguage(srcFile);

                    debugMode = DebugMode.Source;
                    debugContent[DebugMode.Source] = File.ReadAllText(srcFile);
                }

                debugContent[DebugMode.Assembly] = avm_asm.ToString();
                FileName.Text = Path.GetFileName(path);

                ReloadTextArea();

                BlockchainLoad();

                UpdateSourceViewMenus();

                shouldReset = true;

                settings.lastOpenedFile = path;
                settings.Save();
            }
        }

        #endregion

        #region Main Menu Commands

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {                
                LoadDataFromFile(openFileDialog.FileName);
            }
        }

        private void findToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenSearch();
        }

        private void findDialogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFindDialog();
        }

        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenReplaceDialog();
        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextArea.Cut();
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextArea.Copy();
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextArea.Paste();
        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextArea.SelectAll();
        }

        private void selectLineToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Line line = TextArea.Lines[TextArea.CurrentLine];
            TextArea.SetSelection(line.Position + line.Length, line.Position);
        }

        private void clearSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextArea.SetEmptySelection(0);
        }

        private void indentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Indent();
        }

        private void outdentSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Outdent();
        }

        private void uppercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Uppercase();
        }

        private void lowercaseSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Lowercase();
        }

        private void wordWrapToolStripMenuItem1_Click(object sender, EventArgs e)
        {

            // toggle word wrap
            wordWrapItem.Checked = !wordWrapItem.Checked;
            TextArea.WrapMode = wordWrapItem.Checked ? WrapMode.Word : WrapMode.None;
        }

        private void indentGuidesToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // toggle indent guides
            indentGuidesItem.Checked = !indentGuidesItem.Checked;
            TextArea.IndentationGuides = indentGuidesItem.Checked ? IndentView.LookBoth : IndentView.None;
        }

        private void hiddenCharactersToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // toggle view whitespace
            hiddenCharactersItem.Checked = !hiddenCharactersItem.Checked;
            TextArea.ViewWhitespace = hiddenCharactersItem.Checked ? WhitespaceMode.VisibleAlways : WhitespaceMode.Invisible;
        }

        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomIn();
        }

        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomOut();
        }

        private void zoom100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ZoomDefault();
        }

        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextArea.FoldAll(FoldAction.Contract);
        }

        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TextArea.FoldAll(FoldAction.Expand);
        }


        #endregion

        #region Uppercase / Lowercase

        private void Lowercase()
        {

            // save the selection
            int start = TextArea.SelectionStart;
            int end = TextArea.SelectionEnd;

            // modify the selected text
            TextArea.ReplaceSelection(TextArea.GetTextRange(start, end - start).ToLower());

            // preserve the original selection
            TextArea.SetSelection(start, end);
        }

        private void Uppercase()
        {

            // save the selection
            int start = TextArea.SelectionStart;
            int end = TextArea.SelectionEnd;

            // modify the selected text
            TextArea.ReplaceSelection(TextArea.GetTextRange(start, end - start).ToUpper());

            // preserve the original selection
            TextArea.SetSelection(start, end);
        }

        #endregion

        #region Indent / Outdent

        private void Indent()
        {
            // we use this hack to send "Shift+Tab" to scintilla, since there is no known API to indent,
            // although the indentation function exists. Pressing TAB with the editor focused confirms this.
            GenerateKeystrokes("{TAB}");
        }

        private void Outdent()
        {
            // we use this hack to send "Shift+Tab" to scintilla, since there is no known API to outdent,
            // although the indentation function exists. Pressing Shift+Tab with the editor focused confirms this.
            GenerateKeystrokes("+{TAB}");
        }

        private void GenerateKeystrokes(string keys)
        {
            HotKeyManager.Enable = false;
            TextArea.Focus();
            SendKeys.Send(keys);
            HotKeyManager.Enable = true;
        }

        #endregion

        #region Zoom

        private void ZoomIn()
        {
            TextArea.ZoomIn();
        }

        private void ZoomOut()
        {
            TextArea.ZoomOut();
        }

        private void ZoomDefault()
        {
            TextArea.Zoom = 0;
        }


        #endregion

        #region Quick Search Bar

        bool SearchIsOpen = false;

        public object IEnumerabl { get; private set; }

        private void OpenSearch()
        {

            SearchManager.SearchBox = TxtSearch;
            SearchManager.TextArea = TextArea;

            if (!SearchIsOpen)
            {
                SearchIsOpen = true;
                InvokeIfNeeded(delegate ()
                {
                    PanelSearch.Visible = true;
                    TxtSearch.Text = SearchManager.LastSearch;
                    TxtSearch.Focus();
                    TxtSearch.SelectAll();
                });
            }
            else
            {
                InvokeIfNeeded(delegate ()
                {
                    TxtSearch.Focus();
                    TxtSearch.SelectAll();
                });
            }
        }
        private void CloseSearch()
        {
            if (SearchIsOpen)
            {
                SearchIsOpen = false;
                InvokeIfNeeded(delegate ()
                {
                    PanelSearch.Visible = false;
                    //CurBrowser.GetBrowser().StopFinding(true);
                });
            }
        }

        private void BtnClearSearch_Click(object sender, EventArgs e)
        {
            CloseSearch();
        }

        private void BtnPrevSearch_Click(object sender, EventArgs e)
        {
            SearchManager.Find(false, false);
        }
        private void BtnNextSearch_Click(object sender, EventArgs e)
        {
            SearchManager.Find(true, false);
        }
        private void TxtSearch_TextChanged(object sender, EventArgs e)
        {
            SearchManager.Find(true, true);
        }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (HotKeyManager.IsHotkey(e, Keys.Enter))
            {
                SearchManager.Find(true, false);
            }
            if (HotKeyManager.IsHotkey(e, Keys.Enter, true) || HotKeyManager.IsHotkey(e, Keys.Enter, false, true))
            {
                SearchManager.Find(false, false);
            }
        }

        #endregion

        #region Find & Replace Dialog

        private void OpenFindDialog()
        {

        }
        private void OpenReplaceDialog()
        {


        }

        #endregion

        #region Utils

        public static Color IntToColor(int rgb)
        {
            return Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        public void InvokeIfNeeded(Action action)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(action);
            }
            else
            {
                action.Invoke();
            }
        }

        #endregion

        #region DEBUGGER
        private RunForm runForm = new RunForm();

        private bool ResetDebugger()
        {
            if (string.IsNullOrEmpty(targetAVMPath))
            {
                MessageBox.Show("Please load an .avm file first!");
                return false;
            }

            if (this.debugger == null)
            {
                this.debugger = new NeoEmulator(blockchain);

                var address = blockchain.FindAddressByName(this.contractName);

                if (address == null)
                {
                    address = blockchain.DeployContract(this.contractName, this.contractBytecode);
                    SendLogToPanel($"Deployed contract {contractName} on virtual blockchain.");
                }
                else
                {
                    if (!address.byteCode.SequenceEqual(this.contractBytecode))
                    {
                        address.byteCode = this.contractBytecode;
                        SendLogToPanel($"Updated contract {contractName} bytecode.");
                    }
                }

                this.debugger.SetExecutingAddress(address);
            }

            runForm.emulator = this.debugger;
            runForm.abi = abi;

            var result = runForm.ShowDialog();

            if (result != DialogResult.OK)
            {
                return false;
            }

            RemoveCurrentHighlight();

            /*currentLine = ResolveLine(0);
            if (currentLine > 0 )
            {
                TextArea.Lines[currentLine].MarkerAdd(STEP_BG);
            }*/

            currentLine = -1;

            shouldReset = false;

            logView.Clear();
            stackPanel.Clear();

            UpdateGasCost();

            return true;
        }

        private int ResolveLine(int ofs)
        {
            try
            {
                switch (debugMode)
                {
                    case DebugMode.Source:
                        {
                            var line = map.ResolveLine(ofs);
                            return line - 1;
                        }

                    case DebugMode.Assembly:
                        {
                            var line = avm_asm.ResolveLine(ofs);
                            return line + 2;
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

        private int ResolveOffset(int line)
        {
            try
            {
                switch (debugMode)
                {
                    case DebugMode.Source:
                        {
                            var ofs = map.ResolveOffset(line + 1);
                            return ofs;
                        }

                    case DebugMode.Assembly:
                        {
                            var ofs = avm_asm.ResolveOffset(line);
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

        private bool RemoveCurrentHighlight()
        {
            if (currentLine > 0)
            {
                TextArea.Lines[currentLine].MarkerDelete(STEP_BG);
                return true;
            }

            return false;
        }

        private void JumpToLine(int line)
        {
            if (line < 0)
            {
                return;
            }

            var target = TextArea.Lines[line];
            target.Goto();
            target.EnsureVisible();

            TextArea.FirstVisibleLine = line;
        }


        private void UpdateState()
        {
            currentLine = ResolveLine(debugState.offset);

            switch (debugState.state)
            {
                case DebuggerState.State.Finished:
                    {
                        shouldReset = true;
                        RemoveCurrentHighlight();
                        var val = debugger.GetOutput();

                        BlockchainSave();

                        var gasStr = string.Format("{0:N4}", debugger.GetUsedGas()); 

                        MessageBox.Show("Execution finished.\nGAS cost: " + gasStr +"\nResult: " + FormattingUtils.StackItemAsString(val));

                        break;
                    }

                case DebuggerState.State.Exception:
                    {
                        shouldReset = true;
                        MessageBox.Show("Execution failed with an exception at address " + debugger.GetInstructionPtr().ToString() + " lastOffset: " + debugState.offset.ToString());
                        JumpToLine(currentLine);
                        break;
                    }

                case DebuggerState.State.Break:
                    {
                        MessageBox.Show("Execution hit an breakpoint");
                        JumpToLine(currentLine);
                        break;
                    }
            }
        }

        private void ReloadTextArea()
        {
            var keywords = LanguageSupport.GetLanguageKeywords(sourceLanguage);

            if (keywords.Length == 2)
            {
                TextArea.SetKeywords(0, keywords[0]);
                TextArea.SetKeywords(1, keywords[1]);
            }

            TextArea.ReadOnly = false;
            TextArea.Text = debugContent[debugMode];
            TextArea.ReadOnly = true;
        }

        private void ToggleDebuggerSource()
        {
            if (map == null)
            {
                MessageBox.Show("Map file not available for this .avm");
                return;
            }

            debugMode = debugMode == DebugMode.Assembly ? DebugMode.Source : DebugMode.Assembly;

            ReloadTextArea();

            foreach (var ofs in debugger.Breakpoints)
            {
                var line = ResolveLine(ofs);

                if (line >= 0)
                {
                    TextArea.Lines[line].MarkerAdd(BREAKPOINT_BG);
                    TextArea.Lines[line].MarkerAdd(BREAKPOINT_MARKER);
                }
            }

            if (currentLine > 0 && (debugState.state != DebuggerState.State.Running || debugState.state == DebuggerState.State.Break))
            {
                currentLine = ResolveLine(debugState.offset);
                TextArea.Lines[currentLine].MarkerAdd(STEP_BG);
            }

            UpdateSourceViewMenus();
        }

        private void UpdateSourceViewMenus()
        {
            assemblyToolStripMenuItem.Enabled = debugMode != DebugMode.Assembly;
            originalToolStripMenuItem.Enabled = debugMode != DebugMode.Source;
        }

        private void RunDebugger()
        {
            if (this.debugger == null)
            {
                shouldReset = true;
            }

            if (shouldReset)
            {
                if (!this.ResetDebugger())
                {
                    return;
                }
            }

            debugState = debugger.Run();
            UpdateState();
        }

        private void StepDebugger()
        {
            if (shouldReset)
            {
                if (!this.ResetDebugger())
                {
                    return;
                }
            }

            int oldLine = currentLine;

            RemoveCurrentHighlight();
            do
            {
                debugState = debugger.Step();

                UpdateState();

                if (shouldReset)
                {
                    return;
                }


            } while (currentLine <= 0 || oldLine == currentLine);

            UpdateStackPanel();
            UpdateGasCost();

            var targetLine = TextArea.Lines[currentLine];
            targetLine.EnsureVisible();

            targetLine.MarkerAdd(STEP_BG);

            var firstVisible = TextArea.FirstVisibleLine;

            var lastVisible = firstVisible + TextArea.LinesOnScreen;

            if (lastVisible > TextArea.Lines.Count)
            {
                lastVisible = TextArea.Lines.Count;
            }

            if (oldLine < 0)
            {
                targetLine.Goto();
            }
            else
            if (currentLine == oldLine + 1)
            {
                if (currentLine >= lastVisible)
                {
                    TextArea.LineScroll(1, 0);
                }
            }
            else
            {
                targetLine.Goto();
            }
        }

        private void OpenStorage()
        {
            if (this.debugger == null || this.debugger.currentAddress == null)
            {
                MessageBox.Show("Please deploy the smart contract first!");
                return;
            }

            var form = new StorageForm();
            form.debugger = this.debugger;
            form.ShowDialog();
        }
        #endregion

        #region DEBUG PANELS
        public void SendLogToPanel(string s)
        {
            logView.Text += s + "\n";
        }

        public void ClearLog()
        {
            logView.Text = "";
        }

        private void UpdateStackPanel()
        {
            var sb = new StringBuilder();

            var items = debugger.GetStack();

            foreach (var item in items)
            {
                string s = FormattingUtils.StackItemAsString(item);
                sb.AppendLine(s);
            }

            stackPanel.Text = sb.ToString();
        }

        private void UpdateGasCost()
        {
            gasCostLabel.Visible = true;
            gasCostLabel.Text = "GAS used: "+ debugger.GetUsedGas();
        }

        #endregion

        private void MainForm_Resize(object sender, EventArgs e)
        {
            var padding = 18;

            var logWidthPercent = 0.6f;

            logView.Width = (int)(this.ClientSize.Width * logWidthPercent) - (padding * 2);
            logView.Top = this.ClientSize.Height - (padding + logView.Height);
            logLabel.Top = logView.Top - 18;

            stackPanel.Width = this.ClientSize.Width  - (logView.Width + padding * 2);
            stackPanel.Left = padding + logView.Width;
            stackPanel.Top = logView.Top;
            stackLabel.Left = stackPanel.Left;
            stackLabel.Top = logLabel.Top;

            gasCostLabel.Left = this.ClientSize.Width - 105;
        }

        #region DEBUG MENU
        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RunDebugger();
        }
        
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            OpenStorage();
        }

        private void stepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StepDebugger();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ResetDebugger();
        }

        private void originalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (debugMode != DebugMode.Source)
            {
                ToggleDebuggerSource();
            }
        }

        private void assemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (debugMode != DebugMode.Assembly)
            {
                ToggleDebuggerSource();
            }
        }
        #endregion

        #region BLOCKCHAIN API
        private Blockchain blockchain;

        public string BlockchainPath()
        {
            if (string.IsNullOrEmpty(targetAVMPath))
            {
                return null;
            }

            //return Directory.GetCurrentDirectory() + "/virtual.chain";
            return targetAVMPath.Replace(".avm", ".chain");
        }

       public void BlockchainLoad()
        {
            var path = BlockchainPath();
            blockchain = new Blockchain();
            blockchain.Load(path);
        }

        public void BlockchainSave()
        {
            var path = BlockchainPath();
            blockchain.Save(path);
        }
        #endregion

        private void blockchainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.debugger == null || this.debugger.blockchain == null)
            {
                MessageBox.Show("Please deploy the smart contract first!");
                return;
            }

            var form = new BlockchainForm();
            form.debugger = this.debugger;
            form.ShowDialog();
        }

        private void cCompilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new CSharpCompilerForm();
            form.mainForm = this;
            form.ShowDialog();
        }

        private void keyDecoderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new KeyToolForm();
            form.ShowDialog();
        }
    }
}
