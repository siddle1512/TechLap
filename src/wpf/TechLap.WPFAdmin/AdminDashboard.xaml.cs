using System.Windows;
using TechLap.WPFAdmin.Components;

namespace TechLap.WPFAdmin
{
    public partial class AdminDashboard : Window
    {
        public AdminDashboard()
        {
            InitializeComponent();
        }

        private void UserManager_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new UserManager();
        }

        private void GroupChat_Click(object sender, RoutedEventArgs e)
        {
            //MainContentArea.Content = new GroupChatControl();  // Load Group Chat UserControl
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            GlobalState.Clear();
            LoginWindow login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}
