using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Windows;
using System.Windows.Controls;

namespace TechLap.WPFAdmin
{
    public partial class AdminDashboard : Window
    {
        private HubConnection _connection;

        public AdminDashboard()
        {
            InitializeComponent();
            InitializeSignalR();
        }

        private async void InitializeSignalR()
        {
            // Tạo kết nối tới SignalR Hub
            _connection = new HubConnectionBuilder()
                .WithUrl("https://localhost:7170/chatHub") // Đường dẫn đúng tới hub
                .Build();

            // Nhận tin nhắn từ server và hiển thị trên màn hình
            _connection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                Dispatcher.Invoke(() =>
                {
                    MessageContainer.Children.Add(new TextBlock { Text = $"{user}: {message}" });
                });
            });


            // Khởi động kết nối
            await _connection.StartAsync();
        }

        private void Chat_Click(object sender, RoutedEventArgs e)
        {
            // Hiển thị giao diện Chat
            DashboardContent.Visibility = Visibility.Collapsed;
            ChatContent.Visibility = Visibility.Visible;

            // Xóa nội dung chat cũ
            MessageContainer.Children.Clear();
        }

        private async void SendMessage_Click(object sender, RoutedEventArgs e)
        {
            string message = MessageInput.Text;

            if (!string.IsNullOrEmpty(message))
            {
                // Gửi tin nhắn đến server
                await _connection.InvokeAsync("SendMessage", "Admin", message);

                // Xóa tin nhắn trong TextBox sau khi gửi
                MessageInput.Clear();
            }
        }

    }
}
