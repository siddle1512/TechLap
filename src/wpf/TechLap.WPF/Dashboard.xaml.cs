using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TechLap.WPF.Chat;

namespace TechLap.WPF
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        // Event handlers to load the respective UserControl into the ContentControl
        private void EmployeeManagement_Click(object sender, RoutedEventArgs e)
        {
            //MainContentArea.Content = new EmployeeManager();  // Load Employee management UserControl
        }

        private void LaptopManagement_Click(object sender, RoutedEventArgs e)
        {
            //MainContentArea.Content = new LaptopManager();  // Load Laptop management UserControl
        }

        private void CategoryManagement_Click(object sender, RoutedEventArgs e)
        {
            //MainContentArea.Content = new CategoryManager();  // Load Category management UserControl
        }

        private void UserManagement_Click(object sender, RoutedEventArgs e)
        {
            //MainContentArea.Content = new UserManager();  // Load User management UserControl
        }

        private void OrderManagement_Click(object sender, RoutedEventArgs e)
        {
            //MainContentArea.Content = new OrderManager();  // Load Order management UserControl
        }

        private void SearchLaptop_Click(object sender, RoutedEventArgs e)
        {
            //MainContentArea.Content = new LaptopSearch();  // Load Laptop search UserControl
        }

        private void PromotionManagement_Click(object sender, RoutedEventArgs e)
        {
            //MainContentArea.Content = new PromotionManager();  // Load Promotion management UserControl
        }

        private void GroupChat_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new ChatControl();
        }

        private void CustomerManager_Click(object sender, RoutedEventArgs e)
        {
            MainContentArea.Content = new CustomerManager();
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