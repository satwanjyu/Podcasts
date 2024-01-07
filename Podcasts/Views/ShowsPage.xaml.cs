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
}
