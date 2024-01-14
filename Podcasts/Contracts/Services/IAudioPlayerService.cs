using Windows.Web.Syndication;

namespace Podcasts.Contracts.Services;
public interface IAudioPlayerService
{
    public SyndicationFeed? Show
    {
        get; set;
    }

    public SyndicationItem? Episode
    {
        get; set;
    }

    public void Play();

    public void Pause();

    public void SkipBackward();

    public void SkipForward();

    public void Dispose();
}
