using Microsoft.Extensions.Logging;
using DayNote.Services;
namespace DayNote
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
                });

            builder.Services.AddMauiBlazorWebView();
            builder.Services.AddSingleton<JournalDatabase>();
            builder.Services.AddSingleton<JournalService>();
            builder.Services.AddSingleton<FeatureService>();
            builder.Services.AddSingleton<ThemeService>();
            builder.Services.AddSingleton<PinRepository>();
            builder.Services.AddSingleton<LockService>();
            builder.Services.AddSingleton<PdfExportService>();


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
