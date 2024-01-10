using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using Podcasts.Contracts.Services;
using Podcasts.Contracts.ViewModels;
using Windows.Web.Syndication;

namespace Podcasts.ViewModels;

public partial class ShowsViewModel : ObservableRecipient, INavigationAware
{
    private readonly INavigationService _navigationService;

    public ObservableCollection<SyndicationFeed> Source { get; } = new ObservableCollection<SyndicationFeed>();

    public ShowsViewModel(INavigationService navigationService)
    {
        _navigationService = navigationService;
    }

    public void OnNavigatedTo(object parameter)
    {
        Source.Clear();
    }

    public void OnNavigatedFrom()
    {
    }

    public async Task FollowShow(string showUrl)
    {
        var client = new SyndicationClient();
        var feed = await client.RetrieveFeedAsync(new Uri(showUrl));
        Source.Add(feed);
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
