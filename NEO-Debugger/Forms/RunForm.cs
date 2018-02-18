using LunarParser;
using LunarParser.JSON;
using Neo.Debugger.Utils;
using Neo.Emulator;
using Neo.Emulator.API;
using Neo.Emulator.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Windows.Forms;

namespace Neo.Debugger.Forms
{
    public partial class RunForm : Form
    {
        public NeoEmulator emulator;
        public ABI abi;

        private string lastParams = null;

        public RunForm()
        {
            InitializeComponent();

            assetListBox.Items.Clear();
            assetListBox.Items.Add("None");
            foreach (var entry in Asset.Entries)
            {
                assetListBox.Items.Add(entry.name);
            }
            assetListBox.SelectedIndex = 0;
        }

        private Dictionary<string, object> _lastParams = new Dictionary<string, object>();

        private AVMFunction currentMethod;

        private void LoadFunction(string key)
        {
            if (abi.functions.ContainsKey(key))
            {
                currentMethod = abi.functions[key];

                inputGrid.Rows.Clear();

                if (currentMethod.inputs != null)
                {
                    foreach (var p in currentMethod.inputs)
                    {
                        var param_key = (currentMethod.name + "_" + p.name).ToLower();
                        object val = "";

                        if (_lastParams.ContainsKey(param_key))
                        {
                            val = _lastParams[param_key];
                        }

                        inputGrid.Rows.Add(new object[] { p.name, val});

                        int rowIndex = inputGrid.Rows.Count - 1;

                        if (!_lastParams.ContainsKey(param_key))
                        {
                            EnablePlaceholderText(rowIndex, 1, p);
                        }                        
                    }
                }

                lastParams = key;

                button1.Enabled = true;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = paramsList.Text;
            LoadFunction(key);
        }

        private bool InitInvoke()
        {

            var key = paramsList.Text;
            var f = abi.functions[key];

            var argList = "";

            if (f.inputs != null)
            {
                int index = 0;
                foreach (var p in f.inputs)
                {
                    var temp = ($"{key}_{f.name}").ToLower();
                    var name = inputGrid.Rows[index].Cells[0].Value;

                    object val;
                    
                    // detect placeholder
                    if (inputGrid.Rows[index].Cells[1].Style.ForeColor == Color.Gray)
                    {
                        val = ""; 
                    }
                    else
                    {
                        val = ReadCellVal(index, 1);
                    }

                    if (val == null)
                    {
                        val = ""; // temporary hack, necessary to avoid VM crash
                    }

                    if (val != null && !val.Equals(""))
                    {
                        var param_key = (f.name + "_" + p.name).ToLower();
                        _lastParams[param_key] = val;
                    }

                    if (index>0)
                    {
                        argList += ",";
                    }

                    if (p.type.Contains("Array"))
                    {
                        var s = val.ToString();
                        if (!s.StartsWith("[") || !s.EndsWith("]"))
                        {
                            MessageBox.Show($"Invalid array format for argument #{index}");
                            return false;
                        }
                    }
                    else
                    switch (p.type.ToLower())
                    {
                        case "string": val = $"\"{val}\""; break;

                        case "integer":
                            {
                                BigInteger n;
                                if (!BigInteger.TryParse(val.ToString(), out n))
                                {
                                    MessageBox.Show($"Invalid array format for argument #{index}");
                                    return false;
                                }
                                break;
                            }

                        case "boolean":
                            {
                                switch (val.ToString().ToLower())
                                {
                                    case "true": val = true; break;
                                    case "false": val = false; break;
                                    default:
                                        {
                                            MessageBox.Show($"Invalid array format for argument #{index}");
                                            return false;
                                        }

                                }
                                break;
                            }
                    }

                    argList += val;
                    index++;
                }
            }

            if (key != abi.entryPoint.name)
            {
                if (f.inputs == null || f.inputs.Length == 0)
                {
                    argList = "[null]";
                }
                var operation = Char.ToLowerInvariant(key[0]) + key.Substring(1);
                argList = $"\"{operation}\", {argList}";
            }

            string json = "{\"params\": ["+ argList + "]}";

            if (string.IsNullOrEmpty(json))
            {
                MessageBox.Show("Invalid input!");
                return false;
            }

            DataNode node;

            try
            {
                node = JSONReader.ReadFromString(json);
            }
            catch
            {
                MessageBox.Show("Error parsing input!");
                return false;
            }

            var items = node.GetNode("params");

            if (assetListBox.SelectedIndex > 0)
            {
                foreach (var entry in Asset.Entries)
                {
                    if (entry.name == assetListBox.SelectedItem.ToString())
                    {
                        BigInteger ammount;

                        BigInteger.TryParse(assetAmmount.Text, out ammount);

                        if (ammount > 0)
                        {
                            emulator.SetTransaction(entry.id, ammount);
                        }
                        else
                        {
                            MessageBox.Show(entry.name + " ammount must be greater than zero");
                            return false;
                        }

                        break;
                    }
                }
            }

            emulator.Reset(items);

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (InitInvoke())
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void RunForm_Shown(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;

            inputGrid.AllowUserToAddRows = false;

            assetAmmount.Enabled = assetListBox.SelectedIndex > 0;

            if (Runtime.invokerKeys == null && File.Exists("last.key"))
            {
                var privKey = File.ReadAllBytes("last.key");
                if (privKey.Length == 32)
                {
                    Runtime.invokerKeys = new NeoLux.KeyPair(privKey);
                }
            }

            if (Runtime.invokerKeys != null)
            {
                addressLabel.Text = Runtime.invokerKeys.address;
            }
            else
            {
                addressLabel.Text = "(No key loaded)";
            }

            paramsList.Items.Clear();

            foreach (var f in abi.functions.Values)
            {
                paramsList.Items.Add(f.name);
            }

            int mainItem = paramsList.FindString("Main");
            if (mainItem >= 0) paramsList.SetSelected(mainItem, true);

            /*if (_paramMap != null)
            {
                foreach (var entry in _paramMap)
                {
                    paramsList.Items.Add(entry.Key);

                    if (lastParams != null && entry.Key.Equals(lastParams))
                    {
                        paramsList.SelectedIndex = paramsList.Items.Count - 1;
                    }
                }

                if (paramsList.SelectedIndex<0 && paramsList.Items.Count > 0)
                {
                    paramsList.SelectedIndex = 0;
                }

            }*/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string input = "";
            if (InputUtils.ShowInputDialog("Enter private key", ref input) == DialogResult.OK)
            {
                var privKey = input.HexToByte();
                if (privKey.Length == 32)
                {
                    Runtime.invokerKeys = new NeoLux.KeyPair(privKey);
                    addressLabel.Text = Runtime.invokerKeys.address;

                    File.WriteAllBytes("last.key", privKey);
                }
                else
                {
                    MessageBox.Show("Invalid private key, length should be 32");
                }
            }
        }

        private void listBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            assetAmmount.Enabled = assetListBox.SelectedIndex > 0;
        }

        private void paramsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1_Click(null, null);
        }

