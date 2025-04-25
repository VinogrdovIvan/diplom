using System.Windows;
using AdminPanel.Views;

namespace AdminPanel
{
    public partial class App : Application
    {
        public static string Token { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FrameworkElement.StyleProperty.OverrideMetadata(
                typeof(Window),
                new FrameworkPropertyMetadata
                {
                    DefaultValue = FindResource(typeof(Window))
                });

            var loginWindow = new LoginWindow();
            loginWindow.Show();
        }
    }
}