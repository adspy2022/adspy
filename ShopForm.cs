using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Leadopogo.Simulator
{
    public partial class ShopForm : Form
    {
        public ShopForm()
        {
            InitializeComponent();
        }

        private void txtAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAmount_TextChanged(object sender, EventArgs e)
        {
            if (!int.TryParse(txtAmount.Text, out _)) return;
            var price = int.Parse(txtAmount.Text) * 100;
            txtCost.Text = $"${price:0.00}";
        }

        private async void btnBuy_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtAmount.Text, out _) || int.Parse(txtAmount.Text) < 1 || int.Parse(txtAmount.Text) > 1000) return;
            btnBuy.Enabled = false;
            Form1.Form1Static.UseWaitCursor = true;
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync($"https://adspy.block2play.com/api/payment/create?amount={txtAmount.Text.Trim()}&token={Form1.AuthToken}", new StringContent(string.Empty));
            var responseContentDeserialized = JsonSerializer.Deserialize<JsonNode>(await response.Content.ReadAsStringAsync());
            if (responseContentDeserialized == null) return;
            var psi = new ProcessStartInfo
            {
                FileName = responseContentDeserialized["url"].ToString() ?? throw new InvalidOperationException("No URL found. Please try again later. If the problem persists, contact support."),
                UseShellExecute = true
            };
            Process.Start(psi);
            btnBuy.Enabled = true;
            Form1.Form1Static.UseWaitCursor = false;
        }
    }
}
