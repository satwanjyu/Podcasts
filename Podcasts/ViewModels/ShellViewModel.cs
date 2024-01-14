using CommunityToolkit.Mvvm.ComponentModel;

using Microsoft.UI.Xaml.Navigation;

using Podcasts.Contracts.Services;
using Podcasts.Views;

namespace Podcasts.ViewModels;

public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private object? selected;

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    public IAudioPlayerService? AudioPlayerService
    {
        get; set;
    }

    [ObservableProperty]
    private bool isPlaying = false;

    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService, IAudioPlayerService audioPlayerService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
        AudioPlayerService = audioPlayerService;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        if (e.SourcePageType == typeof(SettingsPage))
        {
            Selected = NavigationViewService.SettingsItem;
            return;
        }

        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }

    public void PlayPause()
    {
        if (AudioPlayerService is null)
        {
            return;
        }
        if (IsPlaying)
        {
            AudioPlayerService.Pause();
            IsPlaying = false;
        }
        else
        {
            AudioPlayerService.Play();
            IsPlaying = true;
        }
    }
}
