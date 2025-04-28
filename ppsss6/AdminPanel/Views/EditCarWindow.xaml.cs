using AdminPanel.Models;
using AdminPanel.ViewModels;
using System.Windows;

namespace AdminPanel.Views
{
    public partial class EditCarWindow : Window
    {
        public EditCarWindow(Car car)
        {
            InitializeComponent();
            DataContext = new EditCarViewModel(car);
        }
    }
}