using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace TechLap.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            try { 
                var username = txtUsername.Text;
                var password = txtPassword.Text;

                // Create HttpClient 
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = JsonConvert.SerializeObject(new { Username = username, Password = password });
                StringContent data = new StringContent(content, Encoding.UTF8, "application/json");

                // Send request to API
                HttpResponseMessage response = await client.PostAsync(ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/admins/login", data);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var jsonResponse = JObject.Parse(responseBody);

                if (jsonResponse["isSuccess"] != null)
                {
                    if (jsonResponse["isSuccess"].Value<bool>())
                    {
                        MessageBox.Show("Login successfully");
                        GlobalState.Token = jsonResponse["data"].Value<string>();
                    }
                }
            } catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
    }
}