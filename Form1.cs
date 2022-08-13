using System.Configuration;
using System.Diagnostics;
using System.Text;
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
        private IList<Account> Leads = new List<Account>();

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
            MessageBox.Show("Please select where to save the file", "Info", MessageBoxButtons.OK);
            var allLines = from lead in Leads
                select new object[]
                {
                    lead.Id.ToString(),
                    lead.Username.ToString(),
                    lead.FullName.ToString(),
                    lead.Gender.ToString(),
                    lead.Phone.ToString(),
                    lead.Email.ToString(),
                    lead.Business.ToString()
                };
            var csv = new StringBuilder();
            // add header to csv
            csv.AppendLine("Id,Username,FullName,Gender,Phone,Email,Business");
            foreach (var allLine in allLines)
            {
                csv.AppendLine(string.Join(",", allLine));
            }
            var sfd = new SaveFileDialog
            {
                Filter = "csv files (*.csv)|*.csv|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(sfd.FileName, csv.ToString());
            }
            MessageBox.Show("File saved successfully", "Info", MessageBoxButtons.OK);
        }

        private void btn_logInOut_Click(object sender, EventArgs e)
        {
            panel1.Visible = false;
            btnReset.Visible = false;
            btnExport.Visible = false;
            dataGridView1.Visible = false;
            panelFooter.Visible = false;
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
            Form1Static.Leads = JsonSerializer.Deserialize<List<Account>>(content);
            
            if (Form1Static.Leads == null || Form1Static.Leads.Count == 0)
            {
                MessageBox.Show("No leads found");
                Environment.Exit(0);
            }
            
            Form1Static.pgsTotal.Maximum = Form1Static.Leads.Count;
            TableView?.Rows.Clear();
            var usedAccounts = 0;
            foreach (var t in Form1Static.Leads)
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
                if (Form1Static.Leads == null || usedAccounts >= Form1Static.Leads.Count) continue;
                TableView?.Rows.Add(Form1Static.Leads[usedAccounts].Id, Form1Static.Leads[usedAccounts].Username, Form1Static.Leads[usedAccounts].FullName, Form1Static.Leads[usedAccounts].Gender, Form1Static.Leads[usedAccounts].Phone, Form1Static.Leads[usedAccounts].Email, Form1Static.Leads[usedAccounts].Business);
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

        private void btnCredits_Click(object sender, EventArgs e)
        {
            OpenChildForm(new ShopForm());
        }
    }
}