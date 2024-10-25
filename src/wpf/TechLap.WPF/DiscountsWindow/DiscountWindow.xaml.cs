using System.Configuration;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using TechLap.API;
using TechLap.API.DTOs.Requests.DiscountRequests;
using TechLap.API.DTOs.Responses.DiscountRespones;
using TechLap.API.Exceptions;
using TechLap.API.Models;

namespace TechLap.WPF.DiscountsWindow
{
    public partial class DiscountWindow : UserControl
    {
        private List<Discount>? _discounts;


        public DiscountWindow()
        {
            InitializeComponent();
            // LoadDiscounts(); // Tải danh sách giảm giá từ API khi khởi động
        }

        // Lấy danh sách Discount từ API
        private async void LoadDiscounts(object sender, RoutedEventArgs e)
        {
            try
            {
                // Hiển thị progress bar khi đang load
                progressBar.Visibility = Visibility.Visible;

                // Tạo HttpClient và gán JWT token vào header
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);
                client.DefaultRequestHeaders.Add("Accept", "application/json");

                HttpResponseMessage response =
                    await client.GetAsync(ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/discounts");

                // Kiểm tra mã trạng thái trả về từ API
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    throw new NotFoundException("No discounts found on the server.");

                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parse JSON response thành ApiResponse
                var apiResponse =
                    JsonConvert.DeserializeObject<ApiResponse<List<GetAdminDiscountRespones>>>(responseBody);

                // Kiểm tra nếu không có dữ liệu
                if (apiResponse == null || !apiResponse.IsSuccess || apiResponse.Data == null ||
                    !apiResponse.Data.Any())
                {
                    throw new Exception("No discounts found.");
                }

                // Gán dữ liệu vào DataGrid
                DiscountDataGrid.ItemsSource = apiResponse.Data;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading discounts: " + ex.Message);
            }
            finally
            {
                // Ẩn progress bar khi đã xử lý xong
                progressBar.Visibility = Visibility.Collapsed;
            }
        }


        // Thêm Discount
        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            DiscountsAdd addWindow = new DiscountsAdd();

            // Show the dialog and wait for it to close
            bool? result = addWindow.ShowDialog();

            if (result == true)
            {
                LoadDiscounts(sender, e); // Reload discounts list after adding
            }
        }

        private Discount _selectedDiscount;

        private async void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            // Lấy radio button được chọn
            var radioButton = sender as RadioButton;
            if (radioButton == null)
                return;

            // Lấy discount được chọn từ DataGrid
            var selectedDiscount = radioButton.DataContext as GetAdminDiscountRespones;
            if (selectedDiscount == null)
                return;

            // Tạo HttpClient và gán JWT token vào header
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            // Gọi API để lấy thông tin chi tiết discount bằng ID
            HttpResponseMessage response =
                await client.GetAsync(ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/discounts/" +
                                      selectedDiscount.Id);

            // Kiểm tra mã trạng thái trả về từ API
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                throw new NotFoundException("Discount not found on the server.");

            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();

            // Parse JSON response thành Discount
            var discountDetail = JsonConvert.DeserializeObject<ApiResponse<Discount>>(responseBody);

            // Kiểm tra nếu không có dữ liệu
            if (discountDetail == null || !discountDetail.IsSuccess || discountDetail.Data == null)
            {
                throw new Exception("No discount details found.");
            }

            // Gán dữ liệu vào _selectedDiscount
            _selectedDiscount = discountDetail.Data;

