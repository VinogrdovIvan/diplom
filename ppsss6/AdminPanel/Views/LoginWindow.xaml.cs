using AdminPanel.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
            DataContext = new LoginViewModel();
            //PasswordBox.PasswordChanged += PasswordBox_PasswordChanged;
        }

        //private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        //{
        //    if (DataContext is LoginViewModel viewModel)
        //    {
        //        viewModel.Password = ((PasswordBox)sender).Password;
        //    }
        //}
    }
}