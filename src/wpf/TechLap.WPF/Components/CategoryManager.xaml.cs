using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Windows;
using System.Windows.Controls;
using TechLap.API;
using TechLap.API.DTOs.Requests;
using TechLap.API.Models;

namespace TechLap.WPF.Components
{
    public partial class CategoryManager : UserControl
    {
        HttpClient client = new HttpClient();
        public CategoryManager()
        {
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                );
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);
            InitializeComponent();
        }

        private void btnLoadCategory_Click(object sender, RoutedEventArgs e)
        {
            this.GetCategories();
        }

        private async void GetCategories()
        {
            var response = await client.GetStringAsync("categories");
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<Category>>>(response);
            dgCategory.ItemsSource = apiResponse?.Data;
        }

        private async void CreateCategory(CreateCategoryRequest request)
        {
            await client.PostAsJsonAsync("categories", request);
        }

        private async void UpdateCategory(CreateCategoryRequest request)
        {
            await client.PutAsJsonAsync("categories/" + request.Id, request);
        }

        private async void RemoveCategory(int id)
        {
            await client.DeleteAsync("categories/" + id);
        }

        private void btnEditCategory_Click(object sender, RoutedEventArgs e)
        {
            Category? category = ((FrameworkElement)sender).DataContext as Category;
            txtCategoryId.Text = category?.Id.ToString();
            txtCategoryName.Text = category?.Name;
        }

        private void btnDeleteCategory_Click(object sender, RoutedEventArgs e)
        {
            Category? category = ((FrameworkElement)sender).DataContext as Category;
            if (category?.Id != null)
            {
                RemoveCategory(category.Id);
            }
        }

        private void btnCategorySave_Click(object sender, RoutedEventArgs e)
        {
            var request = new CreateCategoryRequest(
                Id: Convert.ToInt32(txtCategoryId.Text),
                Name: txtCategoryName.Text
                );

            if (request.Id == 0)
            {
                this.CreateCategory(request);
            }
            else
            {
                this.UpdateCategory(request);
            }

            txtCategoryId.Text = "0";
            txtCategoryName.Text = string.Empty;
        }
    }
}