        private string ReadCellVal(int row, int col)
        {
            var temp = inputGrid.Rows[row].Cells[col].Value;
            var val = temp != null ? temp.ToString() : null;
            return val;
        }

        private void EnablePlaceholderText(int row, int col, AVMInput p)
        {
            var s = p.type;

            if (p.type.Contains("Array"))
            {
                s += " (Eg: [1, 2, \"something\"]";
            }

            var curContent = ReadCellVal(row, col);

            if (curContent != s && !string.IsNullOrEmpty(curContent))
            {
                return;
            }

            inputGrid.Rows[row].Cells[col].Style.ForeColor = Color.Gray;
            inputGrid.Rows[row].Cells[col].Value = s;
        }

        private void DisablePlaceholderText(int row, int col)
        {
            inputGrid.Rows[row].Cells[col].Style.ForeColor = Color.Black;
            inputGrid.Rows[row].Cells[col].Value = "";
        }

        private void VerifyPlaceholderAt(int row, int col)
        {
            if (col == 1 && currentMethod != null && !editMode)
            {
                var val = ReadCellVal(row, col);

                if (string.IsNullOrEmpty(val))
                {
                    var p = currentMethod.inputs[row];
                    EnablePlaceholderText(row, col, p);
                }
            }

        }

        private void inputGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            if (!editMode)
            {
                VerifyPlaceholderAt(e.RowIndex, e.ColumnIndex);
            }
        }

        private void inputGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 1 && currentMethod != null)
            {
                var col = inputGrid.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.ForeColor;
                if (col == Color.Gray)
                {
                    DisablePlaceholderText(e.RowIndex, e.ColumnIndex);
                }
            }
        }

        private bool editMode = false;

        private void inputGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            editMode = true;
        }

        private void inputGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            editMode = false;
            VerifyPlaceholderAt(e.RowIndex, e.ColumnIndex);
        }
    }
}
