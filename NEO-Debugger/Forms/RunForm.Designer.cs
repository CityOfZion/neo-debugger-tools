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
            this.assetListBox = new System.Windows.Forms.ListBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.assetAmmount = new System.Windows.Forms.TextBox();
            this.inputGrid = new System.Windows.Forms.DataGridView();
            this.Parameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.button3 = new System.Windows.Forms.Button();
            this.addressLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.inputGrid)).BeginInit();
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
            this.paramsList.Location = new System.Drawing.Point(12, 25);
            this.paramsList.Name = "paramsList";
            this.paramsList.Size = new System.Drawing.Size(277, 147);
            this.paramsList.TabIndex = 2;
            this.paramsList.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            this.paramsList.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.paramsList_MouseDoubleClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(292, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 13);
            this.label3.TabIndex = 11;
            this.label3.Text = "Asset Transfer";
            // 
            // assetListBox
            // 
            this.assetListBox.FormattingEnabled = true;
            this.assetListBox.Location = new System.Drawing.Point(295, 25);
            this.assetListBox.Name = "assetListBox";
            this.assetListBox.Size = new System.Drawing.Size(173, 69);
            this.assetListBox.TabIndex = 12;
            this.assetListBox.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged_1);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 13;
            this.label4.Text = "Functions";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(292, 99);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 14;
            this.label5.Text = "Ammount";
            // 
            // assetAmmount
            // 
            this.assetAmmount.Location = new System.Drawing.Point(295, 115);
            this.assetAmmount.Name = "assetAmmount";
            this.assetAmmount.Size = new System.Drawing.Size(173, 20);
            this.assetAmmount.TabIndex = 15;
            this.assetAmmount.Text = "0";
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
            this.inputGrid.Location = new System.Drawing.Point(13, 178);
            this.inputGrid.Name = "inputGrid";
            this.inputGrid.RowHeadersVisible = false;
            this.inputGrid.Size = new System.Drawing.Size(455, 125);
            this.inputGrid.TabIndex = 16;
            this.inputGrid.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.inputGrid_CellBeginEdit);
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
            this.button3.Location = new System.Drawing.Point(299, 329);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(169, 25);
            this.button3.TabIndex = 19;
            this.button3.Text = "Load Private Key";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // addressLabel
            // 
            this.addressLabel.AutoSize = true;
            this.addressLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.addressLabel.Location = new System.Drawing.Point(12, 335);
            this.addressLabel.Name = "addressLabel";
            this.addressLabel.Size = new System.Drawing.Size(249, 13);
            this.addressLabel.TabIndex = 18;
            this.addressLabel.Text = "AakVz7XchUZzxbX6fAP6dA7ix6mygHm888";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 316);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 13);
            this.label2.TabIndex = 17;
            this.label2.Text = "Invoker Address";
            // 
            // RunForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(482, 398);
            this.ControlBox = false;
            this.Controls.Add(this.button3);
            this.Controls.Add(this.addressLabel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.inputGrid);
            this.Controls.Add(this.assetAmmount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.assetListBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.paramsList);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "RunForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Invoke Smart Contract";
            this.Shown += new System.EventHandler(this.RunForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.inputGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ListBox paramsList;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ListBox assetListBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox assetAmmount;
        private System.Windows.Forms.DataGridView inputGrid;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label addressLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridViewTextBoxColumn Parameter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
    }
}