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
        }

    }
}