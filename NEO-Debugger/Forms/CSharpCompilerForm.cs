using Neo.Debugger.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Neo.Debugger.Forms
{
    public partial class CSharpCompilerForm : Form
    {
        private Settings _settings;
        private string _avmPath;

        public event LoadCompiledContractEventHandler LoadCompiledContract;
        public delegate void LoadCompiledContractEventHandler(object sender, LoadCompiledContractEventArgs e);

        public CSharpCompilerForm(Settings settings)
        {
            InitializeComponent();
            _settings = settings;
        }

        private void CSharpCompilerForm_Shown(object sender, EventArgs e)
        {
            debugBtn.Enabled = false;
            listBox1.Items.Clear();
            listBox1.Visible = false;
            this.Height -= listBox1.Height;
        }

        private void CSharpCompilerForm_Load(object sender, EventArgs e)
        {

        }

        private void compileBtn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(outputNameText.Text))
            {
                MessageBox.Show("Insert an output name for this contract.");
                return;
            }

            listBox1.Visible = true;
            this.Height += listBox1.Height;

            listBox1.Items.Clear();

            var path = _settings.path;
            Directory.CreateDirectory(path);

            var fileName = path + @"\"+outputNameText.Text+".cs";
            _avmPath = fileName.Replace(".cs", ".avm");
            File.WriteAllText(fileName, sourceCodeText.Text);

            var proc = new Process();
            proc.StartInfo.FileName = "neon.exe";
            proc.StartInfo.Arguments = "\""+fileName+"\"";
            proc.StartInfo.UseShellExecute = false;
            proc.StartInfo.RedirectStandardInput = false;
            proc.StartInfo.RedirectStandardOutput = true;
            proc.StartInfo.RedirectStandardError = true;
            proc.StartInfo.CreateNoWindow = true;

            proc.EnableRaisingEvents = true;

            debugBtn.Enabled = false;

            try
            {
                listBox1.Items.Add("Starting compilation...");

                proc.Start();
                proc.WaitForExit();

                var log = proc.StandardOutput.ReadToEnd().Split('\n');
                string last = null;
                foreach (var temp in log)
                {
                    var line = temp.Replace("\r", "");
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }
                    listBox1.Items.Add(line);

                    last = line;
                }

                if (proc.ExitCode != 0 || last != "SUCC")
                {
                    log = proc.StandardError.ReadToEnd().Split('\n');
                    foreach (var line in log)
                    {
                        listBox1.Items.Add(line);
                    }

                    listBox1.Items.Add("Error during compilation.");
                }
                else
                {
                    listBox1.Items.Add("Compilation sucessfull.");
                    debugBtn.Enabled = true;
                }
            }
            catch (Exception ex)
            {
                listBox1.Items.Add(ex);
            }
        }

        private void debugBtn_Click(object sender, EventArgs e)
        {
            //Invoke the event handler
            LoadCompiledContract?.Invoke(this, new LoadCompiledContractEventArgs
            {
                AvmPath = _avmPath
            });

            this.Close();
        }
    }
}
