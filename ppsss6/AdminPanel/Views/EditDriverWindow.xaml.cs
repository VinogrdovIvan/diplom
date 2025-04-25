using AdminPanel.Models;
using AdminPanel.ViewModels;
using System.Windows;

namespace AdminPanel.Views
{
    public partial class EditDriverWindow : Window
    {
        public EditDriverWindow(Driver driver)
        {
            InitializeComponent();
            DataContext = new EditDriverViewModel(driver);
        }
    }
}