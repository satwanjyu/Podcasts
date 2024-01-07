﻿using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

using Podcasts.Core.Models;

namespace Podcasts.Views;

public sealed partial class EpisodesDetailControl : UserControl
{
    public SampleOrder? ListDetailsMenuItem
    {
        get => GetValue(ListDetailsMenuItemProperty) as SampleOrder;
        set => SetValue(ListDetailsMenuItemProperty, value);
    }

    public static readonly DependencyProperty ListDetailsMenuItemProperty = DependencyProperty.Register("ListDetailsMenuItem", typeof(SampleOrder), typeof(EpisodesDetailControl), new PropertyMetadata(null, OnListDetailsMenuItemPropertyChanged));

    public EpisodesDetailControl()
    {
        InitializeComponent();
    }

    private static void OnListDetailsMenuItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is EpisodesDetailControl control)
        {
            control.ForegroundElement.ChangeView(0, 0, 1);
        }
    }
}
