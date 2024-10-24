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
            OsTextBox.Text = product.Os;
            PriceTextBox.Text = product.Price.ToString();
            StockTextBox.Text = product.Stock.ToString();
            ImageTextBox.Text = product.Image;
            CategoryComboBox.SelectedValue = product.CategoryId;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            
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
                Os = OsTextBox.Text,
                Price = price,        
                Stock = stock,        
                Image = ImageTextBox.Text,
                CategoryId = 1,
            };

        
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