using Avalonia.Controls;
using System;
using Avalonia;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using ti_Lyricstudio.Models;
using ti_Lyricstudio.ViewModels;

namespace ti_Lyricstudio.Views.Controls;

public partial class Editor : UserControl
{
    // view model of the editor
    private EditorViewModel? viewModel;

    // width and height of the current view
    private double _actualViewHeight;
    private double _viewWidth;

    public Editor()
    {
        InitializeComponent();
    }
    
    private void Editor_AttachedToVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        PlayerUIWindow root = e.Root as PlayerUIWindow;
        root.AddHandler(PointerPressedEvent, Editor_BackgroundPressed, RoutingStrategies.Tunnel);
    }

    private void Editor_DetachedFromVisualTree(object? sender, VisualTreeAttachmentEventArgs e)
    {
        PlayerUIWindow root = e.Root as PlayerUIWindow;
        root.RemoveHandler(PointerPressedEvent, Editor_BackgroundPressed);
    }

    private void Editor_BackgroundPressed(object? sender, PointerPressedEventArgs e)
    {
        // ignore if viewModel or lyrics are not initialized
        if (viewModel == null || viewModel.FlattenedLyrics == null || viewModel.SelectedLines == null) return;
        
        //
        if (this.IsVisualAncestorOf(e.Source as Visual)) return;
        
        viewModel.SelectedLines.Clear();
    }

    private void Editor_DataContextChanged(object? sender, EventArgs e)
    {
        // unsubscribe from old view model
        if (viewModel != null)
            viewModel.PropertyChanged -= ViewModel_PropertyChanged;

        // get view model of current editor
        viewModel = DataContext as EditorViewModel ?? throw new MemberAccessException("Failed to load view model.");
        viewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void ViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(EditorViewModel.ActiveLineIndex))
            UpdateScrollPosition();
    }

    private void UpdateScrollPosition()
    {
        int idx = viewModel.ActiveLineIndex;
        double lh = viewModel.LineHeight;
        double newPos = idx < 0 ? 0 : (idx * lh) + (lh / 2) - (_actualViewHeight / 2);
        EditorScroll.Offset = new Avalonia.Vector(0, newPos);
    }

    private void Editor_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        // ignore if viewModel is not initialized
        if (viewModel == null) return;

        // save current size to variable
        _actualViewHeight = EditorScroll.Bounds.Height;
        _viewWidth = EditorView.Bounds.Width;

        // update line width to scale with view width
        viewModel.MaxLineWidth = _viewWidth - 100;
    }

    // switch to Select mode when user tapped the line (in View/Play mode)
    private void EditorLine_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        // ignore if viewModel or lyrics are not initialized
        if (viewModel == null || viewModel.FlattenedLyrics == null || viewModel.SelectedLines == null) return;

        if (sender is Border current) {
            // get index of current line
            int index = EditorView.GetElementIndex(current);

            // ignore when index is not valid
            if (index == -1) return;

            // ignore when index is already selected
            if (viewModel.FlattenedLyrics[index].IsSelected == true) return;

            // set selected lines index to current line
            viewModel.SelectedLines.Clear();
            viewModel.SelectedLines.Add(index);
        }
    }

    // switch to Edit mode when user double-tapped the line (in Select mode)
    private void EditorLine_DoubleTapped(object? sender, Avalonia.Input.TappedEventArgs e)
    {
        // ignore if viewModel is not initialized
        if (viewModel == null) return;

        if (sender is Border current)
        {
            // get index of current line
            int index = EditorView.GetElementIndex(current);

            // ignore when index is not valid
            if (index == -1) return;

            viewModel.FlattenedLyrics[index].LineState = LyricLineState.Editing;
        }
    }
}