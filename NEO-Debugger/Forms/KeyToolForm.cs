using Neo.Cryptography;
using System;
using System.Windows.Forms;

namespace Neo.Debugger.Forms
{
    public partial class KeyToolForm : Form
    {
        public KeyToolForm()
        {
            InitializeComponent();

            keyDataGrid.Columns.Add("Property", "Property");
            keyDataGrid.Columns.Add("Value", "Value");

            keyDataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            keyDataGrid.RowHeadersVisible = false;
            keyDataGrid.Columns[0].ReadOnly = true;
            keyDataGrid.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            keyDataGrid.Columns[0].FillWeight = 2;

            keyDataGrid.Columns[1].ReadOnly = true;
            keyDataGrid.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            keyDataGrid.Columns[1].FillWeight = 4;


        }

        private void KeyToolForm_Shown(object sender, EventArgs e)
        {
        }

        public static string reverseHex(string hex)
        {

            string result = "";
            for (var i = hex.Length - 2; i >= 0; i -= 2)
            {
                result += hex.Substring(i, 2);
            }
            return result;
        }

        private string ToByteArray(byte[] bytes)
        {
            var output = "";
            foreach (var item in bytes)
            {
                if (output.Length > 0) output += ",";
                output += $"{item.ToString().PadLeft(3)}";
            }
            output = $"[{output}]";
            return output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KeyPair keyPair;
            
            if (keyBox.Text.Length == 52)
            {
                keyPair = KeyPair.FromWIF(keyBox.Text);
            }
            else
            if (keyBox.Text.Length == 64)
            {
                var keyBytes = keyBox.Text.HexToBytes();
                keyPair = new KeyPair(keyBytes);
            }
            else
            {
                MessageBox.Show("Invalid key input, must be 52 or 64 hexdecimal characters.");
                return;
            }

            keyDataGrid.Rows.Clear();

            var scriptHash = Emulator.Helper.AddressToScriptHash(keyPair.address);

            keyDataGrid.Rows.Add(new object[] { "Address", keyPair.address });
            keyDataGrid.Rows.Add(new object[] { "Script Hash (RAW, hex) ", scriptHash.ToHexString() });
            keyDataGrid.Rows.Add(new object[] { "Script Hash (RAW, bytes) ", ToByteArray(scriptHash) });
            keyDataGrid.Rows.Add(new object[] { "Public Key (RAW, hex)", keyPair.PublicKey.ToHexString() });
            keyDataGrid.Rows.Add(new object[] { "Private Key (RAW, hex)", keyPair.PrivateKey.ToHexString() });
            keyDataGrid.Rows.Add(new object[] { "Private Key (WIF, hex)", keyPair.WIF });
            keyDataGrid.Rows.Add(new object[] { "Private Key (RAW, bytes)", ToByteArray(keyPair.PrivateKey) });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var bytes = new byte[32];
            var rnd = new Random();
            rnd.NextBytes(bytes);
            keyBox.Text = bytes.ToHexString();
        }
    }
}
