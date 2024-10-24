using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;

namespace TechLap.WPF
{
    public partial class ProductManager : UserControl
    {
        private readonly HttpClient _httpClient;

        public ProductManager()
        {
            InitializeComponent();

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiEndpoint"])
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);
        }

        private bool ValidateProduct(ProductResponse product)
        {
            if (string.IsNullOrWhiteSpace(product.Brand) ||
                string.IsNullOrWhiteSpace(product.Model) ||
                string.IsNullOrWhiteSpace(product.Cpu) ||
                product.Price <= 0 || product.Stock < 0 ||
                product.CategoryId <= 0)
            {
                MessageBox.Show("Please fill in all required fields correctly.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
            return true;
        }

        private async void LoadProducts_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var products = await GetProductsAsync();
                ProductDataGrid.ItemsSource = products;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<List<ProductResponse>> GetProductsAsync()
        {
            var response = await _httpClient.GetAsync("api/products");

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ApiProductResponse>(jsonResponse).Data.ToList();
            }
            else
            {
                throw new Exception($"Failed to load products. Status code: {response.StatusCode}");
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            ProductForm.Visibility = Visibility.Visible;
            ProductForm.LoadProduct(new ProductResponse());
            ProductForm.ProductSaved += OnProductFormSaved;
        }

        private void UpdateProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductDataGrid.SelectedItem is ProductResponse selectedProduct)
            {
                ProductForm.Visibility = Visibility.Visible;
                ProductForm.LoadProduct(selectedProduct);
                ProductForm.ProductSaved += OnProductFormSaved;
            }
            else
            {
                MessageBox.Show("Please select a product to update.");
            }
        }

        public async void OnProductFormSaved(ProductResponse product)
        {
            try
            {
                // Validate product fields before sending to API
                if (!ValidateProduct(product)) return;

                HttpResponseMessage response;

                if (product.Id == 0)
                {
                    response = await _httpClient.PostAsJsonAsync("api/products", product);
                }
                else
                {
                    response = await _httpClient.PutAsJsonAsync($"api/products/{product.Id}", product);
                }

                if (response.IsSuccessStatusCode)
                {
                    string message = product.Id == 0 ? "Product added successfully." : "Product updated successfully.";
                    MessageBox.Show(message);
                    LoadProducts_Click(null, null); // Refresh product list
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    MessageBox.Show($"Failed to save product. Status code: {response.StatusCode}. Error: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                ProductForm.Visibility = Visibility.Collapsed;
                ProductForm.ProductSaved -= OnProductFormSaved;
            }
        }

        private async void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductDataGrid.SelectedItem is ProductResponse selectedProduct)
            {
                var response = await _httpClient.DeleteAsync($"api/products/{selectedProduct.Id}");
                if (response.IsSuccessStatusCode)
                {
                    LoadProducts_Click(sender, e);
                }
                else
                {
                    MessageBox.Show($"Failed to delete product. Status code: {response.StatusCode}");
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.");
            }
        }
    }

    public class ApiProductResponse
    {
        public bool IsSuccess { get; set; }
        public List<ProductResponse> Data { get; set; }
        public string? Message { get; set; }
        public string? Details { get; set; }
    }
    public class ProductResponse
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? Cpu { get; set; }
        public string? Ram { get; set; }
        public string? Vga { get; set; }
        public string? ScreenSize { get; set; }
        public string? HardDisk { get; set; }
        public string? OperatingSystem { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? Image { get; set; }
        public int CategoryId { get; set; }
    }
}
