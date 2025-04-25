using AdminPanel.ViewModels;
using System.Windows;

namespace AdminPanel.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}