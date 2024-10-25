using System.Configuration;
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
    public partial class DiscountWindow
    {
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
            // Lấy dữ liệu từ các TextBox và DatePicker
            string discountCode = DiscountCodeTextBox.Text;
            if (string.IsNullOrWhiteSpace(discountCode) ||
                discountCode.Length < 3 ||
                discountCode.Length > 20)
            {
                MessageBox.Show("Mã giảm giá phải có độ dài từ 4 đến 20 ký tự.",
                    "Lỗi Nhập Liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return; // Dừng thực hiện nếu mã giảm giá không hợp lệ
            }

            if (!decimal.TryParse(PercentageTextBox.Text, out var discountPercentage) || discountPercentage < 1 ||
                discountPercentage > 100)
            {
                MessageBox.Show("Phần trăm giảm giá không hợp lệ. Vui lòng nhập một số trong khoảng từ 1 đến 100.",
                    "Lỗi Nhập Liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(UsageLimitTextBox.Text, out var usageLimit) || usageLimit <= 0)
            {
                MessageBox.Show("Giới hạn sử dụng không hợp lệ. Vui lòng nhập một số nguyên dương.",
                    "Lỗi Nhập Liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            if (!DateTime.TryParse(EndDateTextBox.Text, out var endDate) || endDate <= DateTime.Now.Date)
            {
                MessageBox.Show("Ngày hết hạn phải sau ngày hôm nay.",
                    "Lỗi Nhập Liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            // Tạo request body
            var discountToUpdate = new UpdateAdminDiscountRequest(
                discountCode,
                discountPercentage,
                endDate,
                usageLimit
            );

            // Xác định URL của API
            string apiUrl = ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/discounts/" + _selectedDiscount.Id;

            try
            {
                // Thiết lập HttpClient với các tiêu đề đúng
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", GlobalState.Token);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    HttpResponseMessage response = await client.PutAsJsonAsync(apiUrl, discountToUpdate);

                    if (!response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Lỗi khi cập nhật: {response.StatusCode} - {responseBody}",
                            "Lỗi",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    var apiResponse = JsonConvert.DeserializeObject<UpdateRespones<string>>(jsonResponse);

                    if (apiResponse == null || !apiResponse.IsSuccess)
                    {
                        MessageBox.Show($"Lỗi: {apiResponse?.Message ?? "Không thể cập nhật giảm giá."}",
                            "Lỗi",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                        return;
                    }

                    MessageBox.Show("Giảm giá đã được cập nhật thành công!",
                        "Thành Công",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Lỗi trong quá trình gửi yêu cầu: " + ex.Message,
                    "Lỗi Kết Nối",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (JsonSerializationException ex)
            {
                MessageBox.Show("Lỗi trong quá trình phân tích cú pháp: " + ex.Message,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
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
                    "Bạn có muốn xóa discount này không?",
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

                        MessageBox.Show("Xóa thành công!");

                        // Tải lại danh sách discount sau khi xóa
                        LoadDiscounts(sender, e);
                        ClearDiscountDetails();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Lỗi xóa discount " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Hãy chọn discount muốn xóa");
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