            // Cập nhật các trường trong form hiển thị chi tiết discount
            IdTextBox.Text = _selectedDiscount.Id.ToString();
            DiscountCodeTextBox.Text = _selectedDiscount.DiscountCode;
            PercentageTextBox.Text = _selectedDiscount.DiscountPercentage.ToString();
            StartDateTextBox.Text = _selectedDiscount.StartDate.ToString("yyyy-MM-dd");
            EndDateTextBox.Text = _selectedDiscount.EndDate.ToString("yyyy-MM-dd");
            UsageLimitTextBox.Text = _selectedDiscount.UsageLimit.ToString();
            TimesUsedTextBox.Text = _selectedDiscount.TimesUsed.ToString();
            // // Nếu có trường Status thì cũng có thể gán vào tương tự
        }


        private static readonly HttpClient client = new HttpClient();

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            // Set up HttpClient with the correct headers
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Kiểm tra và chuyển đổi giá trị từ các TextBox
            if (string.IsNullOrWhiteSpace(DiscountCodeTextBox.Text) ||
                !decimal.TryParse(PercentageTextBox.Text, out var discountPercentage) ||
                !DateTime.TryParse(EndDateTextBox.Text, out var endDate) ||
                !int.TryParse(UsageLimitTextBox.Text, out var usageLimit))
            {
                MessageBox.Show("Error: Please ensure all fields are filled out correctly.");
                return;
            }

            // Tạo request body
            var discountToUpdate = new UpdateAdminDiscountRequest(
                DiscountCodeTextBox.Text,
                discountPercentage,
                endDate,
                usageLimit
            );

            // Xác định URL của API
            string apiUrl = ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/discounts/" + _selectedDiscount.Id;

            try
            {
                // Gửi yêu cầu PUT đến API
                HttpResponseMessage response = await client.PutAsJsonAsync(apiUrl, discountToUpdate);

                // Kiểm tra mã trạng thái HTTP
                if (!response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        MessageBox.Show("Error: Discount not found. Please check the ID and try again.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                    {
                        MessageBox.Show("Error: Unauthorized. Please check your token and try again.");
                    }
                    else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                    {
                        MessageBox.Show($"Error: Bad request. {responseBody}");
                    }
                    else
                    {
                        MessageBox.Show($"Error: {response.StatusCode} - {responseBody}");
                    }

                    return;
                }

                // Deserialize JSON response thành ApiResponse
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<UpdateAdminDiscountRequest>>(jsonResponse);

                if (apiResponse == null || !apiResponse.IsSuccess || apiResponse.Data == null)
                {
                    MessageBox.Show("Error: Failed to update discount. Please check your input and try again.");
                    return;
                }

                MessageBox.Show("Discount updated successfully!");
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Error during request: " + ex.Message);
            }
            catch (JsonSerializationException ex)
            {
                MessageBox.Show("Error during deserialization: " + ex.Message);
            }
            LoadDiscounts(sender, e);
        }


        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra nếu đã có discount được chọn thông qua radio button
            if (_selectedDiscount != null)
            {
                // Hiển thị thông báo cảnh báo trước khi xóa
                MessageBoxResult result = MessageBox.Show(
                    "Are you sure you want to delete this discount?",
                    "Delete Confirmation",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                // Kiểm tra nếu người dùng chọn "Yes"
                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        using var client = new HttpClient();
                        client.DefaultRequestHeaders.Authorization =
                            new AuthenticationHeaderValue("Bearer", GlobalState.Token);
                        client.DefaultRequestHeaders.Add("Accept", "application/json");

                        // Tạo đối tượng yêu cầu xóa discount
                        var request = new DeleteAdminDiscountRequest
                        {
                            Id = _selectedDiscount.Id // Gán ID của discount cần xóa
                        };

                        // Chuyển đối tượng yêu cầu thành JSON
                        var jsonRequest = JsonConvert.SerializeObject(request);
                        var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                        // Gửi yêu cầu POST đến API xóa discount
                        HttpResponseMessage response = await client.PostAsync(
                            ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/discounts/delete", content);

                        response.EnsureSuccessStatusCode();

                        MessageBox.Show("Discount deleted successfully!");

                        // Tải lại danh sách discount sau khi xóa
                        LoadDiscounts(sender, e);
                        ClearDiscountDetails();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error deleting discount: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a discount to delete.");
            }
        }

        private void ClearDiscountDetails()
        {
            // Xóa trống các trường hiển thị chi tiết discount
            IdTextBox.Text = string.Empty;
            DiscountCodeTextBox.Text = string.Empty;
            PercentageTextBox.Text = string.Empty;
            StartDateTextBox.Text = string.Empty;
            EndDateTextBox.Text = string.Empty;
            UsageLimitTextBox.Text = string.Empty;
            TimesUsedTextBox.Text = string.Empty;

            // Xóa lựa chọn discount hiện tại
            _selectedDiscount = null;
        }
    }
}