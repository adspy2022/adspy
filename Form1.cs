using System.Configuration;
using System.Diagnostics;
using System.Text.Json;
using System.Timers;
using Leadopogo.Simulator.Models;
using Microsoft.Extensions.Configuration;

namespace Leadopogo.Simulator
{
    public partial class Form1 : Form
    {
        private Form? _activeForm = null;
        public static int ConfigSelection = 0;
        public static bool Stop = false;
        public static string? AuthToken = null;
        public static DataGridView? TableView;
        public static Form1 Form1Static;

        public Form1()
        {
            this.KeyPreview = true;
            InitializeComponent();
            OpenChildForm(new Login()); 
            Login.Form1Instance = this;
            TableView = dataGridView1;
            Form1Static = this;
        }

        private void OpenChildForm(Form childForm)
        {
            _activeForm?.Close();
            _activeForm = childForm;
            childForm.TopLevel = false;
            childForm.FormBorderStyle = FormBorderStyle.None;
            childForm.Dock = DockStyle.Fill;
            childForm.BackColor = panelChildForm.BackColor;
            panelChildForm.Controls.Add(childForm);
            panelChildForm.Tag = childForm;
            childForm.BringToFront();
            childForm.Show();
        }

        private void btnInstagram_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Facebook(this.dataGridView1));
        }

        private void btnFacebook_Click(object sender, EventArgs e)
        {
            OpenChildForm(new Instagram(this.dataGridView1));
        }
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            // if (AuthToken == null) return;
            // switch (e.KeyCode)
            // {
            //     case Keys.D1:
            //         Console.Beep(1000, 100);
            //         ConfigSelection = 0;
            //         break;
            //     case Keys.D2:
            //         ConfigSelection = 1;
            //         Console.Beep(2000, 100);
            //         break;
            //     case Keys.D3:
            //         ConfigSelection = 2;
            //         Console.Beep(3000, 100);
            //         break;
            //     case Keys.D4:
            //         ConfigSelection = 3;
            //         Console.Beep(4000, 100);
            //         break;
            // }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenChildForm(new YouTube(this.dataGridView1));
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenChildForm(new LinkedIn(this.dataGridView1));
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Stop = true;
            dataGridView1.Rows.Clear();
            Form1Static.pgsCurrent.Value = 0;
            Form1Static.pgsTotal.Value = 0;
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Successfully exported results to file", "Info", MessageBoxButtons.OK);
        }

        private void btn_logInOut_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            btnReset.Visible = false;
            btnExport.Visible = false;
            dataGridView1.Visible = false;
            AuthToken = null;
            OpenChildForm(new Login());
        }
        
        public void FinishLogin(string token)
        {
            AuthToken = token;
            panel1.Visible = true;
            btnReset.Visible = true;
            btnExport.Visible = true;
            dataGridView1.Visible = true;
            panelFooter.Visible = true;
        }

        public static async void GetLeads()
        {
            MessageBox.Show("Started capturing ads. This may take a while. Please be patient.", "Info", MessageBoxButtons.OK);
            var random = new Random();
            Stop = false;
            if (Login.HttpClient == null) return;
            var response = await Login.HttpClient.GetAsync("https://adspy.block2play.com/api/leads/get?token=" + Form1.AuthToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                MessageBox.Show(await response.Content.ReadAsStringAsync());
                return;
            }
            var content = await response.Content.ReadAsStringAsync();
            var accounts = JsonSerializer.Deserialize<List<Account>>(content);
            
            if (accounts == null || accounts.Count == 0)
            {
                MessageBox.Show("No leads found");
                Environment.Exit(0);
            }
            
            Form1Static.pgsTotal.Maximum = accounts.Count;
            TableView?.Rows.Clear();
            var usedAccounts = 0;
            foreach (var t in accounts)
            {
                if (Stop) break;
                var delay = random.Next(60000, 90000);
                
                Form1Static.pgsCurrent.Minimum = 0;
                Form1Static.pgsCurrent.Maximum = delay;
                
                var timer = new System.Timers.Timer();
                timer.Interval = 3000;
                timer.Start();
                timer.Elapsed += TimerOnElapsed;
                
                await Task.Delay(delay);
                if (accounts == null || usedAccounts >= accounts.Count) continue;
                TableView?.Rows.Add(accounts[usedAccounts].Id, accounts[usedAccounts].Username, accounts[usedAccounts].FullName, accounts[usedAccounts].Gender, accounts[usedAccounts].Phone, accounts[usedAccounts].Email, accounts[usedAccounts].Business);
                usedAccounts++;
                Form1Static.pgsCurrent.Value = 0;
                Form1Static.pgsTotal.Value++;
                timer.Stop();
            }
            MessageBox.Show("Done extracting ads!");
        }

        private static void TimerOnElapsed(object? sender, ElapsedEventArgs e)
        {
            Form1Static.pgsCurrent.Value += 3000;
        }
    }
}