using AdminPanel.Models;
using AdminPanel.ViewModels;
using System.Windows;

namespace AdminPanel.Views
{
    public partial class EditUserWindow : Window
    {
        public EditUserWindow(User user)
        {
            InitializeComponent();
            DataContext = new EditUserViewModel(user);
        }
    }
}