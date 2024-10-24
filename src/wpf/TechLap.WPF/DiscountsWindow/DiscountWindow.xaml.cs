using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using TechLap.API;
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

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra xem có hàng nào được chọn không
            if (DiscountDataGrid.SelectedItem is Discount selectedDiscount)
            {
                // Tạo một cửa sổ mới để chỉnh sửa Discount đã chọn
                DiscountEdit editWindow = new DiscountEdit(selectedDiscount.Id);

                // Hiển thị cửa sổ chỉnh sửa và chờ cho nó đóng lại
                bool? result = editWindow.ShowDialog();

                if (result == true)
                {
                    LoadDiscounts(sender, e); // Tải lại danh sách Discounts sau khi chỉnh sửa
                }
            }
            else
            {
                MessageBox.Show("Please select a discount to edit."); // Thông báo nếu không có hàng nào được chọn
            }
        }


        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            // var selectedDiscount = DiscountDataGrid.SelectedItem as Discount;
            // if (selectedDiscount != null)
            // {
            //     try
            //     {
            //         using var client = new HttpClient();
            //         HttpResponseMessage response = await client.DeleteAsync(
            //             ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/discount/delete/" +
            //             selectedDiscount.Id);
            //         response.EnsureSuccessStatusCode();
            //
            //         LoadDiscounts(sender, e); // Reload discounts list
            //     }
            //     catch (Exception ex)
            //     {
            //         MessageBox.Show("Error deleting discount: " + ex.Message);
            //     }
            // }
            // else
            // {
            //     MessageBox.Show("Please select a discount to delete.");
            // }
        }

        



    }
}
