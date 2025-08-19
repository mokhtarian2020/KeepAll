using System.Text.Json;
using KeepAll.Core.Abstractions;

namespace KeepAll.App.Pages;

public partial class SettingsPage : ContentPage
{
    private readonly IItemRepository _repo;

    public SettingsPage(IItemRepository repo)
    {
        InitializeComponent();
        _repo = repo;
    }

    private async void OnExport(object sender, EventArgs e)
    {
        var items = await _repo.GetAllAsync();
        var json = JsonSerializer.Serialize(items, new JsonSerializerOptions { WriteIndented = true });
        await Share.RequestAsync(new ShareTextRequest
        {
            Title = "KeepAll Export",
            Text = json
        });
    }
}
