using Neo.Emulator;
using Neo.Emulator.API;
using System;
using System.Windows.Forms;

namespace Neo.Debugger.Forms
{
    public partial class BlockchainForm : Form
    {
        private Blockchain _blockchain;
        public BlockchainForm(Blockchain blockchain)
        {
            InitializeComponent();
            _blockchain = blockchain;

            dataGridView1.Columns.Add("Key", "Key");
            dataGridView1.Columns.Add("Value", "Value");

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.RowHeadersVisible = false;
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[0].FillWeight = 3;

            dataGridView1.Columns[1].ReadOnly = true;
            dataGridView1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns[1].FillWeight = 4;

            dataGridView2.Columns.Add("Address", "Address");
            dataGridView2.Columns.Add("Private Key", "Private Key");
            dataGridView2.Columns.Add("Assets", "Assets");

            dataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView2.RowHeadersVisible = false;

            for (int i=0; i<=2; i++)
            {
                dataGridView2.Columns[i].ReadOnly = true;
                dataGridView2.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dataGridView2.Columns[i].FillWeight = 2;
            }
            dataGridView2.Columns[1].FillWeight = 3;
        }

        private void BlockchainForm_Shown(object sender, System.EventArgs e)
        {
            var blockchain = _blockchain;

            foreach (var block in blockchain.blocks.Values)
            {
                foreach (var tx in block.transactions)
                {
                    listBox1.Items.Add(tx.hash.ToHexString());
                }
            }

            panel1.Visible = false;

            dataGridView2.Rows.Clear();
            foreach (var address in blockchain.addresses)
            {
                string assets = "";

                foreach (var balance in address.balances)
                {
                    if (assets.Length > 0) assets += ", ";

                    assets += balance.Value + " " + balance.Key;
                }

                if (assets.Length == 0)
                {
                    assets = "None";
                }

                dataGridView2.Rows.Add(address.keys.address, address.keys.PrivateKey.ToHexString(), assets);
            }
        }

        private Transaction FindByHash(string hash)
        {
            var blockchain = _blockchain;
            foreach (var block in blockchain.blocks.Values)
            {
                foreach (var tx in block.transactions)
                {
                    if (tx.hash.ToHexString() == hash)
                    {
                        return tx;
                    }
                }
            }

            return null;
        }

        private void listBox1_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            Transaction tx = FindByHash(listBox1.SelectedItem.ToString());
            if (tx == null)
            {
                panel1.Visible = false;
                return;
            }

            panel1.Visible = true;

            dataGridView1.Rows.Clear();

            dataGridView1.Rows.Add("Hash", tx.hash.ToHexString());
            dataGridView1.Rows.Add("Block", tx.block.height);

            int index;

            index = 0;
            foreach (var input in tx.inputs)
            {
                dataGridView1.Rows.Add("Input #" + index, input.prevHash.ToHexString());
                index++;
            }

            index = 0;
            foreach (var output in tx.outputs)
            {
                dataGridView1.Rows.Add("Output #" + index, output.hash.ToString());
                index++;
            }
        }
    }
}
