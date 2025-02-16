using System;
using System.Collections.Generic;
using System.IO;
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

        //
        private readonly string appName = "ti:Lyricstudio";
        private string? fileName;

        BindingExpressionBase subscription;

        public MainWindow()
        {
            InitializeComponent();

            // attach developer tools in Debug build
#if DEBUG
            this.AttachDevTools();
#endif

            // set initial title name
            Title = appName;

            // handle hotkey by keyboard press event
            AddHandler(KeyDownEvent, MainWindow_KeyDown, RoutingStrategies.Direct | RoutingStrategies.Tunnel);
        }

        private void MarkModified()
        {
            // ignore request if already mark modified
            if (opened == false || modified == true) return;

            modified = true;
            Title = $"{appName} — {fileName}*";
        }

        private void UnmarkModified()
        {
            // ignore request if not mark modified
            if (opened == false || modified == false) return;

            modified = false;
            Title = $"{appName} — {fileName}";
        }

        // UI interaction on "Open" menu item clicked
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

                    // unmark workspace modified
                    UnmarkModified();
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

                // set fileName as current file
                fileName = Path.ChangeExtension(files[0].Name, ".lrc");

                // update title as current workspace
                Title = $"{appName} — {fileName}";

                // bind grid to its lyrics source
                subscription = EditorView.Bind(TreeDataGrid.SourceProperty, new Binding("LyricsGridSource"));
            }
        }

        // UI interaction on "Save" menu item clicked
        // save the current workspace if modified
        public void SaveMenu_Click(object? sender, RoutedEventArgs e)
        {
            // ignore request if file is not opened or modified
            if (opened != true || modified != true) return;

            // try to save the file
            (DataContext as MainWindowViewModel).SaveFile();

            // unmark workspace modified
            UnmarkModified();
        }

        // UI interaction on "Quit" menu item clicked
        // confirm user if workspace is modified and exit the application
        public async void QuitMenu_Click(Object? sender, RoutedEventArgs e)
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

            Close();
        }

        // UI interaction on "Delete" menu item clicked
        // delete content of the selected cell
        public void DeleteMenu_Click(object? sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;

            // ignore request if file is not opened
            if (opened == false) return;

            // send source to delete the target data
            viewModel.EmptyCell();

            // workaround: manually update the EditorView
            //     it has some issue with tracking LyricData.Text update, need to investigate
            EditorView.Source = null;
            EditorView.Source = viewModel.LyricsGridSource;

            // mark workspace as modified
            MarkModified();
        }

        // UI interaction on "Delete Selected Row" menu item clicked
        // delete the selected row
        private void DeleteRowMenu_Click(object sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;

            // ignore request if file is not opened
            // hotkey below requires workspace already opened
            if (opened == false) return;

            // request to delete the row
            viewModel.DeleteRow();

            // mark workspace as modified
            MarkModified();
        }

        // UI interaction on user typed some text on EditorView
        // user has modified content; mark it
        private void EditorView_TextInput(object? sender, TextInputEventArgs e)
        {
            MarkModified();
        }

        // UI interaction on hotkey pressed
        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;

            // ignore request if file is not opened
            // hotkey below requires workspace already opened
            if (opened == false) return;
            
            if (e.KeyModifiers == KeyModifiers.Shift && e.Key == Key.A) 
            {
                // (default)Shift + A: move cell timestamp selection up
                viewModel.MoveTimeSelection(0);
            }
            else if (e.KeyModifiers == KeyModifiers.Shift && e.Key == Key.S)
            {
                // (default)Shift + S: move cell timestamp selection down
                viewModel.MoveTimeSelection(1);
            }
            else if (e.Key == Key.Back)
            {
                // mark workspace as modified
                MarkModified();
            }
        }

        private void Player_SetTimeClick(object? sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = (MainWindowViewModel)DataContext;
            viewModel.SetTime();

            // mark workspace as modified
            MarkModified();

            // workaround: manually update the EditorView
            //     UI sometimes desynced when data updates too frequently
            EditorView.Source = null;
            EditorView.Source = viewModel.LyricsGridSource;
        }
    }
}