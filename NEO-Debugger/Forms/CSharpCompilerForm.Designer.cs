namespace Neo.Debugger.Forms
{
    partial class CSharpCompilerForm
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
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.compileBtn = new System.Windows.Forms.Button();
            this.debugBtn = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.outputNameText = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 12);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(944, 334);
            this.textBox1.TabIndex = 0;
            // 
            // compileBtn
            // 
            this.compileBtn.Location = new System.Drawing.Point(318, 352);
            this.compileBtn.Name = "compileBtn";
            this.compileBtn.Size = new System.Drawing.Size(122, 39);
            this.compileBtn.TabIndex = 2;
            this.compileBtn.Text = "Compile";
            this.compileBtn.UseVisualStyleBackColor = true;
            this.compileBtn.Click += new System.EventHandler(this.compileBtn_Click);
            // 
            // debugBtn
            // 
            this.debugBtn.Location = new System.Drawing.Point(446, 352);
            this.debugBtn.Name = "debugBtn";
            this.debugBtn.Size = new System.Drawing.Size(122, 39);
            this.debugBtn.TabIndex = 3;
            this.debugBtn.Text = "Debug";
            this.debugBtn.UseVisualStyleBackColor = true;
            this.debugBtn.Click += new System.EventHandler(this.debugBtn_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(12, 397);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(944, 134);
            this.listBox1.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 352);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Output Name";
            // 
            // outputNameText
            // 
            this.outputNameText.Location = new System.Drawing.Point(15, 368);
            this.outputNameText.Name = "outputNameText";
            this.outputNameText.Size = new System.Drawing.Size(196, 20);
            this.outputNameText.TabIndex = 6;
            this.outputNameText.Text = "Temp";
            // 
            // CSharpCompilerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(968, 541);
            this.Controls.Add(this.outputNameText);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.debugBtn);
            this.Controls.Add(this.compileBtn);
            this.Controls.Add(this.textBox1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CSharpCompilerForm";
            this.ShowIcon = false;
            this.Text = "C# Compiler";
            this.Load += new System.EventHandler(this.CSharpCompilerForm_Load);
            this.Shown += new System.EventHandler(this.CSharpCompilerForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button compileBtn;
        private System.Windows.Forms.Button debugBtn;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox outputNameText;
    }
}