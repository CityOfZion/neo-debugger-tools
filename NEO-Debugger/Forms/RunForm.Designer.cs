namespace Neo.Debugger.Forms
{
    partial class RunForm
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.paramsList = new System.Windows.Forms.ListBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.assetAmount = new System.Windows.Forms.TextBox();
            this.inputGrid = new System.Windows.Forms.DataGridView();
            this.Parameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button3 = new System.Windows.Forms.Button();
            this.addressLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.runTabs = new System.Windows.Forms.TabControl();
            this.methodTab = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.testCasesList = new System.Windows.Forms.ListBox();
            this.transactionPage = new System.Windows.Forms.TabPage();
            this.privateKeyInput = new System.Windows.Forms.TextBox();
            this.assetComboBox = new System.Windows.Forms.ComboBox();
            this.optionsPage = new System.Windows.Forms.TabPage();
            this.label5 = new System.Windows.Forms.Label();
            this.witnessComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.triggerComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.timestampBox = new System.Windows.Forms.TextBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            ((System.ComponentModel.ISupportInitialize)(this.inputGrid)).BeginInit();
            this.runTabs.SuspendLayout();
            this.methodTab.SuspendLayout();
            this.transactionPage.SuspendLayout();
            this.optionsPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Enabled = false;
            this.button1.Location = new System.Drawing.Point(350, 365);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(118, 26);
            this.button1.TabIndex = 0;
            this.button1.Text = "Debug";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(15, 369);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(118, 26);
            this.button2.TabIndex = 1;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // paramsList
            // 
            this.paramsList.FormattingEnabled = true;
            this.paramsList.Location = new System.Drawing.Point(9, 28);
            this.paramsList.Name = "paramsList";
            this.paramsList.Size = new System.Drawing.Size(206, 147);
            this.paramsList.TabIndex = 2;
            this.paramsList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.paramsList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.paramsList_MouseDoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Asset Transfer";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Functions";
            // 
            // assetAmount
            // 
            this.assetAmount.Location = new System.Drawing.Point(9, 186);
            this.assetAmount.Name = "assetAmount";
            this.assetAmount.Size = new System.Drawing.Size(173, 20);
            this.assetAmount.TabIndex = 15;
            this.assetAmount.Text = "0";
            // 
            // inputGrid
            // 
            this.inputGrid.AllowUserToAddRows = false;
            this.inputGrid.AllowUserToDeleteRows = false;
            this.inputGrid.AllowUserToResizeRows = false;
            this.inputGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.inputGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Parameter,
            this.Value});
            this.inputGrid.Location = new System.Drawing.Point(9, 181);
            this.inputGrid.Name = "inputGrid";
            this.inputGrid.RowHeadersVisible = false;
            this.inputGrid.Size = new System.Drawing.Size(433, 125);
            this.inputGrid.TabIndex = 16;
            this.inputGrid.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.inputGrid_CellBeginEdit);
            this.inputGrid.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputGrid_CellClick);
            this.inputGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputGrid_CellEndEdit);
            this.inputGrid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputGrid_CellEnter);
            this.inputGrid.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.inputGrid_CellLeave);
            // 
            // Parameter
            // 
            this.Parameter.HeaderText = "Parameter";
            this.Parameter.Name = "Parameter";
            this.Parameter.ReadOnly = true;
            this.Parameter.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // Value
            // 
            this.Value.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Value.HeaderText = "Value";
            this.Value.Name = "Value";
            this.Value.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(341, 103);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(101, 27);
            this.button3.TabIndex = 19;
            this.button3.Text = "Load Private Key";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click_1);
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressLabel.Location = new System.Drawing.Point(6, 47);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(249, 13);
            this.addressLabel.TabIndex = 18;
            this.addressLabel.Text = "AakVz7XchUZzxbX6fAP6dA7ix6mygHm888";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Invoker Address";
            // 
            // runTabs
            // 
            this.runTabs.Controls.Add(this.methodTab);
            this.runTabs.Controls.Add(this.transactionPage);
            this.runTabs.Controls.Add(this.optionsPage);
            this.runTabs.Location = new System.Drawing.Point(12, 12);
            this.runTabs.Name = "runTabs";
            this.runTabs.SelectedIndex = 0;
            this.runTabs.Size = new System.Drawing.Size(456, 342);
            this.runTabs.TabIndex = 20;
            // 
            // methodTab
            // 
            this.methodTab.Controls.Add(this.label6);
            this.methodTab.Controls.Add(this.testCasesList);
            this.methodTab.Controls.Add(this.label4);
            this.methodTab.Controls.Add(this.paramsList);
            this.methodTab.Controls.Add(this.inputGrid);
            this.methodTab.Location = new System.Drawing.Point(4, 22);
            this.methodTab.Name = "methodTab";
            this.methodTab.Padding = new System.Windows.Forms.Padding(3);
            this.methodTab.Size = new System.Drawing.Size(448, 316);
            this.methodTab.TabIndex = 0;
            this.methodTab.Text = "Method";
            this.methodTab.UseVisualStyleBackColor = true;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(218, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(59, 13);
            this.label6.TabIndex = 18;
            this.label6.Text = "Test cases";
            // 
            // testCasesList
            // 
            this.testCasesList.FormattingEnabled = true;
            this.testCasesList.Location = new System.Drawing.Point(221, 28);
            this.testCasesList.Name = "testCasesList";
            this.testCasesList.Size = new System.Drawing.Size(221, 147);
            this.testCasesList.TabIndex = 17;
            this.testCasesList.SelectedIndexChanged += new System.EventHandler(this.testCasesList_SelectedIndexChanged);
            // 
            // transactionPage
            // 
            this.transactionPage.Controls.Add(this.privateKeyInput);
            this.transactionPage.Controls.Add(this.assetComboBox);
            this.transactionPage.Controls.Add(this.label2);
            this.transactionPage.Controls.Add(this.assetAmount);
            this.transactionPage.Controls.Add(this.button3);
            this.transactionPage.Controls.Add(this.addressLabel);
            this.transactionPage.Controls.Add(this.label3);
            this.transactionPage.Location = new System.Drawing.Point(4, 22);
            this.transactionPage.Name = "transactionPage";
            this.transactionPage.Padding = new System.Windows.Forms.Padding(3);
            this.transactionPage.Size = new System.Drawing.Size(448, 316);
            this.transactionPage.TabIndex = 1;
            this.transactionPage.Text = "Transaction";
            this.transactionPage.UseVisualStyleBackColor = true;
            // 
            // privateKeyInput
            // 
            this.privateKeyInput.Location = new System.Drawing.Point(9, 77);
            this.privateKeyInput.Name = "privateKeyInput";
            this.privateKeyInput.Size = new System.Drawing.Size(433, 20);
            this.privateKeyInput.TabIndex = 21;
            // 
            // assetComboBox
            // 
            this.assetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.assetComboBox.FormattingEnabled = true;
            this.assetComboBox.Location = new System.Drawing.Point(188, 186);
            this.assetComboBox.Name = "assetComboBox";
            this.assetComboBox.Size = new System.Drawing.Size(106, 21);
            this.assetComboBox.TabIndex = 20;
            this.assetComboBox.SelectedIndexChanged += new System.EventHandler(this.assetComboBox_SelectedIndexChanged_1);
            // 
            // optionsPage
            // 
            this.optionsPage.Controls.Add(this.dateTimePicker1);
            this.optionsPage.Controls.Add(this.timestampBox);
            this.optionsPage.Controls.Add(this.label7);
            this.optionsPage.Controls.Add(this.label5);
            this.optionsPage.Controls.Add(this.witnessComboBox);
            this.optionsPage.Controls.Add(this.label1);
            this.optionsPage.Controls.Add(this.triggerComboBox);
            this.optionsPage.Location = new System.Drawing.Point(4, 22);
            this.optionsPage.Name = "optionsPage";
            this.optionsPage.Padding = new System.Windows.Forms.Padding(3);
            this.optionsPage.Size = new System.Drawing.Size(448, 316);
            this.optionsPage.TabIndex = 2;
            this.optionsPage.Text = "Options";
            this.optionsPage.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 58);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(76, 13);
            this.label5.TabIndex = 3;
            this.label5.Text = "CheckWitness";
            // 
            // witnessComboBox
            // 
            this.witnessComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.witnessComboBox.FormattingEnabled = true;
            this.witnessComboBox.Items.AddRange(new object[] {
            "Default",
            "Always True",
            "Always False"});
            this.witnessComboBox.Location = new System.Drawing.Point(99, 55);
            this.witnessComboBox.Name = "witnessComboBox";
            this.witnessComboBox.Size = new System.Drawing.Size(121, 21);
            this.witnessComboBox.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Trigger";
            // 
            // triggerComboBox
            // 
            this.triggerComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.triggerComboBox.FormattingEnabled = true;
            this.triggerComboBox.Items.AddRange(new object[] {
            "Application",
            "Verification"});
            this.triggerComboBox.Location = new System.Drawing.Point(99, 18);
            this.triggerComboBox.Name = "triggerComboBox";
            this.triggerComboBox.Size = new System.Drawing.Size(121, 21);
            this.triggerComboBox.TabIndex = 0;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 95);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(58, 13);
            this.label7.TabIndex = 4;
            this.label7.Text = "Timestamp";
            // 
            // timestampBox
            // 
            this.timestampBox.Location = new System.Drawing.Point(99, 92);
            this.timestampBox.Name = "timestampBox";
            this.timestampBox.Size = new System.Drawing.Size(121, 20);
            this.timestampBox.TabIndex = 5;
            this.timestampBox.TextChanged += new System.EventHandler(this.timestampBox_TextChanged);
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(226, 92);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(200, 20);
            this.dateTimePicker1.TabIndex = 6;
            this.dateTimePicker1.ValueChanged += new System.EventHandler(this.dateTimePicker1_ValueChanged);
            // 
            // RunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 398);
            this.ControlBox = false;
            this.Controls.Add(this.runTabs);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RunForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Invoke Smart Contract";
            this.Shown += new System.EventHandler(this.RunForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.inputGrid)).EndInit();
            this.runTabs.ResumeLayout(false);
            this.methodTab.ResumeLayout(false);
            this.methodTab.PerformLayout();
            this.transactionPage.ResumeLayout(false);
            this.transactionPage.PerformLayout();
            this.optionsPage.ResumeLayout(false);
            this.optionsPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox paramsList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox assetAmount;
        private System.Windows.Forms.DataGridView inputGrid;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Parameter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.TabControl runTabs;
        private System.Windows.Forms.TabPage methodTab;
        private System.Windows.Forms.TabPage transactionPage;
        private System.Windows.Forms.ComboBox assetComboBox;
        private System.Windows.Forms.TextBox privateKeyInput;
        private System.Windows.Forms.TabPage optionsPage;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox witnessComboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox triggerComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ListBox testCasesList;
        private System.Windows.Forms.TextBox timestampBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
    }
}