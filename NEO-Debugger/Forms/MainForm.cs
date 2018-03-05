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
using Neo.Debugger.Data;
using Neo.Debugger.Models;

namespace Neo.Debugger.Forms
{
    public partial class MainForm : Form
    {
        //Command line param
        private string _sourceAvmPath;
        private string _argumentsAvmFile;
        private Settings _settings;
        private DebugManager _debugger;

        private Scintilla TextArea;

        public MainForm(string argumentsAvmFile)
        {
            InitializeComponent();
            _argumentsAvmFile = argumentsAvmFile;
            _sourceAvmPath = argumentsAvmFile;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            _settings = new Settings();

            if (string.IsNullOrEmpty(_sourceAvmPath))
            {
                _sourceAvmPath = _settings.lastOpenedFile;
            }

            //Init the UI controls
            InitUI();

            //Setup emulator log
            Emulator.API.Runtime.OnLogMessage = SendLogToPanel;

            //Init the debugger
            InitDebugger();
        }

        #region Initializers
        private void InitUI()
        {
            // CREATE CONTROL
            TextArea = new ScintillaNET.Scintilla();
            TextPanel.Controls.Add(TextArea);

            // BASIC CONFIG
            TextArea.Dock = System.Windows.Forms.DockStyle.Fill;
            TextArea.TextChanged += (this.TextArea_OnTextChanged);

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

            // INIT HOTKEYS
            InitHotkeys();
        }

        private void InitDebugger()
        {
            _debugger = new DebugManager(_settings);
            _debugger.SendToLog += _debugger_SendToLog;

            //Load if we had a file on the command line or a previously opened
            bool success = false;
            try
            {
                success = LoadDebugFile(_sourceAvmPath);
            }
            catch (Exception)
            {
                success = false;
            }

            if (!success)
            {
                if (!String.IsNullOrEmpty(_argumentsAvmFile)) // display when launched with command line arg (e.g. from Visual Studio/Start)
                {
                    string cwd = Environment.CurrentDirectory;
                    MessageBox.Show("Can't open '" + _sourceAvmPath + "'. Current directory is '" + cwd + "'", "Open AVM File");
                }
            }
        }

        private bool LoadDebugFile(string path)
        {
            if (!_debugger.LoadAvmFile(path))
                return false;

            if (!_debugger.LoadEmulator())
                return false;

            if (!_debugger.LoadContract())
                return false;

            _debugger.LoadTests();

            //Set the UI
            FileName.Text = _debugger.AvmFileName;
            this.Text += " - " + FileName.Text;
            UpdateSourceViewMenus();
            ReloadTextArea();
            return true;
        }

        #endregion

        #region Debugger Actions

        private void RunDebugger()
        {
            //We need to make sure there is a file loaded
            if (!_debugger.AvmFileLoaded)
            {
                MessageBox.Show("Please load an .avm file first!");
                return;
            }

            if (_debugger.ResetFlag && !ResetDebugger())
                return;

            _debugger.Run();
            UpdateDebuggerStateUI();
            _debugger.Emulator.ProfilerDumpCSV();
        }

        private void StepDebugger()
        {
            //We need to make sure there is a file loaded
            if (!_debugger.AvmFileLoaded)
            {
                MessageBox.Show("Please load an .avm file first!");
                return;
            }

            if (_debugger.ResetFlag && !ResetDebugger())
                return;

            Line previousLine = TextArea.Lines[TextArea.CurrentLine];
            do
            {
                _debugger.Step();

                UpdateDebuggerStateUI();

                if (_debugger.ResetFlag)
                    return;

            } while (_debugger.CurrentLine <= 0 || previousLine.Index == _debugger.CurrentLine);

            //Update UI
            UpdateStackPanel();
            UpdateGasCost(_debugger.Emulator.GetUsedGas());
            UpdateDebuggerStateUI();
            _debugger.Emulator.ProfilerDumpCSV();
        }

        private bool ResetDebugger()
        {
            //We need to make sure there is a file loaded
            if (!_debugger.AvmFileLoaded)
            {
                MessageBox.Show("Please load an .avm file first!");
                return false;
            }

            //Get the parameters to execute the debugger
            if (!GetDebugParameters())
                return false;

            //Reset the UI
            RemoveCurrentHighlight();
            logView.Clear();
            stackPanel.Clear();
            UpdateGasCost(_debugger.UsedGasCost);
            return true;
        }

