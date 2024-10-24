using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
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
        private string ApiUrl = ConfigurationManager.AppSettings["ApiEndpoint"];
        public ChatNewControl()
        {
            InitializeComponent();
            LoadUsers(); // Tải danh sách người dùng
            InitializeSignalR();
        }
        private async void LoadUsers()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GlobalState.Token}");

            HttpResponseMessage response = await client.GetAsync($"{ApiUrl}/api/users");
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

                UserList.DisplayMemberPath = "Name"; // Hiển thị tên người dùng
                UserList.SelectedValuePath = "Id"; // Lưu ID của người dùng
            }
            else
            {
                MessageBox.Show("Error loading users: " + response.ReasonPhrase);
            }
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
                    // Chỉ hiển thị tin nhắn nếu nó đến từ một người khác
                    if (user != "Admin")
                    {
                        DisplayMessage(user, message);
                    }
                    else
                    {
                        // Nếu là tin nhắn từ admin, bạn có thể hiển thị trực tiếp mà không cần kiểm tra
                        DisplayMessage("Admin", message);
                    }
                });
            });


            await _connection.StartAsync();
        }

        private async void LoadChatHistory()
        {
            if (_selectedUserId == 0) return; // Kiểm tra nếu không có người dùng nào được chọn
            try
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {GlobalState.Token}");
                HttpResponseMessage response = await client.GetAsync($"{ApiUrl}/api/chat/history?userId={_selectedUserId}&adminId=1");

                if (response.IsSuccessStatusCode)
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var chatMessages = JArray.Parse(responseBody);

                    foreach (var message in chatMessages)
                    {
                        string sender = message["isFromAdmin"].Value<bool>() ? "Admin" : "User";
                        string content = message["messageContent"].Value<string>();
                        DisplayMessage(sender, content);
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
                return; // Không gửi nếu ô nhập rỗng hoặc không có người dùng được chọn
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
                    // Gửi tin nhắn qua SignalR
                    await _connection.InvokeAsync("SendMessage", "Admin", messageContent); // Gửi tin nhắn qua SignalR
                    MessageInput.Text = string.Empty; // Xóa nội dung ô nhập
                }
                else
                {
                    MessageBox.Show("Error sending message: " + response.ReasonPhrase);
                }
            }
        }



        private void UserList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selectedUser = UserList.SelectedItem as dynamic; // Sử dụng dynamic để truy cập ID
            if (selectedUser != null)
            {
                _selectedUserId = selectedUser.Id; // Lưu ID của người dùng đã chọn
                LoadChatHistory(); // Tải lịch sử chat cho người dùng đã chọn
            }
        }

        private void MessageInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MessageInput.Text == "Enter your message...")
            {
                MessageInput.Text = string.Empty; // Xóa placeholder khi có focus
            }
        }

        //public void DisplayMessage(string user, string message)
        //{
        //    var messageBlock = new TextBlock
        //    {
        //        Text = $"{user}: {message}",
        //        Margin = new Thickness(5)
        //    };

        //    // Thay đổi màu sắc của tin nhắn dựa trên người gửi
        //    messageBlock.Foreground = user == "Admin" ? Brushes.Blue : Brushes.Green;

        //    ChatHistory.Children.Add(messageBlock); // Thêm tin nhắn vào UI container
        //}

        private HashSet<string> _sentMessages = new HashSet<string>();

        public void DisplayMessage(string user, string message)
        {
            string uniqueMessageKey = $"{user}: {message}";

            // Chỉ hiển thị tin nhắn nếu nó chưa được hiển thị
            if (_sentMessages.Add(uniqueMessageKey))
            {
                var messageBlock = new TextBlock
                {
                    Text = uniqueMessageKey,
                    Margin = new Thickness(5),
                    Foreground = user == "Admin" ? Brushes.Blue : Brushes.Green
                };

                ChatHistory.Children.Add(messageBlock); // Thêm tin nhắn vào UI container
            }
        }
    }
}