namespace Neo.Debugger.Forms
{
    partial class KeyToolForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.keyBox = new System.Windows.Forms.TextBox();
            this.keyDataGrid = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.keyDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Private Key";
            // 
            // keyBox
            // 
            this.keyBox.Location = new System.Drawing.Point(15, 25);
            this.keyBox.Name = "keyBox";
            this.keyBox.Size = new System.Drawing.Size(495, 20);
            this.keyBox.TabIndex = 1;
            // 
            // keyDataGrid
            // 
            this.keyDataGrid.AllowUserToAddRows = false;
            this.keyDataGrid.AllowUserToDeleteRows = false;
            this.keyDataGrid.AllowUserToResizeRows = false;
            this.keyDataGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.keyDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.keyDataGrid.Location = new System.Drawing.Point(15, 51);
            this.keyDataGrid.Name = "keyDataGrid";
            this.keyDataGrid.Size = new System.Drawing.Size(717, 281);
            this.keyDataGrid.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(516, 25);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(106, 22);
            this.button1.TabIndex = 3;
            this.button1.Text = "Decode";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(628, 25);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(104, 22);
            this.button2.TabIndex = 4;
            this.button2.Text = "Random";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // KeyToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(744, 344);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.keyDataGrid);
            this.Controls.Add(this.keyBox);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "KeyToolForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Key Decoder Tool";
            this.Shown += new System.EventHandler(this.KeyToolForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.keyDataGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox keyBox;
        private System.Windows.Forms.DataGridView keyDataGrid;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
    }
}