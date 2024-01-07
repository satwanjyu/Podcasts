using CommunityToolkit.WinUI.UI.Controls;

using Microsoft.UI.Xaml.Controls;

using Podcasts.ViewModels;

namespace Podcasts.Views;

public sealed partial class EpisodesPage : Page
{
    public EpisodesViewModel ViewModel
    {
        get;
    }

    public EpisodesPage()
    {
        ViewModel = App.GetService<EpisodesViewModel>();
        InitializeComponent();
    }

    private void OnViewStateChanged(object sender, ListDetailsViewState e)
    {
        if (e == ListDetailsViewState.Both)
        {
            ViewModel.EnsureItemSelected();
        }
    }
}
