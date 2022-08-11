using System.Text.Json;
using Leadopogo.Simulator.Models;
using Microsoft.Extensions.Configuration;

namespace Leadopogo.Simulator
{
    public partial class Facebook : Form
    {
        private DataGridView? tableView = null;
        private Random random = new Random();

        public Facebook()
        {
            InitializeComponent();
        }

        public Facebook(DataGridView tableView)
        {
            InitializeComponent();
            this.tableView = tableView;
        }

        private async void txtKeyword_KeyPress(object? sender, KeyPressEventArgs e)
        {
            
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Length < 15)
            {
                MessageBox.Show("Invalid ad id!");
                return;
            }
            Form1.GetLeads();
        }
    }
}
