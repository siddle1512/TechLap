using System.Windows;
using System.Windows.Controls;

namespace TechLap.WPF
{
    public partial class CustomerForm : UserControl
    {
        public CustomerResponse Customer { get; private set; }

        public event Action<CustomerResponse> CustomerSaved;

        public CustomerForm()
        {
            InitializeComponent();
        }

        public void LoadCustomer(CustomerResponse customer)
        {
            Customer = customer;
            NameTextBox.Text = customer.Name;
            EmailTextBox.Text = customer.Email;
            PhoneNumberTextBox.Text = customer.PhoneNumber;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            Customer = new CustomerResponse
            {
                Id = Customer?.Id ?? 0, 
                Name = NameTextBox.Text,
                Email = EmailTextBox.Text,
                PhoneNumber = PhoneNumberTextBox.Text
            };

            // Raise the event
            CustomerSaved?.Invoke(Customer);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Optionally clear fields or perform other cancel actions
            NameTextBox.Clear();
            EmailTextBox.Clear();
            PhoneNumberTextBox.Clear();
            Customer = null;
        }
    }
}