using Avalonia.Controls;
using System;
using ti_Lyricstudio.ViewModels.Editor;

namespace ti_Lyricstudio.Views.Controls.Editor;

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
        viewModel.ViewHeight = EditorView.Bounds.Height;
    }
}