using CommunityToolkit.Mvvm.ComponentModel;
using Podcasts.Contracts.Services;
using Windows.Media.Playback;
using Windows.Web.Syndication;

namespace Podcasts.Services;
public partial class AudioPlayerService : ObservableObject, IAudioPlayerService, IDisposable
{
    private readonly MediaPlayer mediaPlayer = new();

    [ObservableProperty]
    private SyndicationFeed? show;

    private SyndicationItem? episode;

    public SyndicationItem? Episode
    {
        get => episode;
        set
        {
            if (value == null)
            {
                SetProperty(ref episode, null);
                return;
            }
            var uris =
                from link in value.Links
                where link.MediaType == "audio/mpeg"
                select link.Uri;
            if (!uris.Any())
            {
                SetProperty(ref episode, null);
                return;
            }
            if (uris.First() is Uri uri)
            {
                mediaPlayer.SetUriSource(uri);
                SetProperty(ref episode, value);
            }
        }
    }

    public void Play()
    {
        mediaPlayer.Play();
    }

    public void Pause()
    {
        mediaPlayer.Pause();
    }

    public void SkipBackward()
    {
        mediaPlayer.Position -= TimeSpan.FromSeconds(10);
    }

    public void SkipForward()
    {
        mediaPlayer.Position += TimeSpan.FromSeconds(30);
    }

    public void Dispose()
    {
        mediaPlayer.Dispose();
        GC.SuppressFinalize(this);
    }
}
