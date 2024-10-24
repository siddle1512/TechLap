using System.Windows;
using System.Windows.Controls;

namespace TechLap.WPF
{
    public partial class ProductForm : UserControl
    {
        public ProductResponse Product { get; private set; }

        public event Action<ProductResponse> ProductSaved;
        public event Action ProductCanceled;

        public ProductForm()
        {
            InitializeComponent();
        }

        public void LoadProduct(ProductResponse product)
        {
            Product = product;
            BrandTextBox.Text = product.Brand;
            ModelTextBox.Text = product.Model;
            CpuTextBox.Text = product.Cpu;
            RamTextBox.Text = product.Ram;
            VgaTextBox.Text = product.Vga;
            ScreenSizeTextBox.Text = product.ScreenSize;
            HardDiskTextBox.Text = product.HardDisk;
            OsTextBox.Text = product.OperatingSystem;
            PriceTextBox.Text = product.Price.ToString();
            StockTextBox.Text = product.Stock.ToString();
            ImageTextBox.Text = product.Image;
            CategoryComboBox.SelectedValue = product.CategoryId;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrWhiteSpace(BrandTextBox.Text) ||
                string.IsNullOrWhiteSpace(ModelTextBox.Text) ||
                string.IsNullOrWhiteSpace(CpuTextBox.Text) ||
                string.IsNullOrWhiteSpace(PriceTextBox.Text) ||
                !decimal.TryParse(PriceTextBox.Text, out var price) || 
                !int.TryParse(StockTextBox.Text, out var stock))        
            {
                MessageBox.Show("Please fill in all required fields with valid data.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Tạo Product mới hoặc cập nhật Product hiện tại
            Product = new ProductResponse
            {
                Id = Product?.Id ?? 0,
                Brand = BrandTextBox.Text,
                Model = ModelTextBox.Text,
                Cpu = CpuTextBox.Text,
                Ram = RamTextBox.Text,
                Vga = VgaTextBox.Text,
                ScreenSize = ScreenSizeTextBox.Text,
                HardDisk = HardDiskTextBox.Text,
                OperatingSystem = OsTextBox.Text,
                Price = price,        // Sử dụng biến price đã được kiểm tra
                Stock = stock,        // Sử dụng biến stock đã được kiểm tra
                Image = ImageTextBox.Text,
                CategoryId = (int)CategoryComboBox.SelectedValue // Kiểm tra giá trị của CategoryComboBox
            };

            // Gửi sự kiện ProductSaved
            ProductSaved?.Invoke(Product);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ProductCanceled?.Invoke();
            ClearForm();
        }

        private void ClearForm()
        {
            BrandTextBox.Clear();
            ModelTextBox.Clear();
            CpuTextBox.Clear();
            RamTextBox.Clear();
            VgaTextBox.Clear();
            ScreenSizeTextBox.Clear();
            HardDiskTextBox.Clear();
            OsTextBox.Clear();
            PriceTextBox.Clear();
            StockTextBox.Clear();
            ImageTextBox.Clear();
            CategoryComboBox.SelectedIndex = -1;
            Product = null;
        }
    }
}