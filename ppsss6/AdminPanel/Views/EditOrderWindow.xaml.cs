using AdminPanel.Models;
using AdminPanel.ViewModels;
using System.Windows;

namespace AdminPanel.Views
{
    public partial class EditOrderWindow : Window
    {
        public EditOrderWindow(Order order)
        {
            InitializeComponent();
            DataContext = new EditOrderViewModel(order);
        }
    }
}