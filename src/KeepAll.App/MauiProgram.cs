using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using KeepAll.Core.Abstractions;
using KeepAll.Metadata;
using KeepAll.Storage;

namespace KeepAll.App;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .ConfigureFonts(f =>
            {
                // If Inter fonts are not present, use system fonts; add later
                f.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                f.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // DI
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "keepall.db3");
        builder.Services.AddSingleton<IItemRepository>(sp =>
        {
            var repo = new SqliteItemRepository(dbPath);
            // Initialize database synchronously to avoid TypeInitialization errors
            try
            {
                repo.InitAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex}");
                // Continue with uninitialized repo - it will retry on first use
            }
            return repo;
        });
        builder.Services.AddSingleton<IMetadataService, MetadataService>();

        // VMs + Pages
        builder.Services.AddSingleton<Pages.InboxPage>();
        builder.Services.AddSingleton<Pages.MediaPage>();
        builder.Services.AddSingleton<Pages.SearchPage>();
        builder.Services.AddSingleton<Pages.SettingsPage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}
