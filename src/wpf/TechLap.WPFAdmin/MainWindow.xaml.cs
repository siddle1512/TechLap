using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TechLap.WPFAdmin
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
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