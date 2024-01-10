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

    private async void OnFollowClick(object sender, RoutedEventArgs e)
    {
        var showUrl = ShowUrlBox.Text;
        // TODO: catch exceptions from XmlReader
        await ViewModel.FollowShow(showUrl);
    }
}
