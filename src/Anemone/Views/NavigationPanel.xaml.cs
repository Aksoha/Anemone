using System;
using System.Collections.Generic;
using System.Windows;
using Anemone.Core;

namespace Anemone.Views;

public partial class NavigationPanel
{
    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(IEnumerable<NavigationPanelItem>),
        typeof(NavigationPanel)
    );


    public static readonly DependencyProperty SelectedPanelItemProperty = DependencyProperty.Register(
        nameof(SelectedPanelItem),
        typeof(NavigationPanelItem),
        typeof(NavigationPanel),
        new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty IsExpandedProperty = DependencyProperty.Register(
        nameof(IsExpanded),
        typeof(bool),
        typeof(NavigationPanel),
        new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public NavigationPanel()
    {
        InitializeComponent();
        HamburgerButton.IsChecked = IsExpanded;
        HamburgerButton.Unchecked += HamburgerButtonOnUnchecked;
        HamburgerButton.Checked += HamburgerButtonOnChecked;
    }

    public IEnumerable<NavigationPanelItem> ItemsSource
    {
        get => (IEnumerable<NavigationPanelItem>)GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }


    public NavigationPanelItem SelectedPanelItem
    {
        get => (NavigationPanelItem)GetValue(SelectedPanelItemProperty);
        set => SetValue(SelectedPanelItemProperty, value);
    }

    public bool IsExpanded
    {
        get => (bool)GetValue(IsExpandedProperty);
        set => SetValue(IsExpandedProperty, value);
    }

    public event EventHandler? Expanded;
    public event EventHandler? Collapsed;
    public event EventHandler? SelectedItemChanged;

    private void HamburgerButtonOnUnchecked(object sender, RoutedEventArgs e)
    {
        Expanded?.Invoke(this, e);
    }

    private void HamburgerButtonOnChecked(object sender, RoutedEventArgs e)
    {
        Collapsed?.Invoke(this, e);
    }

    private void SelectionChanged(object sender, RoutedEventArgs e)
    {
        SelectedItemChanged?.Invoke(this, e);
    }
}