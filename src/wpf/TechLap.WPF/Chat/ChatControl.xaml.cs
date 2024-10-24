using Microsoft.AspNetCore.SignalR.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace TechLap.WPF.Chat
{
    public partial class ChatControl : UserControl
    {
        private HubConnection _connection;
        private const string ApiUrl = "https://localhost:7097/api/chat/history"; // Địa chỉ API để lấy lịch sử tin nhắn

        public ChatControl()
        {
            InitializeComponent();
            InitializeSignalR();
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
                Foreground = user == "User" ? Brushes.Green : Brushes.Blue // Màu xanh cho admin, màu xanh lá cho user
            };

            ChatHistory.Children.Add(messageBlock); // Thêm tin nhắn vào UI container
        }


        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            var messageContent = MessageInput.Text.Trim();

            if (string.IsNullOrEmpty(messageContent) || messageContent == "Enter your message...")
            {
                return; // Nếu ô nhập rỗng hoặc chứa placeholder, không gửi
            }

            // Gửi tin nhắn qua SignalR
            await _connection.InvokeAsync("SendMessage", "User", messageContent);
            MessageInput.Text = string.Empty; // Xóa ô nhập
        }

        private void MessageInput_GotFocus(object sender, RoutedEventArgs e)
        {
            if (MessageInput.Text == "Enter your message...")
            {
                MessageInput.Text = string.Empty; // Xóa placeholder khi có focus
            }
        }
    }
}