        private bool GetDebugParameters()
        {
            //Run form with defaults from settings if available
            RunForm runForm = new RunForm(_debugger.ABI, _debugger.Tests, _debugger.ContractName, _settings.lastPrivateKey, _settings.lastParams, _settings.lastFunction);
            var result = runForm.ShowDialog();
            var debugParams = runForm.DebugParameters;
            if (result != DialogResult.OK)
                return false;

            if (runForm.currentMethod != null)
            {
                _settings.lastFunction = runForm.currentMethod.name;
            }

            _debugger.SetDebugParameters(debugParams);
            return true;
        }

        #endregion

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
        /// The mask to detect a breakpoint marker
        /// </summary>
        const uint BREAKPOINT_MASK = (1 << BREAKPOINT_MARKER);

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

            TextArea.Styles[Style.LineNumber].BackColor = ColorUtil.IntToColor(BACK_COLOR);
            TextArea.Styles[Style.LineNumber].ForeColor = ColorUtil.IntToColor(FORE_COLOR);
            TextArea.Styles[Style.IndentGuide].ForeColor = ColorUtil.IntToColor(FORE_COLOR);
            TextArea.Styles[Style.IndentGuide].BackColor = ColorUtil.IntToColor(BACK_COLOR);

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
            marker.SetBackColor(ColorUtil.IntToColor(0xFF003B));
            marker.SetForeColor(ColorUtil.IntToColor(0x000000));
            marker.SetAlpha(100);

            marker = TextArea.Markers[BREAKPOINT_BG];
            marker.Symbol = MarkerSymbol.Background;
            marker.SetBackColor(ColorUtil.IntToColor(0x600000));

            marker = TextArea.Markers[STEP_BG];
            marker.Symbol = MarkerSymbol.Background;
            marker.SetBackColor(ColorUtil.IntToColor(0x5a5a23));
        }

