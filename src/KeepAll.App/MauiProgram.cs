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
        builder.Services.AddSingleton(new SqliteItemRepository(dbPath));
        builder.Services.AddSingleton<IItemRepository>(sp =>
        {
            var repo = sp.GetRequiredService<SqliteItemRepository>();
            repo.InitAsync().GetAwaiter().GetResult();
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
