using System.Windows;
using TechLap.WPFAdmin.Components;
using TechLap.WPFAdmin.ChatNew;
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
            MainContentArea.Content = new ChatNewControl();
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
