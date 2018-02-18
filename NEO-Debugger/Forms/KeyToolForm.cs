using Neo.Cryptography;
using NeoLux;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                output += $"0x{item.ToString("X2")}, ";
            }
            return output;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            KeyPair keyPair;

            var keyBytes = keyBox.Text.HexToBytes();
            if (keyBox.Text.Length == 52)
            {
                keyPair = KeyPair.FromWIF(keyBox.Text);
            }
            else
            if (keyBytes.Length == 32)
            {
                keyPair = new KeyPair(keyBytes);
            }
            else
            {
                MessageBox.Show("Invalid key input, must be 52 or 64 hexdecimal characters.");
                return;
            }

            keyDataGrid.Rows.Clear();

            keyDataGrid.Rows.Add(new object[] { "Address", keyPair.address });
            keyDataGrid.Rows.Add(new object[] { "Public Key (RAW, hex)", keyPair.PublicKey.ToHexString() });
            keyDataGrid.Rows.Add(new object[] { "Private Key (RAW, hex)", keyPair.PrivateKey.ToHexString() });
            keyDataGrid.Rows.Add(new object[] { "Private Key (WIF, hex)", keyPair.WIF });
            keyDataGrid.Rows.Add(new object[] { "Private Key (RAW, bytes)", ToByteArray(keyPair.PrivateKey) });
        }
    }
}
