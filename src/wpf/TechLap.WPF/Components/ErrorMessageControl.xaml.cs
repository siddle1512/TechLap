using System.Windows;
using System.Windows.Controls;

namespace TechLap.WPF.Components
{
    /// <summary>
    /// Interaction logic for ErrorMessageControl.xaml
    /// </summary>
    public partial class ErrorMessageControl : UserControl
    {
        public ErrorMessageControl()
        {
            InitializeComponent();
        }

        public void ShowError(string message)
        {
            txtErrorMessage.Text = message;
            this.Visibility = Visibility.Visible;
        }

        public void HideError()
        {
            this.Visibility = Visibility.Collapsed;
        }
    }
}
