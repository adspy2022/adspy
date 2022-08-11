using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Leadopogo.Simulator
{
    public partial class Login : Form
    {
        public static Form1? Form1Instance;
        public static HttpClient? HttpClient;
        public Login()
        {
            HttpClient = new HttpClient();
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            this.btnLogin.Enabled = false;
            this.btnLogin.Text = "Logging in...";
            Application.UseWaitCursor = true;
            
            var response = await HttpClient.PostAsync("https://adspy.block2play.com/api/auth/login", new StringContent(JsonSerializer.Serialize(new
            {
                Username = txtUsername.Text,
                Password = txtPassword.Text
            }), Encoding.UTF8, "application/json"));

            var responseContentDeserialized = JsonSerializer.Deserialize<JsonNode>(await response.Content.ReadAsStringAsync());
            
            if (response.IsSuccessStatusCode)
            {
               
                var token = responseContentDeserialized?["token"]?.GetValue<string>();
                if (token == null)
                {
                    MessageBox.Show("Internal Error while logging in. Please try again later. If the problem persists, contact the administrator.");
                    Environment.Exit(1);
                }

                Application.UseWaitCursor = false;
                
                Form1Instance?.FinishLogin(token);
                this.Close();
            }
            else
            {
                MessageBox.Show(responseContentDeserialized?["message"]?.GetValue<string>() ?? "Internal Error while logging in. Please try again later. If the problem persists, contact the administrator.");
                this.btnLogin.Enabled = true;
                this.btnLogin.Text = "Login";
                Application.UseWaitCursor = false;
            }
        }
    }
}
