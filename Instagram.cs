using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Forms;
using Leadopogo.Simulator.Models;
using Microsoft.Extensions.Configuration;

namespace Leadopogo.Simulator
{
    public partial class Instagram : Form
    {
        private DataGridView? tableView = null;
        private readonly Random random = new Random();
        private readonly HttpClient client;

        public Instagram()
        {
            client = new HttpClient();
            InitializeComponent();
        }

        public Instagram(DataGridView tableView)
        {
            InitializeComponent();
            this.tableView = tableView;
        }

        private async void txtKeyword_KeyPress(object? sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != (char)Keys.Enter) return;
            if (textBox1.Text.Length < 15)
            {
                MessageBox.Show("Invalid ad id!");
                return;
            }
            Form1.GetLeads();
        }
    }
}
