using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using TechLap.API;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Requests.DiscountRequests;
using TechLap.API.DTOs.Responses.OrderDTOs;
using TechLap.API.Enums;

namespace TechLap.WPF
{
    public partial class OrderManager : UserControl
    {
        private readonly HttpClient _httpClient;
        private List<CustomerResponse> _allCustomers = new List<CustomerResponse>();
        private List<ProductResponse> _allProducts = new List<ProductResponse>();
        private List<OrderDetailRequest> _addedProducts = new List<OrderDetailRequest>();

        public OrderManager()
        {
            InitializeComponent();

            var apiEndpoint = ConfigurationManager.AppSettings["ApiEndpoint"]
                ?? throw new InvalidOperationException("API endpoint is not configured.");
            _httpClient = new HttpClient { BaseAddress = new Uri(apiEndpoint) };
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);

            txtOrderDate.Text = DateTime.Now.ToString("yyyy-MM-dd");

            _ = LoadCustomersAsync();
            _ = LoadProductsAsync();
        }

        private async Task<List<OrderResponse>?> GetOrdersAsync()
        {
            var response = await _httpClient.GetAsync("api/orders");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiResponse<List<OrderResponse>>>(jsonResponse)?.Data;
            }
            else
            {
                throw new Exception($"Error: {response.ReasonPhrase}");
            }
        }

        private async Task<bool> CreateOrderAsync(OrderRequest orderRequest)
        {
            var json = JsonConvert.SerializeObject(orderRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/orders", content);

            return response.IsSuccessStatusCode;
        }

        private async Task LoadCustomersAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/customers");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<CustomerResponse>>>(jsonResponse);
                    _allCustomers = apiResponse?.Data ?? new List<CustomerResponse>();

                    cmbCustomer.ItemsSource = _allCustomers;
                }
                else
                {
                    MessageBox.Show($"Error: {response.ReasonPhrase}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SearchCustomerTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var searchText = txtCustomer.Text.ToLower();
            var filteredCustomers = _allCustomers
                .Where(c => c.Name.ToLower().Contains(searchText))
                .ToList();

            cmbCustomer.ItemsSource = filteredCustomers;
        }

        private async Task<bool> ApplyDiscountAsync(string discountCode)
        {
            try
            {
                var request = new ApplyUserDiscountRequest(discountCode);

                var json = JsonConvert.SerializeObject(request);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"api/discounts/{discountCode}", content);

                return response.IsSuccessStatusCode;


            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying discount: {ex.Message}", "Discount Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/products");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<ProductResponse>>>(jsonResponse);
                    _allProducts = apiResponse?.Data ?? new List<ProductResponse>();

                    cmbProduct.ItemsSource = _allProducts;
                }
                else
                {
                    MessageBox.Show($"Error: {response.ReasonPhrase}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task LoadOrderDetailAsync(int orderId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/orders/{orderId}");

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<ApiResponse<OrderResponse>>(jsonResponse);
                    var order = apiResponse?.Data;

                    if (order != null)
                    {
                        DisplayOrderDetail(order);
                    }
                }
                else
                {
                    MessageBox.Show($"Error: {response.ReasonPhrase}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<bool> UpdateOrderAsync(int orderId, OrderRequest orderRequest)
        {
            var json = JsonConvert.SerializeObject(orderRequest);
            var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"api/orders/{orderId}", content);

            return response.IsSuccessStatusCode;
        }

        private void SearchProductTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            var searchText = txtSearchProduct.Text.ToLower();
            var filteredProducts = _allProducts
                .Where(c => c.Model != null && c.Model.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            cmbProduct.ItemsSource = filteredProducts;
        }

        private async void btnLoadOrders_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var orders = await GetOrdersAsync();
                ClearOrderForm();

                dgOrder.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task RefreshOrderList()
        {
            try
            {
                var orders = await GetOrdersAsync();
                dgOrder.ItemsSource = orders;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ProductComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cmbProduct.SelectedItem is ProductResponse selectedProduct)
            {
                txtPrice.Text = selectedProduct.Price.ToString("F2");
                txtQuantity.Text = "1";
            }
            else
            {
                txtPrice.Text = "0.00";
            }
        }

        private void QuantityTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            if (cmbProduct.SelectedItem is ProductResponse selectedProduct && int.TryParse(txtQuantity.Text, out int quantity))
            {
                var price = selectedProduct.Price * quantity;
                txtPrice.Text = price.ToString("F2");
            }
            else
            {
                txtPrice.Text = "0.00";
            }
        }

        private void AddProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (cmbProduct.SelectedItem is ProductResponse selectedProduct && int.TryParse(txtQuantity.Text, out int quantity))
            {
                var existingProduct = _addedProducts.FirstOrDefault(p => p.ProductId == selectedProduct.Id);

                if (existingProduct != null)
                {
                    var updatedProduct = new OrderDetailRequest(
                        existingProduct.ProductId,
                        existingProduct.Quantity + quantity,
                        existingProduct.Price + (selectedProduct.Price * quantity)
                    );

                    _addedProducts[_addedProducts.IndexOf(existingProduct)] = updatedProduct;
                }
                else
                {
                    var price = selectedProduct.Price * quantity;

                    var orderDetail = new OrderDetailRequest(
                        selectedProduct.Id,
                        quantity,
                        price
                    );

                    _addedProducts.Add(orderDetail);
                }

                dgProducts.ItemsSource = null;
                dgProducts.ItemsSource = _addedProducts;

                CalculateTotalPrice();
            }
            else
            {
                MessageBox.Show("Please select a valid product and enter quantity.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_addedProducts.Count == 0)
                {
                    MessageBox.Show("Please add at least one product to the order.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbCustomer.SelectedValue == null)
                {
                    MessageBox.Show("Please select a customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbPaymentMethod.SelectedItem == null)
                {
                    MessageBox.Show("Please select a payment method.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (cmbOrderStatus.SelectedItem == null)
                {
                    MessageBox.Show("Please select an order status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                var discountCode = txtDiscountCode.Text.Trim();

                if (!string.IsNullOrEmpty(discountCode))
                {
                    var discountApplied = await ApplyDiscountAsync(discountCode);
                    if (!discountApplied)
                    {
                        MessageBox.Show("Failed to apply discount code.", "Discount Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                var selectedCustomerId = (int)cmbCustomer.SelectedValue;
                var paymentMethod = (PaymentMethod)Enum.Parse(typeof(PaymentMethod),
                    cmbPaymentMethod.Text);
                var orderStatus = (OrderStatus)Enum.Parse(typeof(OrderStatus),
                    cmbOrderStatus.Text);

                var orderRequest = new OrderRequest(
                    DateTime.Now,
                    decimal.Parse(txtTotalPrice.Text),
                    paymentMethod,
                    orderStatus,
                    int.TryParse(discountCode, out int discountId) ? (int?)discountId : null,
                    _addedProducts,
                    selectedCustomerId
                );

                if (dgOrder.SelectedItem is OrderResponse selectedOrder)
                {
                    var isSuccess = await UpdateOrderAsync(selectedOrder.Id, orderRequest);
                    if (isSuccess)
                    {
                        MessageBox.Show("Order updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to update the order. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    var isSuccess = await CreateOrderAsync(orderRequest);
                    if (isSuccess)
                    {
                        MessageBox.Show("Order created successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Failed to create the order. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                await RefreshOrderList();
                ClearOrderForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CalculateTotalPrice()
        {
            var totalPrice = _addedProducts.Sum(p => p.Price);
            txtTotalPrice.Text = totalPrice.ToString("F2");
        }

        private void ClearOrderForm()
        {
            cmbCustomer.SelectedIndex = -1;
            cmbProduct.SelectedIndex = -1;
            txtQuantity.Text = "1";
            txtPrice.Text = "0.00";
            _addedProducts.Clear();
            dgProducts.ItemsSource = null;
            txtTotalPrice.Text = "0.00";
            txtDiscountCode.Text = string.Empty;
            cmbPaymentMethod.SelectedIndex = -1;
            cmbOrderStatus.SelectedIndex = -1;
        }

        private void RemoveProductButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgProducts.SelectedItem is OrderDetailRequest selectedProduct)
            {
                _addedProducts.Remove(selectedProduct);
                dgProducts.ItemsSource = null;
                dgProducts.ItemsSource = _addedProducts;
            }
            else
            {
                MessageBox.Show("Please select a product to remove.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private async void OrderSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dgOrder.SelectedItem is OrderResponse selectedOrder)
            {
                await LoadOrderDetailAsync(selectedOrder.Id);
            }
        }

        private void DisplayOrderDetail(OrderResponse order)
        {
            txtOrderDate.Text = order.OrderDate.ToString("yyyy-MM-dd");
            cmbCustomer.SelectedValue = order.CustomerId;
            cmbPaymentMethod.Text = order.PaymentMethod;
            cmbOrderStatus.Text = order.OrderStatus;

            _addedProducts = order.OrderDetails
                .Select(od => new OrderDetailRequest(od.ProductId, od.Quantity, od.Price))
                .ToList();

            dgProducts.ItemsSource = null;
            dgProducts.ItemsSource = _addedProducts;

            CalculateTotalPrice();
        }
    }
}
