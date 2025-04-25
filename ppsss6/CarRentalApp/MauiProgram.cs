using CarRentalApp.Handlers;
using CarRentalApp.Services;
using CarRentalApp.ViewModels;
using CarRentalApp.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CarRentalApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
            builder.Logging.AddDebug();
#endif

            builder.Services
                .AddTransient<AuthorizationHeaderHandler>()
                .AddHttpClient("Backend")
                .ConfigureHttpClient(client => client.BaseAddress = new Uri("http://10.0.2.2:5299/"))
                .AddHttpMessageHandler<AuthorizationHeaderHandler>();

            builder.Services.AddTransient<IAuthService, AuthService>();
            builder.Services.AddTransient<ICarService, CarService>();
            builder.Services.AddTransient<IOrderService, OrderService>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<HomeViewModel>(); 
            builder.Services.AddTransient<OrdersViewModel>();
            builder.Services.AddTransient<ProfileViewModel>();
            builder.Services.AddTransient<EditProfileViewModel>();
            builder.Services.AddTransient<CalculateCostViewModel>();

            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<RegisterPage>();
            builder.Services.AddTransient<HomePage>(); 
            builder.Services.AddTransient<OrdersPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<EditProfilePage>();
            builder.Services.AddTransient<CalculateCostPage>();

            return builder.Build();
        }
    }
}