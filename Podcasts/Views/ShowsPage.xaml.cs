using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Podcasts.ViewModels;

namespace Podcasts.Views;

public sealed partial class ShowsPage : Page
{
    public ShowsViewModel ViewModel
    {
        get;
    }

    public ShowsPage()
    {
        ViewModel = App.GetService<ShowsViewModel>();
        InitializeComponent();
    }

    private async void FollowShow_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new FollowShowDialog
        {
            XamlRoot = XamlRoot
        };
        var result = await dialog.ShowAsync();
        if (result == ContentDialogResult.Primary)
        {
            await ViewModel.FollowShow(dialog.Url);
        }
    }
}
