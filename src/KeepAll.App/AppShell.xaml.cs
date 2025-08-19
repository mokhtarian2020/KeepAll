namespace KeepAll.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
    }

    private async void OnSettingsClicked(object sender, EventArgs e)
    {
        await Navigation.PushModalAsync(new Pages.SettingsPage(Handler?.MauiContext?.Services.GetRequiredService<KeepAll.Core.Abstractions.IItemRepository>()!));
    }
}
