using Microsoft.Extensions.Logging;
using ProyectoFinal_Biblioteca.ViewModel;

namespace ProyectoFinal_Biblioteca
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

            //builder.Services.AddSingleton<LibraryPage>();
            builder.Services.AddSingleton<LibraryViewModel>();

            //builder.Services.AddTransient<StatisticsPage>();
            builder.Services.AddTransient<StatisticsViewModel>();
            return builder.Build();
        }
    }
}
