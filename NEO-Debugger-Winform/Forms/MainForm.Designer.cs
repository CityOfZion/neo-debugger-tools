﻿namespace Neo.Debugger.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.TextPanel = new System.Windows.Forms.Panel();
            this.FileName = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newFromTemplateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findDialogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.findAndReplaceToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.goToLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wordWrapItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indentGuidesItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hiddenCharactersItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomInToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoomOutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zoom100ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stepToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebuildToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.blockchainToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cCompilerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyDecoderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.codeGeneratorToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.neoLuxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.PanelSearch = new System.Windows.Forms.Panel();
            this.BtnNextSearch = new System.Windows.Forms.Button();
            this.BtnPrevSearch = new System.Windows.Forms.Button();
            this.BtnCloseSearch = new System.Windows.Forms.Button();
            this.TxtSearch = new System.Windows.Forms.TextBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.logView = new System.Windows.Forms.RichTextBox();
            this.logLabel = new System.Windows.Forms.Label();
            this.stackLabel = new System.Windows.Forms.Label();
            this.gasCostLabel = new System.Windows.Forms.Label();
            this.projectTree = new System.Windows.Forms.TreeView();
            this.stackPanel = new System.Windows.Forms.DataGridView();
            this.menuStrip1.SuspendLayout();
            this.PanelSearch.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stackPanel)).BeginInit();
            this.SuspendLayout();
            // 
            // TextPanel
            // 
            this.TextPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextPanel.Location = new System.Drawing.Point(208, 95);
            this.TextPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TextPanel.Name = "TextPanel";
            this.TextPanel.Size = new System.Drawing.Size(549, 345);
            this.TextPanel.TabIndex = 0;
            // 
            // FileName
            // 
            this.FileName.AutoSize = true;
            this.FileName.Font = new System.Drawing.Font("Segoe UI Light", 16.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FileName.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.FileName.Location = new System.Drawing.Point(12, 45);
            this.FileName.Name = "FileName";
            this.FileName.Size = new System.Drawing.Size(52, 30);
            this.FileName.TabIndex = 1;
            this.FileName.Text = "Title";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.menuStrip1.Font = new System.Drawing.Font("Segoe UI", 10.2F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.blockchainToolStripMenuItem1,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(769, 27);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.newFromTemplateToolStripMenuItem,
            this.openToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(41, 23);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(200, 24);
            this.newToolStripMenuItem.Text = "New";
            // 
            // newFromTemplateToolStripMenuItem
            // 
            this.newFromTemplateToolStripMenuItem.Name = "newFromTemplateToolStripMenuItem";
            this.newFromTemplateToolStripMenuItem.Size = new System.Drawing.Size(200, 24);
            this.newFromTemplateToolStripMenuItem.Text = "New From Template";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(200, 24);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(197, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(200, 24);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findToolStripMenuItem,
            this.findDialogToolStripMenuItem,
            this.findAndReplaceToolStripMenuItem,
            this.toolStripSeparator7,
            this.goToLineToolStripMenuItem});
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(61, 23);
            this.searchToolStripMenuItem.Text = "Search";
            // 
            // findToolStripMenuItem
            // 
            this.findToolStripMenuItem.Name = "findToolStripMenuItem";
            this.findToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+F";
            this.findToolStripMenuItem.Size = new System.Drawing.Size(241, 24);
            this.findToolStripMenuItem.Text = "Quick Find...";
            this.findToolStripMenuItem.Click += new System.EventHandler(this.findToolStripMenuItem_Click);
            // 
            // findDialogToolStripMenuItem
            // 
            this.findDialogToolStripMenuItem.Name = "findDialogToolStripMenuItem";
            this.findDialogToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Alt+F";
            this.findDialogToolStripMenuItem.Size = new System.Drawing.Size(241, 24);
            this.findDialogToolStripMenuItem.Text = "Find...";
            this.findDialogToolStripMenuItem.Click += new System.EventHandler(this.findDialogToolStripMenuItem_Click);
            // 
            // findAndReplaceToolStripMenuItem
            // 
            this.findAndReplaceToolStripMenuItem.Name = "findAndReplaceToolStripMenuItem";
            this.findAndReplaceToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+H";
            this.findAndReplaceToolStripMenuItem.Size = new System.Drawing.Size(241, 24);
            this.findAndReplaceToolStripMenuItem.Text = "Find and Replace...";
            this.findAndReplaceToolStripMenuItem.Click += new System.EventHandler(this.findAndReplaceToolStripMenuItem_Click);
            // 
            // toolStripSeparator7
            // 
            this.toolStripSeparator7.Name = "toolStripSeparator7";
            this.toolStripSeparator7.Size = new System.Drawing.Size(238, 6);
            // 
            // goToLineToolStripMenuItem
            // 
            this.goToLineToolStripMenuItem.Name = "goToLineToolStripMenuItem";
            this.goToLineToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+G";
            this.goToLineToolStripMenuItem.Size = new System.Drawing.Size(241, 24);
            this.goToLineToolStripMenuItem.Text = "Go To Line...";
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wordWrapItem,
            this.indentGuidesItem,
            this.hiddenCharactersItem,
            this.toolStripSeparator4,
            this.zoomInToolStripMenuItem,
            this.zoomOutToolStripMenuItem,
            this.zoom100ToolStripMenuItem,
            this.toolStripSeparator5,
            this.collapseAllToolStripMenuItem,
            this.expandAllToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(50, 23);
            this.viewToolStripMenuItem.Text = "View";
            // 
            // wordWrapItem
            // 
            this.wordWrapItem.Name = "wordWrapItem";
            this.wordWrapItem.Size = new System.Drawing.Size(221, 24);
            this.wordWrapItem.Text = "Word Wrap";
            this.wordWrapItem.Click += new System.EventHandler(this.wordWrapToolStripMenuItem1_Click);
            // 
            // indentGuidesItem
            // 
            this.indentGuidesItem.Checked = true;
            this.indentGuidesItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.indentGuidesItem.Name = "indentGuidesItem";
            this.indentGuidesItem.Size = new System.Drawing.Size(221, 24);
            this.indentGuidesItem.Text = "Show Indent Guides";
            this.indentGuidesItem.Click += new System.EventHandler(this.indentGuidesToolStripMenuItem_Click);
            // 
            // hiddenCharactersItem
            // 
            this.hiddenCharactersItem.Name = "hiddenCharactersItem";
            this.hiddenCharactersItem.Size = new System.Drawing.Size(221, 24);
            this.hiddenCharactersItem.Text = "Show Whitespace";
            this.hiddenCharactersItem.Click += new System.EventHandler(this.hiddenCharactersToolStripMenuItem_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(218, 6);
            // 
            // zoomInToolStripMenuItem
            // 
            this.zoomInToolStripMenuItem.Name = "zoomInToolStripMenuItem";
            this.zoomInToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Plus";
            this.zoomInToolStripMenuItem.Size = new System.Drawing.Size(221, 24);
            this.zoomInToolStripMenuItem.Text = "Zoom In";
            this.zoomInToolStripMenuItem.Click += new System.EventHandler(this.zoomInToolStripMenuItem_Click);
            // 
            // zoomOutToolStripMenuItem
            // 
            this.zoomOutToolStripMenuItem.Name = "zoomOutToolStripMenuItem";
            this.zoomOutToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+Minus";
            this.zoomOutToolStripMenuItem.Size = new System.Drawing.Size(221, 24);
            this.zoomOutToolStripMenuItem.Text = "Zoom Out";
            this.zoomOutToolStripMenuItem.Click += new System.EventHandler(this.zoomOutToolStripMenuItem_Click);
            // 
            // zoom100ToolStripMenuItem
            // 
            this.zoom100ToolStripMenuItem.Name = "zoom100ToolStripMenuItem";
            this.zoom100ToolStripMenuItem.ShortcutKeyDisplayString = "Ctrl+0";
            this.zoom100ToolStripMenuItem.Size = new System.Drawing.Size(221, 24);
            this.zoom100ToolStripMenuItem.Text = "Zoom 100%";
            this.zoom100ToolStripMenuItem.Click += new System.EventHandler(this.zoom100ToolStripMenuItem_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(218, 6);
            // 
            // collapseAllToolStripMenuItem
            // 
            this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
            this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(221, 24);
            this.collapseAllToolStripMenuItem.Text = "Collapse All";
            this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
            // 
            // expandAllToolStripMenuItem
            // 
            this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
            this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(221, 24);
            this.expandAllToolStripMenuItem.Text = "Expand All";
            this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.stepToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.rebuildToolStripMenuItem,
            this.toolStripMenuItem2});
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(74, 23);
            this.debugToolStripMenuItem.Text = "Contract";
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.ShortcutKeyDisplayString = "F5";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(149, 24);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // stepToolStripMenuItem
            // 
            this.stepToolStripMenuItem.Name = "stepToolStripMenuItem";
            this.stepToolStripMenuItem.ShortcutKeyDisplayString = "F10";
            this.stepToolStripMenuItem.Size = new System.Drawing.Size(149, 24);
            this.stepToolStripMenuItem.Text = "Step";
            this.stepToolStripMenuItem.Click += new System.EventHandler(this.stepToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(149, 24);
            this.resetToolStripMenuItem.Text = "Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // rebuildToolStripMenuItem
            // 
            this.rebuildToolStripMenuItem.Name = "rebuildToolStripMenuItem";
            this.rebuildToolStripMenuItem.Size = new System.Drawing.Size(149, 24);
            this.rebuildToolStripMenuItem.Text = "Rebuild";
            this.rebuildToolStripMenuItem.Click += new System.EventHandler(this.rebuildToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.ShortcutKeyDisplayString = "F6";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(149, 24);
            this.toolStripMenuItem2.Text = "Storage";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // blockchainToolStripMenuItem1
            // 
            this.blockchainToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.viewToolStripMenuItem1,
            this.loadToolStripMenuItem,
            this.resetToolStripMenuItem1});
            this.blockchainToolStripMenuItem1.Name = "blockchainToolStripMenuItem1";
            this.blockchainToolStripMenuItem1.Size = new System.Drawing.Size(85, 23);
            this.blockchainToolStripMenuItem1.Text = "Blockchain";
            // 
            // viewToolStripMenuItem1
            // 
            this.viewToolStripMenuItem1.Name = "viewToolStripMenuItem1";
            this.viewToolStripMenuItem1.Size = new System.Drawing.Size(111, 24);
            this.viewToolStripMenuItem1.Text = "View";
            this.viewToolStripMenuItem1.Click += new System.EventHandler(this.blockchainToolStripMenuItem_Click);
            // 
            // loadToolStripMenuItem
            // 
            this.loadToolStripMenuItem.Name = "loadToolStripMenuItem";
            this.loadToolStripMenuItem.Size = new System.Drawing.Size(111, 24);
            this.loadToolStripMenuItem.Text = "Load";
            this.loadToolStripMenuItem.Click += new System.EventHandler(this.loadToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem1
            // 
            this.resetToolStripMenuItem1.Name = "resetToolStripMenuItem1";
            this.resetToolStripMenuItem1.Size = new System.Drawing.Size(111, 24);
            this.resetToolStripMenuItem1.Text = "Reset";
            this.resetToolStripMenuItem1.Click += new System.EventHandler(this.resetToolStripMenuItem1_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cCompilerToolStripMenuItem,
            this.keyDecoderToolStripMenuItem,
            this.codeGeneratorToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(52, 23);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // cCompilerToolStripMenuItem
            // 
            this.cCompilerToolStripMenuItem.Name = "cCompilerToolStripMenuItem";
            this.cCompilerToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.cCompilerToolStripMenuItem.Text = "Compile";
            this.cCompilerToolStripMenuItem.Click += new System.EventHandler(this.cCompilerToolStripMenuItem_Click);
            // 
            // keyDecoderToolStripMenuItem
            // 
            this.keyDecoderToolStripMenuItem.Name = "keyDecoderToolStripMenuItem";
            this.keyDecoderToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.keyDecoderToolStripMenuItem.Text = "Key Tool";
            this.keyDecoderToolStripMenuItem.Click += new System.EventHandler(this.keyDecoderToolStripMenuItem_Click);
            // 
            // codeGeneratorToolStripMenuItem
            // 
            this.codeGeneratorToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.neoLuxToolStripMenuItem});
            this.codeGeneratorToolStripMenuItem.Name = "codeGeneratorToolStripMenuItem";
            this.codeGeneratorToolStripMenuItem.Size = new System.Drawing.Size(176, 24);
            this.codeGeneratorToolStripMenuItem.Text = "Code Generator";
            // 
            // neoLuxToolStripMenuItem
            // 
            this.neoLuxToolStripMenuItem.Name = "neoLuxToolStripMenuItem";
            this.neoLuxToolStripMenuItem.Size = new System.Drawing.Size(130, 24);
            this.neoLuxToolStripMenuItem.Text = "Neo-Lux";
            this.neoLuxToolStripMenuItem.Click += new System.EventHandler(this.neoLuxToolStripMenuItem_Click);
            // 
            // PanelSearch
            // 
            this.PanelSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.PanelSearch.BackColor = System.Drawing.Color.White;
            this.PanelSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.PanelSearch.Controls.Add(this.BtnNextSearch);
            this.PanelSearch.Controls.Add(this.BtnPrevSearch);
            this.PanelSearch.Controls.Add(this.BtnCloseSearch);
            this.PanelSearch.Controls.Add(this.TxtSearch);
            this.PanelSearch.Location = new System.Drawing.Point(462, 93);
            this.PanelSearch.Name = "PanelSearch";
            this.PanelSearch.Size = new System.Drawing.Size(292, 40);
            this.PanelSearch.TabIndex = 10;
            this.PanelSearch.Visible = false;
            // 
            // BtnNextSearch
            // 
            this.BtnNextSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnNextSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnNextSearch.ForeColor = System.Drawing.Color.White;
            this.BtnNextSearch.Image = ((System.Drawing.Image)(resources.GetObject("BtnNextSearch.Image")));
            this.BtnNextSearch.Location = new System.Drawing.Point(233, 4);
            this.BtnNextSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnNextSearch.Name = "BtnNextSearch";
            this.BtnNextSearch.Size = new System.Drawing.Size(25, 30);
            this.BtnNextSearch.TabIndex = 9;
            this.BtnNextSearch.Tag = "Find next (Enter)";
            this.BtnNextSearch.UseVisualStyleBackColor = true;
            this.BtnNextSearch.Click += new System.EventHandler(this.BtnNextSearch_Click);
            // 
            // BtnPrevSearch
            // 
            this.BtnPrevSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnPrevSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnPrevSearch.ForeColor = System.Drawing.Color.White;
            this.BtnPrevSearch.Image = ((System.Drawing.Image)(resources.GetObject("BtnPrevSearch.Image")));
            this.BtnPrevSearch.Location = new System.Drawing.Point(205, 4);
            this.BtnPrevSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnPrevSearch.Name = "BtnPrevSearch";
            this.BtnPrevSearch.Size = new System.Drawing.Size(25, 30);
            this.BtnPrevSearch.TabIndex = 8;
            this.BtnPrevSearch.Tag = "Find previous (Shift+Enter)";
            this.BtnPrevSearch.UseVisualStyleBackColor = true;
            this.BtnPrevSearch.Click += new System.EventHandler(this.BtnPrevSearch_Click);
            // 
            // BtnCloseSearch
            // 
            this.BtnCloseSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.BtnCloseSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.BtnCloseSearch.ForeColor = System.Drawing.Color.White;
            this.BtnCloseSearch.Image = ((System.Drawing.Image)(resources.GetObject("BtnCloseSearch.Image")));
            this.BtnCloseSearch.Location = new System.Drawing.Point(261, 4);
            this.BtnCloseSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.BtnCloseSearch.Name = "BtnCloseSearch";
            this.BtnCloseSearch.Size = new System.Drawing.Size(25, 30);
            this.BtnCloseSearch.TabIndex = 7;
            this.BtnCloseSearch.Tag = "Close (Esc)";
            this.BtnCloseSearch.UseVisualStyleBackColor = true;
            this.BtnCloseSearch.Click += new System.EventHandler(this.BtnClearSearch_Click);
            // 
            // TxtSearch
            // 
            this.TxtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TxtSearch.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TxtSearch.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TxtSearch.Location = new System.Drawing.Point(10, 6);
            this.TxtSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TxtSearch.Name = "TxtSearch";
            this.TxtSearch.Size = new System.Drawing.Size(189, 25);
            this.TxtSearch.TabIndex = 6;
            this.TxtSearch.TextChanged += new System.EventHandler(this.TxtSearch_TextChanged);
            this.TxtSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TxtSearch_KeyDown);
            // 
            // openFileDialog
            // 
            this.openFileDialog.DefaultExt = "txt";
            this.openFileDialog.Filter = "NEO AVM files|*.avm";
            // 
            // logView
            // 
            this.logView.Location = new System.Drawing.Point(8, 463);
            this.logView.Name = "logView";
            this.logView.ReadOnly = true;
            this.logView.Size = new System.Drawing.Size(361, 153);
            this.logView.TabIndex = 11;
            this.logView.Text = "";
            // 
            // logLabel
            // 
            this.logLabel.AutoSize = true;
            this.logLabel.Location = new System.Drawing.Point(5, 445);
            this.logLabel.Name = "logLabel";
            this.logLabel.Size = new System.Drawing.Size(27, 15);
            this.logLabel.TabIndex = 12;
            this.logLabel.Text = "Log";
            // 
            // stackLabel
            // 
            this.stackLabel.AutoSize = true;
            this.stackLabel.Location = new System.Drawing.Point(372, 444);
            this.stackLabel.Name = "stackLabel";
            this.stackLabel.Size = new System.Drawing.Size(35, 15);
            this.stackLabel.TabIndex = 14;
            this.stackLabel.Text = "Stack";
            // 
            // gasCostLabel
            // 
            this.gasCostLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.gasCostLabel.AutoSize = true;
            this.gasCostLabel.Location = new System.Drawing.Point(730, 45);
            this.gasCostLabel.Name = "gasCostLabel";
            this.gasCostLabel.Size = new System.Drawing.Size(27, 15);
            this.gasCostLabel.TabIndex = 15;
            this.gasCostLabel.Text = "Log";
            this.gasCostLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.gasCostLabel.Visible = false;
            // 
            // projectTree
            // 
            this.projectTree.Location = new System.Drawing.Point(12, 95);
            this.projectTree.Name = "projectTree";
            this.projectTree.Size = new System.Drawing.Size(181, 345);
            this.projectTree.TabIndex = 16;
            this.projectTree.DoubleClick += new System.EventHandler(this.projectTree_DoubleClick);
            // 
            // stackPanel
            // 
            this.stackPanel.AllowUserToAddRows = false;
            this.stackPanel.AllowUserToDeleteRows = false;
            this.stackPanel.AllowUserToResizeRows = false;
            this.stackPanel.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.stackPanel.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.stackPanel.Location = new System.Drawing.Point(375, 463);
            this.stackPanel.MultiSelect = false;
            this.stackPanel.Name = "stackPanel";
            this.stackPanel.ReadOnly = true;
            this.stackPanel.RowHeadersVisible = false;
            this.stackPanel.Size = new System.Drawing.Size(382, 153);
            this.stackPanel.TabIndex = 17;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(769, 628);
            this.Controls.Add(this.stackPanel);
            this.Controls.Add(this.projectTree);
            this.Controls.Add(this.gasCostLabel);
            this.Controls.Add(this.stackLabel);
            this.Controls.Add(this.logLabel);
            this.Controls.Add(this.logView);
            this.Controls.Add(this.PanelSearch);
            this.Controls.Add(this.FileName);
            this.Controls.Add(this.TextPanel);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "Neo Debugger";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Resize += new System.EventHandler(this.MainForm_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.PanelSearch.ResumeLayout(false);
            this.PanelSearch.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stackPanel)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel TextPanel;
        private System.Windows.Forms.Label FileName;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wordWrapItem;
        private System.Windows.Forms.ToolStripMenuItem hiddenCharactersItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem zoomInToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomOutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoom100ToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem collapseAllToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandAllToolStripMenuItem;
        private System.Windows.Forms.Panel PanelSearch;
        private System.Windows.Forms.Button BtnNextSearch;
        private System.Windows.Forms.Button BtnPrevSearch;
        private System.Windows.Forms.Button BtnCloseSearch;
        private System.Windows.Forms.TextBox TxtSearch;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToLineToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem findAndReplaceToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
        private System.Windows.Forms.ToolStripMenuItem findDialogToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indentGuidesItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.RichTextBox logView;
        private System.Windows.Forms.Label logLabel;
        private System.Windows.Forms.Label stackLabel;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stepToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.Label gasCostLabel;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cCompilerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keyDecoderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newFromTemplateToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blockchainToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem loadToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem1;
        private System.Windows.Forms.TreeView projectTree;
        private System.Windows.Forms.ToolStripMenuItem rebuildToolStripMenuItem;
        private System.Windows.Forms.DataGridView stackPanel;
        private System.Windows.Forms.ToolStripMenuItem codeGeneratorToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem neoLuxToolStripMenuItem;
    }
}

