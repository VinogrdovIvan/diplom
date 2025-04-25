using AdminPanel.ViewModels;
using System.Windows;

namespace AdminPanel.Views
{
    public partial class AddCarWindow : Window
    {
        public AddCarWindow()
        {
            InitializeComponent();
            DataContext = new AddCarViewModel();
        }
    }
}