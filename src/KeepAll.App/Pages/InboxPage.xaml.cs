using System.Collections.ObjectModel;
using KeepAll.Core.Abstractions;
using KeepAll.Core.Models;

namespace KeepAll.App.Pages;

public partial class InboxPage : ContentPage
{
    private readonly IItemRepository _repo;

    public ObservableCollection<Item> Items { get; } = new();

    public InboxPage(IItemRepository repo)
    {
        InitializeComponent();
        _repo = repo;
        BindingContext = this;
        Loaded += async (_, __) => await RefreshAsync();
    }

    private async Task RefreshAsync()
    {
        // Import any pending shared items first
        await ImportPendingSharedItems();
        
        Items.Clear();
        foreach (var it in await _repo.GetAllAsync())
            Items.Add(it);
    }

    private async Task ImportPendingSharedItems()
    {
        var key = "pending_shared_items";
        var pending = Preferences.Get(key, "");
        if (!string.IsNullOrWhiteSpace(pending))
        {
            var lines = pending.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines)
            {
                var isUrl = Uri.TryCreate(line.Trim(), UriKind.Absolute, out var u);
                var item = new Item { 
                    Title = isUrl ? "(Shared Link)" : line.Trim(), 
                    Url = isUrl ? line.Trim() : "", 
                    Type = ItemType.Link,
                    SourceApp = "Shared"
                };
                await _repo.UpsertAsync(item);
            }
            Preferences.Set(key, ""); // Clear the queue
        }
    }

    private async void OnAddClicked(object sender, EventArgs e)
    {
        var title = await DisplayPromptAsync("Quick Add", "Title (or paste URL):");
        if (string.IsNullOrWhiteSpace(title)) return;

        var isUrl = Uri.TryCreate(title, UriKind.Absolute, out var u);
        var item = new Item { Title = isUrl ? "(Link)" : title, Url = isUrl ? title : "", Type = isUrl ? ItemType.Link : ItemType.Link };
        await _repo.UpsertAsync(item);
        await RefreshAsync();
        await DisplayAlert("Saved", "Saved to Inbox.", "OK");
    }

    private async void OnOpenClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Item it && !string.IsNullOrWhiteSpace(it.Url))
            await Launcher.OpenAsync(it.Url);
    }

    private async void OnMoreClicked(object sender, EventArgs e)
    {
        if ((sender as ImageButton)?.BindingContext is not Item it) return;
        var action = await DisplayActionSheet("Item", "Cancel", null, "Mark Done", "Edit", "Delete");
        if (action == "Mark Done")
        {
            it.Status = ItemStatus.Done;
            it.CompletedAt = DateTime.UtcNow;
            await _repo.UpsertAsync(it);
            await RefreshAsync();
        }
        else if (action == "Delete")
        {
            await _repo.DeleteAsync(it.Id);
            await RefreshAsync();
        }
        else if (action == "Edit")
        {
            var nt = await DisplayPromptAsync("Edit title", "Title:", initialValue: it.Title);
            if (!string.IsNullOrWhiteSpace(nt)) { it.Title = nt; await _repo.UpsertAsync(it); await RefreshAsync(); }
        }
    }

    private async void OnSearchChanged(object sender, TextChangedEventArgs e)
    {
        var q = e.NewTextValue?.Trim();
        if (string.IsNullOrEmpty(q)) { await RefreshAsync(); return; }
        Items.Clear();
        foreach (var it in await _repo.SearchAsync(q))
            Items.Add(it);
    }
}
