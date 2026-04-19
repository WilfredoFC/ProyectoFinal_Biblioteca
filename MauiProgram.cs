using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using ProyectoFinal_Biblioteca.Services;
using ProyectoFinal_Biblioteca.ViewModels;
using ProyectoFinal_Biblioteca.Views;

namespace ProyectoFinal_Biblioteca
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseMauiCommunityToolkitMediaElement() // ← Método específico para el MediaElement
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

            // Servicios
            builder.Services.AddSingleton<DatabaseService>();
            builder.Services.AddSingleton<BookApiService>();

            // ViewModels
            builder.Services.AddTransient<LibraryViewModel>();
            builder.Services.AddTransient<SearchViewModel>();
            builder.Services.AddTransient<AddBookViewModel>();
            builder.Services.AddTransient<BookDetailViewModel>();
            builder.Services.AddTransient<StatisticsViewModel>();
            builder.Services.AddTransient<SearchViewModel>();

            // Pages
            builder.Services.AddTransient<LibraryPage>();
            builder.Services.AddTransient<SearchPage>();
            builder.Services.AddTransient<AddBookPage>();
            builder.Services.AddTransient<BookDetailPage>();
            builder.Services.AddTransient<StatisticsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}