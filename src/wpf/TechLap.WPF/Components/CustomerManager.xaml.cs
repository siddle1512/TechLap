using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace TechLap.WPF
{
    public partial class CustomerManager : UserControl
    {
        private readonly HttpClient _httpClient;
        private CustomerResponse _currentCustomer;

        public CustomerManager()
        {
            InitializeComponent();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiEndpoint"] ?? throw new ArgumentNullException(nameof(ConfigurationManager)))
            };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);
        }

        private async void LoadCustomers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var customers = await GetCustomersAsync();
                CustomerDataGrid.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<List<CustomerResponse>> GetCustomersAsync()
        {
            var response = await _httpClient.GetAsync("api/customers");
            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse>(jsonResponse).Data;
            }
            else
            {
                throw new Exception($"Failed to load customers. Status code: {response.StatusCode}");
            }
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            _currentCustomer = new CustomerResponse();
            ShowCustomerForm();
        }

        private void UpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is CustomerResponse selectedCustomer)
            {
                _currentCustomer = selectedCustomer;
                ShowCustomerForm();
            }
            else
            {
                MessageBox.Show("Please select a customer to update.");
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameTextBox.Text) ||
                string.IsNullOrWhiteSpace(EmailTextBox.Text) ||
                string.IsNullOrWhiteSpace(PhoneNumberTextBox.Text))
            {
                MessageBox.Show("All fields are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            _currentCustomer.Name = NameTextBox.Text;
            _currentCustomer.Email = EmailTextBox.Text;
            _currentCustomer.PhoneNumber = PhoneNumberTextBox.Text;

            try
            {
                HttpResponseMessage response;
                if (_currentCustomer.Id == 0)
                {
                    response = await _httpClient.PostAsJsonAsync("api/customers", _currentCustomer);
                }
                else
                {
                    response = await _httpClient.PutAsJsonAsync($"api/customers/{_currentCustomer.Id}", _currentCustomer);
                }

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show(_currentCustomer.Id == 0 ? "Customer added successfully." : "Customer updated successfully.");
                    LoadCustomers_Click(null, null);
                }
                else
                {
                    MessageBox.Show($"Failed to save customer. Status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                HideCustomerForm();
            }
        }

        private async void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is CustomerResponse selectedCustomer)
            {
                var response = await _httpClient.DeleteAsync($"api/customers/{selectedCustomer.Id}");
                if (response.IsSuccessStatusCode)
                {
                    LoadCustomers_Click(sender, e);
                }
                else
                {
                    MessageBox.Show($"Failed to delete customer. Status code: {response.StatusCode}");
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.");
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            HideCustomerForm();
        }

        private void ShowCustomerForm()
        {
            NameTextBox.Text = _currentCustomer.Name;
            EmailTextBox.Text = _currentCustomer.Email;
            PhoneNumberTextBox.Text = _currentCustomer.PhoneNumber;
            CustomerFormPanel.Visibility = Visibility.Visible;
        }

        private void HideCustomerForm()
        {
            CustomerFormPanel.Visibility = Visibility.Collapsed;
            NameTextBox.Clear();
            EmailTextBox.Clear();
            PhoneNumberTextBox.Clear();
            _currentCustomer = null;
        }
    }

    public class ApiResponse
    {
        public bool IsSuccess { get; set; }
        public List<CustomerResponse> Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Details { get; set; } = string.Empty;
    }

    public class CustomerResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
