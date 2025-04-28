using System.Windows;
using AdminPanel.Views;

namespace AdminPanel
{
    public partial class App : Application
    {
        public static string Token { get; set; }

        public static void ClearTokenAndReturnToLogin()
        {
            Token = null;
            Current?.Windows.OfType<MainWindow>().FirstOrDefault()?.Close();
            new LoginWindow().Show();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FrameworkElement.StyleProperty.OverrideMetadata(
                typeof(Window),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = FindResource(typeof(Window))
                });

            new LoginWindow().Show();
        }
    }
}