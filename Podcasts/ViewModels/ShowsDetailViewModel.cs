using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Podcasts.Contracts.Services;
using Podcasts.Contracts.ViewModels;
using Windows.Web.Syndication;

namespace Podcasts.ViewModels;

public partial class ShowsDetailViewModel : ObservableRecipient, INavigationAware
{
    public SyndicationFeed? Feed
    {
        get; set;
    }

    private readonly IAudioPlayerService audioPlayerService;

    private readonly ShellViewModel shellViewModel;

    public ShowsDetailViewModel(IAudioPlayerService audioPlayerService, ShellViewModel shellViewModel)
    {
        this.audioPlayerService = audioPlayerService;
        this.shellViewModel = shellViewModel;
    }

    public void OnNavigatedTo(object parameter)
    {
        if (parameter is SyndicationFeed feed)
        {
            Feed = feed;
        }
    }

    public void OnNavigatedFrom()
    {
    }

    public void EpisodeListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.FirstOrDefault() is SyndicationItem item)
        {
            audioPlayerService.Show = Feed;
            audioPlayerService.Episode = item;
            audioPlayerService.Play();
            shellViewModel.IsPlaying = true;
        }
    }
}
