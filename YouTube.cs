﻿using System;
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
    public partial class YouTube : Form
    {
        private DataGridView? tableView = null;
        private readonly Random random = new Random();
        private readonly HttpClient client;


        public YouTube()
        {
            client = new HttpClient();
            InitializeComponent();
        }

        public YouTube(DataGridView dataGridView1)
        {
            this.tableView = dataGridView1;
            InitializeComponent();
        }

        private async Task<RandomUserApiResponse.RandomUserApi> GenerateRandomUser()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://randomuser.me/api/");
            var randomUser = JsonSerializer.DeserializeAsync<RandomUserApiResponse.RandomUserApi>(await response.Content.ReadAsStreamAsync());
            return await randomUser ?? throw new InvalidOperationException();
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
