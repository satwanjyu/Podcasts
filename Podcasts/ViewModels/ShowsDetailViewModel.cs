using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Podcasts.Contracts.ViewModels;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Web.Syndication;

namespace Podcasts.ViewModels;

public partial class ShowsDetailViewModel : ObservableRecipient, INavigationAware
{

    [ObservableProperty]
    private SyndicationFeed? feed;

    private readonly MediaPlayer mediaPlayer;

    public ShowsDetailViewModel()
    {
        mediaPlayer = App.MediaPlayer;
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
            var uris =
                from link in item.Links
                where link.MediaType == "audio/mpeg"
                select link.Uri;
            if (uris.FirstOrDefault() is Uri uri)
            {
                mediaPlayer.Source = MediaSource.CreateFromUri(uri);
                mediaPlayer.Play();
            }
        }
    }
}
