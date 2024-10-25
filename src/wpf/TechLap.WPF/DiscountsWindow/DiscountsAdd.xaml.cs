// DiscountsAdd.xaml.cs

using System.Net.Http;
using System.Windows;
using TechLap.API.Models;
using System.Configuration;
using System.Net.Http.Headers;

namespace TechLap.WPF.DiscountsWindow
{
    public partial class DiscountsAdd : Window
    {
        public DiscountsAdd()
        {
            InitializeComponent();
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
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

            // Kiểm tra và phân tích các giá trị đầu vào
            if (!decimal.TryParse(DiscountPercentageTextBox.Text, out var discountPercentage) || discountPercentage < 1 || discountPercentage > 100)
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

            if (EndDatePicker.SelectedDate == null || EndDatePicker.SelectedDate.Value.Date <= DateTime.Now.Date)
            {
                MessageBox.Show("Ngày hết hạn phải sau ngày hôm nay.",
                    "Lỗi Nhập Liệu",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var newDiscount = new Discount
            {
                DiscountCode = discountCode,
                DiscountPercentage = discountPercentage,
                EndDate = EndDatePicker.SelectedDate.Value,
                UsageLimit = usageLimit,
                TimesUsed = 0 // Giá trị mặc định cho TimesUsed
            };

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", GlobalState.Token);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    // Gửi yêu cầu POST để thêm discount
                    HttpResponseMessage response = await client.PostAsJsonAsync(
                        ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/discounts/create", newDiscount);

                    // Đảm bảo phản hồi thành công
                    if (response.IsSuccessStatusCode)
                    {
                        MessageBox.Show("Giảm giá đã được thêm thành công!",
                            "Thành Công",
                            MessageBoxButton.OK,
                            MessageBoxImage.Information);
                        this.DialogResult = true; // Chỉ ra rằng việc thêm thành công
                        Close(); // Đóng dialog
                    }
                    else
                    {
                        // Xử lý các lỗi không thành công
                        string errorMessage = await response.Content.ReadAsStringAsync();
                        MessageBox.Show($"Lỗi khi thêm giảm giá: Mã đã tồn tại",
                            "Lỗi",
                            MessageBoxButton.OK,
                            MessageBoxImage.Error);
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Lỗi kết nối: " + ex.Message,
                    "Lỗi Kết Nối",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm giảm giá: " + ex.Message,
                    "Lỗi",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }
    }
}