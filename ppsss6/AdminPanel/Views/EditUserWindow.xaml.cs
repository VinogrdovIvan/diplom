using AdminPanel.Models;
using AdminPanel.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace AdminPanel.Views
{
    public partial class EditUserWindow : Window
    {
        public EditUserWindow(User user)
        {
            InitializeComponent();
            DataContext = new EditUserViewModel(user);
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext is EditUserViewModel vm)
            {
                vm.User.Password = ((PasswordBox)sender).Password;
            }
        }
    }
}