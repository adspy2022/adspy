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
    public partial class LinkedIn : Form
    {
        private DataGridView? tableView = null;
        private readonly Random random = new Random();
        private readonly HttpClient client;


        public LinkedIn()
        {
            client = new HttpClient();
            InitializeComponent();
        }

        public LinkedIn(DataGridView dataGridView1)
        {
            this.tableView = dataGridView1;
            InitializeComponent();
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
