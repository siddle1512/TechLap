using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TechLap.WPF.Chat
{
    public partial class ChatControl : UserControl
    {
        private HubConnection _connection;
        private string ApiUrl = ConfigurationManager.AppSettings["ApiEndpoint"];
        public ChatControl()
        {
            InitializeComponent();
            InitializeSignalR();
        }

        private async void InitializeSignalR()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{ApiUrl}/chatHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(GlobalState.Token);
                })
                .Build();

            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    DisplayMessage(user, message);
                });
            });

            await _connection.StartAsync();
        }

        private void DisplayMessage(string user, string message)
        {
            var messageBlock = new TextBlock
            {
                Text = $"{user}: {message}",
                Margin = new Thickness(5),
                Foreground = user == "User" ? Brushes.Green : Brushes.Blue 
            };

            ChatHistory.Children.Add(messageBlock);
        }


        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            var messageContent = MessageInput.Text.Trim();

            if (string.IsNullOrEmpty(messageContent) || messageContent == "Enter your message...")
            {
                return; 
            }

            var messageData = new
            {
                receiverId = 1,
                messageContent = messageContent
            };

            var json = JsonConvert.SerializeObject(messageData);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync($"{ApiUrl}/api/chat/send", content);

                if (response.IsSuccessStatusCode)
                {
                    await _connection.InvokeAsync("SendMessage", "User", messageContent);
                    MessageInput.Text = string.Empty;
                }
                else
                {
                    MessageBox.Show("Error sending message: " + response.ReasonPhrase);
                }
            }
        }


        private void MessageInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MessageInput.Text == "Enter your message...")
            {
                MessageInput.Text = string.Empty;
            }
        }
    }
}
