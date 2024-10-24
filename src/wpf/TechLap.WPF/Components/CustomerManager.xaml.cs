using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace TechLap.WPF
{
    /// <summary>
    /// Interaction logic for CustomerManager.xaml
    /// </summary>
    public partial class CustomerManager : UserControl
    {
        private readonly HttpClient _httpClient;

        public CustomerManager()
        {
            InitializeComponent();

            // Initialize HttpClient and set base address
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiEndpoint"] ?? throw new ArgumentNullException(nameof(ConfigurationManager)))
            };

            // Set the authorization header with the token from GlobalState
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);
        }

        // Event handler for "Load Customers" button click
        private async void LoadCustomers_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Call the API to get customers
                var customers = await GetCustomersAsync();

                // Populate the ListBox with customer data
                CustomerDataGrid.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to call the API and get customers
        private async Task<List<CustomerResponse>> GetCustomersAsync()
        {
            var response = await _httpClient.GetAsync("api/customers"); // Adjust the endpoint as necessary

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse>(jsonResponse).Data.ToList();
            }
            else
            {
                throw new Exception($"Failed to load customers. Status code: {response.StatusCode}");
            }
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            CustomerForm.Visibility = Visibility.Visible;
            CustomerForm.LoadCustomer(new CustomerResponse());
            CustomerForm.CustomerSaved += OnCustomerFormSaved;
        }

        private void UpdateCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is CustomerResponse selectedCustomer)
            {
                CustomerForm.Visibility = Visibility.Visible;
                CustomerForm.LoadCustomer(selectedCustomer); // Load selected customer for editing
                CustomerForm.CustomerSaved += OnCustomerFormSaved; // Subscribe to event
            }
            else
            {
                MessageBox.Show("Please select a customer to update.");
            }
        }

        public async void OnCustomerFormSaved(CustomerResponse customer)
        {
            try
            {
                if (customer.Id == 0)
                {
                    // Add new customer
                    var response = await _httpClient.PostAsJsonAsync("api/customers", customer);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Customer added successfully.");
                        LoadCustomers_Click(null, null); // Refresh the customer list
                    }
                    else
                    {
                        MessageBox.Show($"Failed to add customer. Status code: {response.StatusCode}");
                    }
                }
                else
                {
                    // Update existing customer
                    var response = await _httpClient.PutAsJsonAsync($"api/customers/{customer.Id}", customer);
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Customer updated successfully.");
                        LoadCustomers_Click(null, null); // Refresh the customer list
                    }
                    else
                    {
                        MessageBox.Show($"Failed to update customer. Status code: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                CustomerForm.Visibility = Visibility.Collapsed; // Hide the form
                CustomerForm.CustomerSaved -= OnCustomerFormSaved; // Unsubscribe from the event
            }
        }

        private async void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is CustomerResponse selectedCustomer)
            {
                var response = await _httpClient.DeleteAsync($"api/customers/{selectedCustomer.Id}");
                if (response.IsSuccessStatusCode)
                {
                    LoadCustomers_Click(sender, e); // Reload customers
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
