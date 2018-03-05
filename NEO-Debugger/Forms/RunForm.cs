using LunarParser;
using LunarParser.JSON;
using Neo.Cryptography;
using Neo.Debugger.Models;
using Neo.Debugger.Utils;
using Neo.Emulator;
using Neo.Emulator.API;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace Neo.Debugger.Forms
{
    public partial class RunForm : Form
    {
        private ABI _abi;
        private TestSuite _testSuite;
        private string currentContractName = "";
        private bool editMode = false;
        private int editRow;

        public AVMFunction currentMethod { get; private set; }

        private DebugParameters _debugParameters;
        public DebugParameters DebugParameters
        {
            get
            {
                return _debugParameters;
            }
        }

        private string _defaultPrivateKey;
        private Dictionary<string, string> _defaultParams;

        public RunForm(ABI abi, TestSuite tests, string contractName, string defaultPrivateKey, Dictionary<string,string> defaultParams, string defaultFunction)
        {
            InitializeComponent();
            _testSuite = tests;
            _abi = abi;
            currentContractName = contractName;

            //Defaults.
            if (defaultPrivateKey != null)
                _defaultPrivateKey = defaultPrivateKey;
            if (defaultParams != null)
                _defaultParams = defaultParams;

            assetComboBox.Items.Clear();
            assetComboBox.Items.Add("None");
            foreach (var entry in Asset.Entries)
            {
                assetComboBox.Items.Add(entry.name);
            }
            assetComboBox.SelectedIndex = 0;

            triggerComboBox.SelectedIndex = 0;
            witnessComboBox.SelectedIndex = 0;

            paramsList.Items.Clear();

            foreach (var f in _abi.functions.Values)
            {
                paramsList.Items.Add(f.name);
            }

            if (string.IsNullOrEmpty(defaultFunction))
            {
                defaultFunction = _abi.entryPoint.name;
            }
            
            int mainItem = paramsList.FindString(defaultFunction);
            if (mainItem >= 0) paramsList.SetSelected(mainItem, true);
            
            testCasesList.Items.Clear();
            foreach (var entry in _testSuite.cases.Keys)
            {
                testCasesList.Items.Add(entry);
            }
        }

        private bool InitInvoke()
        {
            var key = paramsList.Text;
            var f = _abi.functions[key];

            _debugParameters = new DebugParameters();

            //Get the private key used
            _debugParameters.PrivateKey = privateKeyInput.Text;

            //Get the witness mode
            CheckWitnessMode witnessMode;
            var ws = witnessComboBox.SelectedItem.ToString().Replace(" ", "");

            if (!Enum.TryParse<CheckWitnessMode>(ws, out witnessMode))
            {
                return false;
            }
            _debugParameters.WitnessMode = witnessMode;

            //Get the trigger type
            TriggerType type;
            var ts = triggerComboBox.SelectedItem.ToString().Replace(" ", "");

            if (!Enum.TryParse<TriggerType>(ts, out type))
            {
                return false;
            }
            _debugParameters.TriggerType = type;
            
            //Get the arguments list
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
                        var param_key = (currentContractName + "_" + f.name + "_" + p.name).ToLower();
                        //Add our default running parameters for next time
                        _debugParameters.DefaultParams[param_key] = val.ToString();
                    }

                    if (index > 0)
                    {
                        argList += ",";
                    }

                    if (p.type.Contains("Array"))
                    {
                        var s = val.ToString();

                        if (s.StartsWith("[") && s.EndsWith("]"))
                        {
                            val = s;
                        }
                        else
                        if (s.StartsWith("\"") && s.EndsWith("\""))
                        {
                            s = s.Substring(1, s.Length - 2);
                            if (Util.IsHex(s))
                            {
                                var bytes = s.HexToBytes();
                                s = Util.BytesToString(bytes);
                            }
                            else if (Util.IsValidWallet(s))
                            {
                                var scriptHash = Emulator.Helper.AddressToScriptHash(s);
                                s = Util.BytesToString(scriptHash);
                            }
                            else
                            {
                                ShowArgumentError(f, index, val);
                                return false;
                            }

                            val = $"[{s}]";
                        }
                        else
                        {
                            ShowArgumentError(f, index, val);
                            return false;
                        }
                    }
                    else
                        switch (p.type.ToLower())
                        {
                            case "string":
                                {
                                    var s = val.ToString();
                                    if (!s.StartsWith("\"") || !s.EndsWith("\""))
                                    {
                                        ShowArgumentError(f, index, val);
                                        return false;
                                    }

                                    break;
                                }

                            case "integer":
                                {
                                    BigInteger n;
                                    var s = val.ToString();
                                    if (string.IsNullOrEmpty(s) || !BigInteger.TryParse(s, out n))
                                    {
                                        ShowArgumentError(f, index, val);
                                        ResetTabs();
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
                                                ShowArgumentError(f, index, val);
                                                ResetTabs();
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
            if (key != _abi.entryPoint.name)
            {
                if (f.inputs == null || f.inputs.Length == 0)
                {
                    argList = "[null]";
                }
                var operation = Char.ToLowerInvariant(key[0]) + key.Substring(1);
                argList = $"\"{operation}\", {argList}";
            }

            //Set the arguments list
            try
            {
                _debugParameters.ArgList = Util.GetArgsListAsNode(argList);
            }
            catch
            {
                MessageBox.Show("Error parsing input!");
                ResetTabs();
                return false;
            }
            
            if (assetComboBox.SelectedIndex > 0)
            {
                foreach (var entry in Asset.Entries)
                {
                    if (entry.name == assetComboBox.SelectedItem.ToString())
                    {
                        BigInteger amount;
                        BigInteger.TryParse(assetAmount.Text, out amount);
                        if (amount > 0)
                        {
                            amount *= 100000000; // fix decimals

                            //Add the transaction info
                            _debugParameters.Transaction.Add(entry.id, amount);
                        }
                        else
                        {
                            MessageBox.Show(entry.name + " amount must be greater than zero");
                            return false;
                        }

                        break;
                    }
                }
            }

            uint timestamp;
            if (!uint.TryParse(timestampBox.Text, out timestamp))
            {
                MessageBox.Show("Invalid timestamp");
                return false;
            }
            else
            {
                _debugParameters.Timestamp = timestamp;
            }

            return true;
        }

        #region Main Form

        private void RunForm_Shown(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.None;

            inputGrid.AllowUserToAddRows = false;

            assetAmount.Enabled = assetComboBox.SelectedIndex > 0;
            timestampBox.Text = Emulator.Helper.ToTimestamp(DateTime.UtcNow).ToString();

            if (Runtime.invokerKeys == null && File.Exists("last.key"))
            {
                var privKey = File.ReadAllBytes("last.key");
                if (privKey.Length == 32)
                {
                    Runtime.invokerKeys = new KeyPair(privKey);
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

            privateKeyInput.Text = _defaultPrivateKey;
        }

        private void ResetTabs()
        {
            this.runTabs.SelectedTab = methodTab;
        }

        #endregion

        #region Input Lists Handlers

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = paramsList.Text;
            LoadFunction(key);
        }

        private void paramsList_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            button1_Click(null, null);
        }

        private void testCasesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            var key = testCasesList.Text;
            var testCase = _testSuite.cases[key];
            var methodName = testCase.method != null ? testCase.method : _abi.entryPoint.name;

            for (int i = 0; i < paramsList.Items.Count; i++)
            {
                if (paramsList.Items[i].ToString() == methodName)
                {
                    paramsList.SelectedIndex = i;

                    for (int j = 0; j < inputGrid.RowCount; j++)
                    {
                        string val;

                        if (testCase.args != null && j < testCase.args.ChildCount)
                        {
                            var node = testCase.args[j];
                            val = Util.ParseNode(node, j);
                        }
                        else
                        {
                            val = "";
                        }

                        inputGrid.Rows[j].Cells[1].Value = val;
                        inputGrid.Rows[j].Cells[1].Style.ForeColor = Color.Black;
                    }

                    break;
                }
            }
        }

        private void inputGrid_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (editMode)
            {
                editMode = false;
                VerifyPlaceholderAt(editRow, 1);
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

        private void inputGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            editMode = true;
            editRow = e.RowIndex;
        }

        private void inputGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            editMode = false;
            VerifyPlaceholderAt(e.RowIndex, e.ColumnIndex);
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
                else
                {
                    inputGrid.Rows[row].Cells[col].Style.ForeColor = Color.Black;
                }
            }

        }

        private void LoadFunction(string key)
        {
            if (_abi.functions.ContainsKey(key))
            {
                currentMethod = _abi.functions[key];

                inputGrid.Rows.Clear();

                if (currentMethod.inputs != null)
                {
                    foreach (var p in currentMethod.inputs)
                    {
                        var param_key = (currentContractName + "_" + currentMethod.name + "_" + p.name).ToLower();
                        object val = "";

                        bool isEmpty = true;

                        if (_defaultParams.ContainsKey(param_key))
                        {
                            val = _defaultParams[param_key];
                            isEmpty = false;
                        }

                        inputGrid.Rows.Add(new object[] { p.name, val });

                        int rowIndex = inputGrid.Rows.Count - 1;

                        if (isEmpty)
                        {
                            EnablePlaceholderText(rowIndex, 1, p);
                        }
                    }
                }

                button1.Enabled = true;
            }
        }

        #endregion

        #region Button Click Handlers

        private void button1_Click(object sender, EventArgs e)
        {
            if (InitInvoke())
            {
                this.DialogResult = DialogResult.OK;
            }
        }

        private void button3_Click_1(object sender, EventArgs e)
        {
            var keyPair = Util.GetKeyFromString(privateKeyInput.Text);
            if (keyPair != null)
            {
                Runtime.invokerKeys = keyPair;
                addressLabel.Text = Runtime.invokerKeys.address;
            }
            else
            {
                MessageBox.Show("Invalid private key, length should be 52 or 64");
            }
        }

        #endregion

        #region Helpers

        private void ShowArgumentError(AVMFunction f, int index, object val)
        {
            string error;

            if (val == null || string.IsNullOrEmpty(val.ToString()))
            {
                error = "Missing";
            }
            else
            {
                error = "Invalid format in ";
            }

            MessageBox.Show($"{error} argument #{index + 1} (\"{f.inputs[index].name}\") of {f.name} method");
            ResetTabs();
        }

        #endregion

        private bool lockDate;

        private void timestampBox_TextChanged(object sender, EventArgs e)
        {
            if (lockDate) return;

            uint timestamp;
            if (uint.TryParse(timestampBox.Text, out timestamp))
            {
                dateTimePicker1.Value = Emulator.Helper.ToDateTime(timestamp);
            }
        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {
            lockDate = true;
            timestampBox.Text = Emulator.Helper.ToTimestamp(dateTimePicker1.Value).ToString();
            lockDate = false;
        }

        private void assetComboBox_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            assetAmount.Enabled = assetComboBox.SelectedIndex > 0;
        }
    }
}
