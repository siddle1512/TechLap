﻿using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Net.Http;
using System.Text;
using System.Windows;
using TechLap.WPFAdmin.Components;

namespace TechLap.WPFAdmin
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
                var content = JsonConvert.SerializeObject(new { Username = username, Password = password });
                StringContent data = new StringContent(content, Encoding.UTF8, "application/json");

                // Send request to API
                HttpResponseMessage response = await client.PostAsync(ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/admins/login", data);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response
                var jsonResponse = JObject.Parse(responseBody);

                if (jsonResponse["isSuccess"] != null && jsonResponse["isSuccess"].Value<bool>())
                {
                    GlobalState.Token = jsonResponse["data"].Value<string>();
                    AdminDashboard adminDashboard = new AdminDashboard();
                    adminDashboard.Show();
                    Close();
                }
                else
                {
                    errorMessageControl.ShowError("Login failed! Incorrect username or password.");
                }
            }
            catch (Exception)
            {
                errorMessageControl.ShowError("Login failed! Please try again.");
            }
            finally
            {
                // Ẩn progress bar sau khi xử lý xong
                progressBar.Visibility = Visibility.Collapsed;
            }
        }

    }
}
