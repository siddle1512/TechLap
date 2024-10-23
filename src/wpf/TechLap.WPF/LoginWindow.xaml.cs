using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Windows;

namespace TechLap.WPF
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Hiển thị progress bar
                progressBar.Visibility = Visibility.Visible;

                var username = txtUsername.Text;
                var password = txtPassword.Password;

                // Create HttpClient 
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                var content = JsonConvert.SerializeObject(new { Email = username, Password = password });
                StringContent data = new StringContent(content, Encoding.UTF8, "application/json");

                // Send request to API
                HttpResponseMessage response = await client.PostAsync(ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/user/login", data);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var jsonResponse = JObject.Parse(responseBody);

                if (jsonResponse["isSuccess"] != null)
                {
                    if (jsonResponse["isSuccess"].Value<bool>())
                    {
                        //MessageBox.Show("Login successfully");
                        GlobalState.Token = jsonResponse["data"].Value<string>();
                        Dashboard adminDashboard = new Dashboard();
                        adminDashboard.Show();
                        Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                // Ẩn progress bar sau khi xử lý xong
                progressBar.Visibility = Visibility.Collapsed;
            }
        }

    }
}
