using System.Collections.ObjectModel;
using KeepAll.Core.Abstractions;
using KeepAll.Core.Models;

namespace KeepAll.App.Pages;

public partial class SearchPage : ContentPage
{
    private readonly IItemRepository _repo;
    public ObservableCollection<Item> Items { get; } = new();

    public SearchPage(IItemRepository repo)
    {
        InitializeComponent();
        _repo = repo;
        BindingContext = this;
    }

    private async void OnSearchChanged(object sender, TextChangedEventArgs e)
    {
        Items.Clear();
        var q = e.NewTextValue?.Trim();
        if (string.IsNullOrEmpty(q)) return;

        foreach (var it in await _repo.SearchAsync(q))
            Items.Add(it);
    }

    private async void OnOpenClicked(object sender, EventArgs e)
    {
        if ((sender as Button)?.BindingContext is Item it && !string.IsNullOrWhiteSpace(it.Url))
            await Launcher.OpenAsync(it.Url);
    }
}
