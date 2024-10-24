using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TechLap.WPFAdmin.ChatNew
{
    public partial class ChatNewControl : UserControl
    {
        private HubConnection _connection;
        private int _selectedUserId;
        public ChatNewControl()
        {
            InitializeComponent();
            LoadUsers(); 
            InitializeSignalR();
        }
        private async void LoadUsers()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GlobalState.Token}");

            HttpResponseMessage response = await client.GetAsync("https://localhost:7097/api/users");
            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var users = JObject.Parse(responseBody)["data"];

                foreach (var user in users)
                {
                    var userName = user["fullName"].Value<string>();
                    var userId = user["id"].Value<int>();
                    UserList.Items.Add(new { Name = userName, Id = userId });
                }

                UserList.DisplayMemberPath = "Name"; 
                UserList.SelectedValuePath = "Id"; 
            }
            else
            {
                MessageBox.Show("Error loading users: " + response.ReasonPhrase);
            }
        }

        private async void InitializeSignalR()
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7170/chatHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult(GlobalState.Token);
                })
                .Build();

            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    if (user != "Admin")
                    {
                        DisplayMessage(user, message);
                    }
                    else
                    {
                        DisplayMessage("Admin", message);
                    }
                });
            });

            await _connection.StartAsync();
        }

        private async void LoadChatHistory()
        {
            if (_selectedUserId == 0) return;
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GlobalState.Token}");

                HttpResponseMessage response = await client.GetAsync($"https://localhost:7097/api/chat/history?userId={_selectedUserId}&adminId=1");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var chatHistoryObject = JObject.Parse(responseBody);

                    if (chatHistoryObject["data"] != null)
                    {
                        var chatMessages = chatHistoryObject["data"] as JArray;
                        ChatHistory.Children.Clear(); 

                        foreach (var message in chatMessages)
                        {
                            string sender = message["isFromAdmin"].Value<bool>() ? "Admin" : "User";
                            string content = message["messageContent"].Value<string>();
                            DisplayMessage(sender, content);
                        }
                    }
                }
                else
                {
                    MessageBox.Show("Error loading chat history: " + response.ReasonPhrase);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load chat history: " + ex.Message);
            }
        }

        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            var messageContent = MessageInput.Text.Trim();

            if (string.IsNullOrEmpty(messageContent) || _selectedUserId == 0)
            {
                return; 
            }

            var messageData = new
            {
                receiverId = _selectedUserId,
                messageContent = messageContent
            };

            var json = JsonConvert.SerializeObject(messageData);

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync("https://localhost:7097/api/chat/send", content);

                if (response.IsSuccessStatusCode)
                {
                    await _connection.InvokeAsync("SendMessage", "Admin", messageContent);
                    MessageInput.Text = string.Empty; 
                }
                else
                {
                    MessageBox.Show("Error sending message: " + response.ReasonPhrase);
                }
            }
        }



        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedUser = UserList.SelectedItem as dynamic; 
            if (selectedUser != null)
            {
                int newSelectedUserId = selectedUser.Id; 
                if (_selectedUserId != newSelectedUserId) 
                {
                    _selectedUserId = newSelectedUserId;
                    LoadChatHistory(); 
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

        private HashSet<string> _sentMessages = new HashSet<string>();

        public void DisplayMessage(string user, string message)
        {
            string uniqueMessageKey = $"{user}: {message}";

            if (_sentMessages.Add(uniqueMessageKey))
            {
                var messageBlock = new TextBlock
                {
                    Text = uniqueMessageKey,
                    Margin = new Thickness(5),
                    Foreground = user == "Admin" ? Brushes.Blue : Brushes.Green
                };

                ChatHistory.Children.Add(messageBlock); 
            }
        }
    }
}