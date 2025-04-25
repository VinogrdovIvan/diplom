using AdminPanel.ViewModels;
using System.Windows;

namespace AdminPanel.Views
{
    public partial class AddDriverWindow : Window
    {
        public AddDriverWindow()
        {
            InitializeComponent();
            DataContext = new AddDriverViewModel();
        }
    }
}