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
            if (e.KeyChar != (char)Keys.Enter) return;
            Form1.GetLeads();
        }
        
        private async Task<RandomUserApiResponse.RandomUserApi> GenerateRandomUser()
        {
            var httpClient = new HttpClient();
            var response = await httpClient.GetAsync("https://randomuser.me/api/");
            var randomUser = JsonSerializer.DeserializeAsync<RandomUserApiResponse.RandomUserApi>(await response.Content.ReadAsStreamAsync());
            return await randomUser ?? throw new InvalidOperationException();
        }
    }
}
