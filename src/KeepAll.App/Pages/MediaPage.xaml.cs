using System.Collections.ObjectModel;
using KeepAll.Core.Abstractions;
using KeepAll.Core.Models;

namespace KeepAll.App.Pages;

public partial class MediaPage : ContentPage
{
    private readonly IItemRepository _repo;
    public ObservableCollection<Item> Items { get; } = new();

    private ItemType _filter = ItemType.Book;

    public MediaPage(IItemRepository repo)
    {
        InitializeComponent();
        _repo = repo;
        BindingContext = this;
        Loaded += async (_, __) => await ApplyFilterAsync(_filter);
    }

    private async Task ApplyFilterAsync(ItemType type)
    {
        _filter = type;
        Items.Clear();
        var all = await _repo.GetAllAsync();
        foreach (var it in all.Where(i => i.Type == _filter))
            Items.Add(it);
    }

    private async void OnBooks(object s, EventArgs e) => await ApplyFilterAsync(ItemType.Book);
    private async void OnMovies(object s, EventArgs e) => await ApplyFilterAsync(ItemType.Movie);
    private async void OnPodcasts(object s, EventArgs e) => await ApplyFilterAsync(ItemType.Podcast);

    private async void OnOpenClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Item it && !string.IsNullOrWhiteSpace(it.Url))
            await Launcher.OpenAsync(it.Url);
    }
}
