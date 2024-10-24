using Newtonsoft.Json;
using System.Configuration;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Windows;
using System.Windows.Controls;
using TechLap.API;
using TechLap.API.DTOs.Requests;
using TechLap.API.Enums;
using TechLap.API.Models;

namespace TechLap.WPFAdmin.Components
{
    public partial class UserManager : UserControl
    {
        HttpClient client = new HttpClient();

        public UserManager()
        {
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["ApiEndpoint"] + "/api/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json")
                );
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", GlobalState.Token);
            InitializeComponent();
        }

        private void btnLoadUsers_Click(object sender, RoutedEventArgs e)
        {
            this.GetUsers();
        }

        private async void GetUsers()
        {
            var response = await client.GetStringAsync("users");
            var apiResponse = JsonConvert.DeserializeObject<ApiResponse<IEnumerable<User>>>(response);
            dgUser.ItemsSource = apiResponse?.Data;
        }

        private async void CreateUser(CreateUserRequest request)
        {
            await client.PostAsJsonAsync("users", request);
        }

        private async void UpdateUser(CreateUserRequest request)
        {
            await client.PutAsJsonAsync("users/" + request.Id, request);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var request = new CreateUserRequest(
                Id: Convert.ToInt32(txtUserId.Text),
                FullName: txtFullName.Text,
                BirthYear: dpBirthYear.SelectedDate ?? DateTime.Now,
                Gender: cmbGender.SelectedItem is ComboBoxItem selectedGender
                    ? Enum.TryParse<Gender>(selectedGender.Content as string, out var gender)
                        ? gender
                        : Gender.Other
                    : Gender.Other,
                Email: txtEmail.Text,
                PhoneNumber: txtPhoneNumber.Text,
                HashedPassword: txtHashedPassword.Text,
                AvatarPath: string.Empty,
                Address: txtAddress.Text,
                Status: cmbStatus.SelectedItem is ComboBoxItem selectedStatus
                    ? Enum.TryParse<UserStatus>(selectedStatus.Content as string, out var status)
                        ? status
                        : UserStatus.Active
                    : UserStatus.Active
                );

            if (request.Id == 0)
            {
                this.CreateUser(request);
                lblMessage.Content = "User Added"; 
            }
            else
            {
                this.UpdateUser(request);
                lblMessage.Content = "User Updated";
            }

            txtFullName.Text = string.Empty;
            dpBirthYear.SelectedDate = null;
            cmbGender.SelectedIndex = -1;
            txtEmail.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtHashedPassword.Text = string.Empty;
            txtAddress.Text = string.Empty;
            cmbStatus.SelectedIndex = -1;
        }

        void btnEditUser(object sender, RoutedEventArgs e)
        {
            User? user = ((FrameworkElement)sender).DataContext as User;
            txtUserId.Text = user?.Id.ToString();
            txtFullName.Text = user?.FullName;
            dpBirthYear.SelectedDate = user?.BirthYear;
            cmbGender.SelectedItem = user?.Gender;
            txtEmail.Text = user?.Email;
            txtPhoneNumber.Text = user?.PhoneNumber;
            txtAddress.Text = user?.Address;
            cmbStatus.SelectedItem = user?.Status;
        }
    }
}

