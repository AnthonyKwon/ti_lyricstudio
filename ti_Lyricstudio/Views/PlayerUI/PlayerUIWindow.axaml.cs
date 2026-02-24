using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ti_Lyricstudio.ViewModels;

namespace ti_Lyricstudio.Views;

public partial class PlayerUIWindow : Window
{
    // title of the application
    private readonly string appName = "ti:Lyricstudio";
    private string? fileName;

    BindingExpressionBase? subscription;

    public PlayerUIWindow()
    {
        InitializeComponent();

        // register open file by pointer pressed(clicked) event
        AddHandler(PointerPressedEvent, FileOpen_Click, RoutingStrategies.Direct | RoutingStrategies.Tunnel);

    }

    /// <summary>
    /// Close and dispose the current workspace.
    /// </summary>
    /// <param name="editorOnly">Only dispose the EditorView instance. (false by default)</param>
    /// <returns></returns>
    /// <exception cref="MemberAccessException"></exception>
        // close current workspace
    public async Task<bool> CloseWorkspace(bool editorOnly = false)
    {
        // get view model of current window
        PlayerUIWindowViewModel viewModel = DataContext as PlayerUIWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");

        // ask user to continue if file was opened and modified
        if (viewModel.FileOpened())
        {
            if (viewModel.FileModified())
            {
                // ask user to continue
                IMsBox<ButtonResult> box = MessageBoxManager.GetMessageBoxStandard("File modified",
                    "File has been modified. Are you sure to continue without saving?",
                    ButtonEnum.YesNo);
                ButtonResult result = await box.ShowAsync();

                // return false if user refused to close workspace
                if (result != ButtonResult.Yes) return false;
            }

            // unbind the source from EditorView
            subscription?.Dispose();

            // EditorView disposed; stop here if user asked to dispose EditorView only
            if (editorOnly) return true;

            // close current workspace
            viewModel.CloseFile();

            // reset the window to initial state
            MainView.IsVisible = false;
            OpenFileGuide.IsVisible = true;

            // register open file by pointer pressed(clicked) event
            AddHandler(PointerPressedEvent, FileOpen_Click, RoutingStrategies.Direct | RoutingStrategies.Tunnel);

            // reset the application title
            Title = appName;

            // file sucessfully closed; exit with truthy value
            return true;
        }

        // file is not opened; exit with truthy value
        return true;
    }

    // UI interaction on user tries to open file
    // check if current workspace is modified and open file dialog
    public async void FileOpen_Click(object? sender, RoutedEventArgs e)
    {
        // get view model of current window
        PlayerUIWindowViewModel viewModel = DataContext as PlayerUIWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");

        // close the current workspace
        if (!await CloseWorkspace()) return;

        // define audio file type that app can use
        FilePickerFileType AudioAll = new("All Audios")
        {
            Patterns = ["*.ape", "*.flac", "*.m4a", "*.mp3", "*.oga", "*.ogg", "*.opus", "*.wav", "*.wma"],
            AppleUniformTypeIdentifiers = ["mp3", "aiff", "wav", "midi", "mpeg4Audio"],
            MimeTypes = ["audio/*"]
        };

        // configure options for file picker
        FilePickerOpenOptions openOptions = new()
        {
            Title = "Open workspace...",
            FileTypeFilter = [AudioAll, FilePickerFileTypes.All],
            AllowMultiple = false
        };

        // open file picker and get result file
        IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(openOptions);

        if (files.Count >= 1)
        {
            // try to open the file
            viewModel.OpenFile(files[0].TryGetLocalPath() ?? throw new FileNotFoundException("Application is not able to get path of the selected file."));

            // allocate DataContext to Player and make it visible
            OpenFileGuide.IsVisible = false;
            MainView.IsVisible = true;

            // unregister open file by pointer pressed(clicked) event
            RemoveHandler(PointerPressedEvent, FileOpen_Click);

            // set fileName as current file
            fileName = Path.ChangeExtension(files[0].Name, ".lrc");

            // update title as current workspace
            Title = $"{appName} :: {fileName}";
        }
    }
}