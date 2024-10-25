using System.Net.Http;
using System.Windows;
using System.Configuration;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Windows.Controls;
using Newtonsoft.Json;
using TechLap.API;
using TechLap.API.DTOs.Requests;
using TechLap.API.DTOs.Responses.ProductRespones;
using TechLap.API.Models;

namespace TechLap.WPF.Components
{
    public partial class SearchProducts : UserControl
    {
        public SearchProducts()
        {
            InitializeComponent();
        }

        private async void Search_Click(object sender, RoutedEventArgs e)
        {
            string brand = BrandTextBox.Text;
            string model = ModelTextBox.Text;
            string cpu = CpuTextBox.Text;
            string ram = RamTextBox.Text;
            string vga = VgaTextBox.Text;
            string screenSize = ScreenSizeTextBox.Text;
            string hardDisk = HardDiskTextBox.Text;
            string operatingSystem = OperatingSystemTextBox.Text;

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", GlobalState.Token);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    var searchRequest = new SearchProductsRequest(brand, model, cpu, ram, vga, screenSize, hardDisk, operatingSystem);
                    string url = ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/products/searchConfiguration";

                    HttpResponseMessage response = await client.PostAsJsonAsync(url, searchRequest);

                    if (response.IsSuccessStatusCode)
                    {
                        var jsonContent = await response.Content.ReadAsStringAsync();
                        var searchResponse = JsonConvert.DeserializeObject<ApiResponse<List<Product>>>(jsonContent); // Đổi về kiểu Product

                        if (searchResponse != null && searchResponse.IsSuccess)
                        {
                            ProductDataGrid.ItemsSource = null; // Reset trước khi gán dữ liệu mới
                            ProductDataGrid.ItemsSource = searchResponse.Data; // Gán dữ liệu vào DataGrid
                        }
                        else
                        {
                            MessageBox.Show("Không tìm thấy sản phẩm phù hợp.", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    else
                    {
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Lỗi khi tìm kiếm sản phẩm: {errorMessage}", "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message, "Lỗi Kết Nối", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tìm kiếm sản phẩm: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
