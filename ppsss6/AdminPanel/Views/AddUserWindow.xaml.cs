using AdminPanel.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel.Views
{
    public partial class AddUserWindow : Window
    {
        public AddUserWindow()
        {
            InitializeComponent();
            DataContext = new AddUserViewModel();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddUserViewModel viewModel)
            {
                viewModel.Password = ((PasswordBox)sender).Password;
                UpdatePasswordStrength(((PasswordBox)sender).Password);
            }
        }

        private void UpdatePasswordStrength(string password)
        {
            if (password.Length == 0)
            {
                passwordStrengthText.Text = "";
                passwordStrengthBar.Value = 0;
            }
            else if (password.Length < 6)
            {
                passwordStrengthText.Text = "Слабый";
                passwordStrengthBar.Value = 33;
                passwordStrengthText.Foreground = System.Windows.Media.Brushes.Red;
            }
            else if (password.Length < 10)
            {
                passwordStrengthText.Text = "Средний";
                passwordStrengthBar.Value = 66;
                passwordStrengthText.Foreground = System.Windows.Media.Brushes.Orange;
            }
            else
            {
                passwordStrengthText.Text = "Сильный";
                passwordStrengthBar.Value = 100;
                passwordStrengthText.Foreground = System.Windows.Media.Brushes.Green;
            }
        }
    }
}