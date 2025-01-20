using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Base;
using MsBox.Avalonia.Enums;
using ti_Lyricstudio.ViewModels;

namespace ti_Lyricstudio.Views
{
    public partial class MainWindow : Window
    {
        // marker to check if file is opened
        private bool opened = false;
        // marker to check if file has modified
        private bool modified = false;

        BindingExpressionBase subscription;

        public MainWindow()
        {
            InitializeComponent();

            // attach developer tools in Debug build
#if DEBUG
            this.AttachDevTools();
#endif

            EditorView.AddHandler(KeyDownEvent, EditorView_KeyDown, RoutingStrategies.Direct | RoutingStrategies.Tunnel);
        }

        // UI interaction on "Open" button clicked
        // check if current workspace is modified and open file dialog
        public async void OpenMenu_Click(object? sender, RoutedEventArgs e)
        {
            // ask user to continue if file was opened and modified
            if (opened == true || modified == true)
            {
                if (modified == true)
                {
                    // ask user to continue
                    IMsBox<ButtonResult> box = MessageBoxManager.GetMessageBoxStandard("File modified", 
                        "File has been modified. Are you sure to continue without saving?",
                        ButtonEnum.YesNo);
                    ButtonResult result = await box.ShowAsync();
                    if (result != ButtonResult.Yes) return;
                }

                // close current workspace
                (DataContext as MainWindowViewModel).CloseFile();

                // unbind the source from EditorView
                EditorView.Source = null;
                subscription.Dispose();

                // reset the window to initial state
                Player.IsVisible = false;
                Preview.IsVisible = false;
            }

            // define audio file type that app can use
            //TODO: define AppleUniformTypeIdentifiers
            FilePickerFileType AudioAll = new("All Audios")
            {
                Patterns = ["*.alac", "*.ape", "*.flac", "*.m4a", "*.mp3", "*.oga", "*.ogg", "*.opus", "*.wav", "*.wma"],
                MimeTypes = ["audio/*"]
            };

            // configure options for file picker
            FilePickerOpenOptions openOptions = new();
            openOptions.Title = "Open workspace...";
            openOptions.FileTypeFilter = [AudioAll, FilePickerFileTypes.All];
            openOptions.AllowMultiple = false;

            // open file picker and get result file
            IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(openOptions);

            if (files.Count >= 1)
            {
                // get view model of current window
                MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
                
                // try to open the file
                viewModel.OpenFile(files[0].TryGetLocalPath());

                // manually update the EditorView
                EditorView.Source = null;
                EditorView.Source = viewModel.LyricsGridSource;

                // allocate DataContext to Player and make it visible
                Player.IsVisible = true;
                Preview.IsVisible = true;

                // mark as opened
                opened = true;

                // bind grid to its lyrics source
                subscription = EditorView.Bind(TreeDataGrid.SourceProperty, new Binding("LyricsGridSource"));
            }
        }

        // UI interaction on "Save" button clicked
        // save the current workspace if modified
        public void SaveMenu_Click(object? sender, RoutedEventArgs e)
        {
            // ignore request if file is not opened or modified
            if (opened != true || modified != true) return;

            // try to save the file
            (DataContext as MainWindowViewModel).SaveFile();

            // unmark workspace modified
            modified = false;
        }

        // UI interaction on user typed some text on EditorView
        // user has modified content; mark it
        private void EditorView_TextInput(object? sender, Avalonia.Input.TextInputEventArgs e)
        {
            if (opened == true) modified = true;
        }

        private void EditorView_KeyDown(object? sender, KeyEventArgs e)
        {
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;

            if (e.KeyModifiers == KeyModifiers.Shift && e.Key == Key.Delete)
            {
                // (default)Shift + Delete: delete the selected row
                viewModel.DeleteRow();

                // mark workspace as modified
                modified = true;
            }
            else if (e.Key == Key.Delete)
            {
                // (default)Delete: delete content of the selected cell
                viewModel.EmptyCell();

                // workaround: manually update the EditorView
                //     it has some issue with tracking LyricData.Text update, need to investigate
                EditorView.Source = null;
                EditorView.Source = viewModel.LyricsGridSource;

                // mark workspace as modified
                modified = true;
            }
            else if (e.Key == Key.Back)
            {
                // mark workspace as modified
                modified = true;
            }
        }

        private void Player_SetTimeClick(object? sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            viewModel.SetTime();

            // mark workspace as modified
            modified = true;

            // workaround: manually update the EditorView
            //     UI sometimes desynced when data updates too frequently
            EditorView.Source = null;
            EditorView.Source = viewModel.LyricsGridSource;
        }
    }
}