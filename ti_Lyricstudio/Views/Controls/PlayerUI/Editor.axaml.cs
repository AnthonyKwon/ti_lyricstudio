using Avalonia.Controls;
using System;
using ti_Lyricstudio.ViewModels;

namespace ti_Lyricstudio.Views.Controls;

public partial class Editor : UserControl
{
    // view model of the editor
    private EditorViewModel viewModel;

    public Editor()
    {
        InitializeComponent();
    }

    private void Editor_DataContextChanged(object? sender, EventArgs e)
    {
        // get view model of current editor
        viewModel = DataContext as EditorViewModel ?? throw new MemberAccessException("Failed to load view model.");
    }

    private void Editor_SizeChanged(object? sender, SizeChangedEventArgs e)
    {
        // ignore if viewModel is not initialized
        if (viewModel == null) return;

        // update the new view height
        viewModel.ActualViewHeight = EditorScroll.Bounds.Height;
        viewModel.ViewWidth = EditorView.Bounds.Width;
        viewModel.ViewHeight = EditorView.Bounds.Height;

        // trigger the size changed event in ViewModel
        // change to actual event
        viewModel.Editor_SizeChanged();
    }

    private void Editor_PointerPressed(object? sender, Avalonia.Input.PointerPressedEventArgs e)
    {
        // set thickness of every border to 0
        Avalonia.Thickness newThickness = new(0);
        foreach (Border b in EditorView.Children)
            b.BorderThickness = newThickness;

        // set selected line to current line
        viewModel.SelectedLines.Clear();
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
            if (viewModel.FlattenedLyrics[index].Selected == true) return;

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
            int index = EditorView.Children.IndexOf(current);

            // ignore when index is not valid
            if (index == -1) return;

            System.Diagnostics.Debug.WriteLine(viewModel.FlattenedLyrics[index].Text);
        }
    }
}