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
            // // Lấy dữ liệu từ các TextBox và DatePicker
            // if (string.IsNullOrWhiteSpace(DiscountCodeTextBox.Text) ||
            //     string.IsNullOrWhiteSpace(DiscountPercentageTextBox.Text) ||
            //     EndDatePicker.SelectedDate == null ||
            //     string.IsNullOrWhiteSpace(UsageLimitTextBox.Text))
            // {
            //     MessageBox.Show("Please fill in all fields.");
            //     return; // Dừng thực hiện nếu có trường không hợp lệ
            // }

            var newDiscount = new Discount
            {
                DiscountCode = DiscountCodeTextBox.Text,
                DiscountPercentage = decimal.Parse(DiscountPercentageTextBox.Text),
                EndDate = EndDatePicker.SelectedDate.Value,
                UsageLimit = int.Parse(UsageLimitTextBox.Text),
                TimesUsed = 0 // Giá trị mặc định cho TimesUsed
            };

            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);
                    client.DefaultRequestHeaders.Add("Accept", "application/json");

                    // Truyền trực tiếp đối tượng Discount mà không cần chuyển đổi
                    // Lưu ý: Đây là cách không chuẩn trong API RESTful
                    HttpResponseMessage response = await client.PostAsJsonAsync(ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/discounts/create", newDiscount);

                    // Đảm bảo phản hồi thành công
                    response.EnsureSuccessStatusCode();

                    MessageBox.Show("Discount added successfully!");
                    this.DialogResult = true; // Chỉ ra rằng việc thêm thành công
                    Close(); // Đóng dialog
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding discount: " + ex.Message);
            }
        }

    }
}
