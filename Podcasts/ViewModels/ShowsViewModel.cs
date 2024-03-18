using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Podcasts.Contracts.Services;
using Podcasts.Contracts.ViewModels;
using Podcasts.Services;
using Windows.Web.Syndication;

namespace Podcasts.ViewModels;

public partial class ShowsViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;

    private readonly SqliteDataService sqliteDataService;

    public ObservableCollection<SyndicationFeed> Source { get; } = new ObservableCollection<SyndicationFeed>();

    public ShowsViewModel(INavigationService navigationService, SqliteDataService sqliteDataService)
    {
        _navigationService = navigationService;
        this.sqliteDataService = sqliteDataService;
    }

    public async void OnNavigatedTo(object parameter)
    {
        Source.Clear();
        while (sqliteDataService.connection == null)
        {
            await Task.Delay(100);
        }
        var feeds = await sqliteDataService.GetFeeds(sqliteDataService.connection!);
        foreach (var feed in feeds)
        {
            Source.Add(feed);
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public async Task FollowShow(string showUrl)
    {
        var client = new SyndicationClient();
        var feed = await client.RetrieveFeedAsync(new Uri(showUrl));
        Source.Add(feed);
        await sqliteDataService.InsertFeed(sqliteDataService.connection!, feed, showUrl);
    }

    [RelayCommand]
    private void OnItemClick(SyndicationFeed? clickedItem)
    {
        if (clickedItem != null)
        {
            _navigationService.SetListDataItemForNextConnectedAnimation(clickedItem);
            _navigationService.NavigateTo(typeof(ShowsDetailViewModel).FullName!, clickedItem);
        }
    }
}