        private void InitCodeFolding()
        {

            TextArea.SetFoldMarginColor(true, ColorUtil.IntToColor(BACK_COLOR));
            TextArea.SetFoldMarginHighlightColor(true, ColorUtil.IntToColor(BACK_COLOR));

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
                TextArea.Markers[i].SetForeColor(ColorUtil.IntToColor(BACK_COLOR)); // styles for [+] and [-]
                TextArea.Markers[i].SetBackColor(ColorUtil.IntToColor(FORE_COLOR)); // styles for [+] and [-]
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

        private void InitColors()
        {
            TextArea.SetSelectionBackColor(true, ColorUtil.IntToColor(0x114D9C));
        }

        private void InitSyntaxColoring()
        {

            // Configure the default style
            TextArea.StyleResetDefault();
            TextArea.Styles[Style.Default].Font = "Consolas";
            TextArea.Styles[Style.Default].Size = 10;
            TextArea.Styles[Style.Default].BackColor = ColorUtil.IntToColor(0x212121);
            TextArea.Styles[Style.Default].ForeColor = ColorUtil.IntToColor(0xFFFFFF);
            TextArea.StyleClearAll();

            // Configure the CPP (C#) lexer styles
            TextArea.Styles[Style.Cpp.Identifier].ForeColor = ColorUtil.IntToColor(0xD0DAE2);
            TextArea.Styles[Style.Cpp.Comment].ForeColor = ColorUtil.IntToColor(0xBD758B);
            TextArea.Styles[Style.Cpp.CommentLine].ForeColor = ColorUtil.IntToColor(0x40BF57);
            TextArea.Styles[Style.Cpp.CommentDoc].ForeColor = ColorUtil.IntToColor(0x2FAE35);
            TextArea.Styles[Style.Cpp.Number].ForeColor = ColorUtil.IntToColor(0xD69D85);
            TextArea.Styles[Style.Cpp.String].ForeColor = ColorUtil.IntToColor(0xD69D85);
            TextArea.Styles[Style.Cpp.Character].ForeColor = ColorUtil.IntToColor(0xE95454);
            TextArea.Styles[Style.Cpp.Preprocessor].ForeColor = ColorUtil.IntToColor(0x8AAFEE);
            TextArea.Styles[Style.Cpp.Operator].ForeColor = ColorUtil.IntToColor(0xE0E0E0);
            TextArea.Styles[Style.Cpp.Regex].ForeColor = ColorUtil.IntToColor(0xD69D85);
            TextArea.Styles[Style.Cpp.CommentLineDoc].ForeColor = ColorUtil.IntToColor(0x77A7DB);
            TextArea.Styles[Style.Cpp.Word].ForeColor = ColorUtil.IntToColor(0x48A8EE);
            TextArea.Styles[Style.Cpp.Word2].ForeColor = ColorUtil.IntToColor(0xF98906);
            TextArea.Styles[Style.Cpp.CommentDocKeyword].ForeColor = ColorUtil.IntToColor(0xB3D991);
            TextArea.Styles[Style.Cpp.CommentDocKeywordError].ForeColor = ColorUtil.IntToColor(0xFF0000);
            TextArea.Styles[Style.Cpp.GlobalClass].ForeColor = ColorUtil.IntToColor(0x48A8EE);

            TextArea.Lexer = Lexer.Cpp;
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

                        LoadDebugFile(path);
                    }
                }
            };

        }

        #endregion

        #region Main Form Events and Commands

        private void TextArea_MarginClick(object sender, MarginClickEventArgs e)
        {
            if (e.Margin == BOOKMARK_MARGIN)
            {
                var line = TextArea.Lines[TextArea.LineFromPosition(e.Position)];

                // Do we have a marker for this line?
                if ((line.MarkerGet() & BREAKPOINT_MASK) > 0)
                {
                    if (!_debugger.RemoveBreakpoint(line.Index))
                    {
                        SendLogToPanel("Error removing breakpoint.");
                        return;
                    }

                    // Remove existing from UI
                    line.MarkerDelete(BREAKPOINT_MARKER);
                }
                else
                {
                    if (!_debugger.AddBreakpoint(line.Index))
                    {
                        SendLogToPanel("Error adding breakpoint.");
                        return;
                    }

                    // Add breakpoint to UI
                    line.MarkerAdd(BREAKPOINT_MARKER);                
                }
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            var padding = 18;

            var logWidthPercent = 0.6f;

            logView.Width = (int)(this.ClientSize.Width * logWidthPercent) - (padding * 2);
            logView.Top = this.ClientSize.Height - (padding + logView.Height);
            logLabel.Top = logView.Top - 18;

            stackPanel.Width = this.ClientSize.Width - (logView.Width + padding * 2);
            stackPanel.Left = padding + logView.Width;
            stackPanel.Top = logView.Top;
            stackLabel.Left = stackPanel.Left;
            stackLabel.Top = logLabel.Top;

            gasCostLabel.Left = this.ClientSize.Width - 105;
        }

        private void TextArea_OnTextChanged(object sender, EventArgs e)
        {

        }

        #endregion

        #region Main Menu Commands

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {                
                LoadDebugFile(openFileDialog.FileName);
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

        #region Debugger UI Helpers

        private void UpdateDebuggerStateUI()
        {
            RemoveCurrentHighlight();

            //Update the UI to reflect the debugger state
            switch (_debugger.State)
            {
                case DebuggerState.State.Running:
                    {
                        JumpToLine(_debugger.CurrentLine);
                        TextArea.Lines[TextArea.CurrentLine].MarkerAdd(STEP_BG);
                        break;
                    }

                case DebuggerState.State.Finished:
                    {
                        var val = _debugger.Emulator.GetOutput();
                        var gasStr = string.Format("{0:N4}", _debugger.Emulator.GetUsedGas());

                        string hintType = !string.IsNullOrEmpty(_settings.lastFunction) && _debugger.ABI != null && _debugger.ABI.functions.ContainsKey(_settings.lastFunction) ? _debugger.ABI.functions[_settings.lastFunction].returnType : null;

                        MessageBox.Show("Execution finished.\nGAS cost: " + gasStr + "\nResult: " + FormattingUtils.StackItemAsString(val, false, hintType));
                        break;
                    }

                case DebuggerState.State.Exception:
                    {
                        MessageBox.Show("Execution failed with an exception at address " + _debugger.Emulator.GetInstructionPtr().ToString() + " lastOffset: " + _debugger.Offset.ToString());
                        JumpToLine(_debugger.CurrentLine);
                        TextArea.Lines[TextArea.CurrentLine].MarkerAdd(BREAKPOINT_BG); //Highlight red
                        break;
                    }

                case DebuggerState.State.Break:
                    {
                        JumpToLine(_debugger.CurrentLine);
                        TextArea.Lines[TextArea.CurrentLine].MarkerAdd(BREAKPOINT_BG);
                        break;
                    }
            }
        }

        private void RemoveCurrentHighlight()
        {
            TextArea.Lines[TextArea.CurrentLine].MarkerDelete(BREAKPOINT_BG);
            TextArea.Lines[TextArea.CurrentLine].MarkerDelete(STEP_BG);
        }

        private void JumpToLine(int line)
        {
            if (line < 0)
            {
                return;
            }

            var targetLine = TextArea.Lines[line];
            targetLine.EnsureVisible();
            targetLine.Goto();

            var totalLines = TextArea.Lines.Count();
            var firstVisible = TextArea.FirstVisibleLine;
            var lastVisible = TextArea.FirstVisibleLine + TextArea.LinesOnScreen;
            var targetIndex = targetLine.Index;
            var paddingLines = 10;

            if (targetIndex < firstVisible)
            {
                TextArea.LineScroll(firstVisible - targetIndex + paddingLines, 0);
            }
            else if (targetIndex > (lastVisible - paddingLines) && lastVisible < (totalLines - paddingLines))
            {
                TextArea.LineScroll(lastVisible - targetIndex + paddingLines, 0);
            }
            else if (targetIndex < (firstVisible + paddingLines) && targetIndex < (totalLines - paddingLines))
            {
                TextArea.LineScroll(targetIndex - firstVisible - paddingLines, 0);
            }
        }

        private void ReloadTextArea()
        {
            var keywords = LanguageSupport.GetLanguageKeywords(_debugger.Language);

            if (keywords.Length == 2)
            {
                TextArea.SetKeywords(0, keywords[0]);
                TextArea.SetKeywords(1, keywords[1]);
            }

            TextArea.ReadOnly = false;
            TextArea.Text = _debugger.DebugContent[_debugger.Mode];
            TextArea.ReadOnly = true;
        }

        private void ToggleDebuggerSource()
        {
            if (!_debugger.MapLoaded)
            {
                MessageBox.Show("Map file not available for this .avm");
                return;
            }

            //Toggle debug mode
            _debugger.ToggleDebugMode();

            ReloadTextArea();

            var breakpointLines = _debugger.GetBreakPointLineNumbers();
            foreach(var line in breakpointLines)
            {
                TextArea.Lines[line].MarkerAdd(BREAKPOINT_MARKER);
            }

            if (_debugger.IsSteppingOrOnBreakpoint)
                TextArea.Lines[_debugger.CurrentLine].MarkerAdd(STEP_BG);

            UpdateSourceViewMenus();
        }

        private void UpdateSourceViewMenus()
        {
            assemblyToolStripMenuItem.Enabled = _debugger.Mode != DebugMode.Assembly;
            originalToolStripMenuItem.Enabled = _debugger.Mode != DebugMode.Source;
        }

        private void OpenStorage()
        {
            if (!_debugger.SmartContractDeployed)
            {
                MessageBox.Show("Please deploy the smart contract first!");
                return;
            }

            var form = new StorageForm(_debugger.Emulator);
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

            var items = _debugger.Emulator.GetStack();

            foreach (var item in items)
            {
                string s = FormattingUtils.StackItemAsString(item);
                sb.AppendLine(s);
            }

            stackPanel.Text = sb.ToString();
        }

        private void UpdateGasCost(double gasUsed)
        {
            gasCostLabel.Visible = true;
            gasCostLabel.Text = "GAS used: " + gasUsed; 
        }

        #endregion

        #region DEBUG MENU

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_debugger.AvmFileLoaded)
                return;

            RunDebugger();
        }
        
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (!_debugger.AvmFileLoaded)
                return;

            OpenStorage();
        }

        private void stepToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(!_debugger.AvmFileLoaded)
                return;

            StepDebugger();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_debugger.AvmFileLoaded)
                return;

            ResetDebugger();
        }

        private void originalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_debugger.AvmFileLoaded)
                return;

            if (_debugger.Mode != DebugMode.Source)
            {
                ToggleDebuggerSource();
            }
        }

        private void assemblyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_debugger.AvmFileLoaded)
                return;

            if (_debugger.Mode != DebugMode.Assembly)
            {
                ToggleDebuggerSource();
            }
        }

        private void blockchainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!_debugger.BlockchainLoaded)
            {
                MessageBox.Show("Please deploy the smart contract first!");
                return;
            }

            var form = new BlockchainForm(_debugger.Blockchain);
            form.ShowDialog();
        }

        private void cCompilerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new CSharpCompilerForm(_settings);
            form.LoadCompiledContract += Form_LoadCompiledContract;
            form.ShowDialog();
        }

        private void Form_LoadCompiledContract(object sender, LoadCompiledContractEventArgs e)
        {
            LoadDebugFile(e.AvmPath);
        }

        private void keyDecoderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var form = new KeyToolForm();
            form.ShowDialog();
        }

        #endregion

        #region UI Helpers

        //Make sure the update executes on the main UI thread
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

        #region Helpers

        private void _debugger_SendToLog(object sender, DebugManagerLogEventArgs e)
        {
            SendLogToPanel(e.Message);
        }

        #endregion
    }
}
