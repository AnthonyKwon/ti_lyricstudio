using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
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
        // title of the application
        private readonly string appName = "ti:Lyricstudio";
        private string? fileName;

        BindingExpressionBase? subscription;

        public MainWindow()
        {
            InitializeComponent();

#if DEBUG
            // attach developer tools in Debug build
            this.AttachDevTools();
#endif

            // set initial title name
            Title = appName;

            // handle hotkey by keyboard press event
            AddHandler(KeyDownEvent, MainWindow_KeyDown, RoutingStrategies.Direct | RoutingStrategies.Tunnel);
        }

        private void LyricsDataChanged(object? sender, EventArgs e)
        {
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");

            // workaround: manually update the EditorView
            //     it has some issue with tracking LyricData.Text update, need to investigate
            EditorView.Source = null;
            EditorView.Source = viewModel.LyricsGridSource;

            Title = $"{appName} :: {fileName}*";
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
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");

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
                EditorView.Source = null;
                subscription?.Dispose();

                // EditorView disposed; stop here if user asked to dispose EditorView only
                if (editorOnly) return true;

                // close current workspace
                viewModel.CloseFile();

                // unsubscribe from event when lyrics data changed
                viewModel.DataChanged -= LyricsDataChanged;

                // reset the window to initial state
                Player.IsVisible = false;
                Preview.IsVisible = false;

                // reset the application title
                Title = appName;

                // file sucessfully closed; exit with truthy value
                return true;
            }

            // file is not opened; exit with truthy value
            return true;
        }

        // UI interaction on "Open" menu item clicked
        // check if current workspace is modified and open file dialog
        public async void OpenMenu_Click(object? sender, RoutedEventArgs e)
        {
            // get view model of current window
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");

            // close the current workspace
            if (!await CloseWorkspace()) return;

            // define audio file type that app can use
            //TODO: define AppleUniformTypeIdentifiers
            FilePickerFileType AudioAll = new("All Audios")
            {
                Patterns = ["*.alac", "*.ape", "*.flac", "*.m4a", "*.mp3", "*.oga", "*.ogg", "*.opus", "*.wav", "*.wma"],
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

                // manually update the EditorView
                EditorView.Source = null;
                EditorView.Source = viewModel.LyricsGridSource;

                // allocate DataContext to Player and make it visible
                Player.IsVisible = true;
                Preview.IsVisible = true;

                // subscribe to event when lyrics data changed
                viewModel.DataChanged += LyricsDataChanged;

                // set fileName as current file
                fileName = Path.ChangeExtension(files[0].Name, ".lrc");

                // update title as current workspace
                Title = $"{appName} :: {fileName}";

                // bind grid to its lyrics source
                subscription = EditorView.Bind(TreeDataGrid.SourceProperty, new Binding("LyricsGridSource"));
            }
        }

        // UI interaction on "Import >> From File..." menu item clicked
        // check if current workspace is modified and open file dialog
        public async void ImportFileMenu_Click(object? sender, RoutedEventArgs e)
        {
            // get view model of current window
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");

            // close the current workspace
            if (!await CloseWorkspace(true)) return;

            // define audio file type that app can use
            //TODO: define AppleUniformTypeIdentifiers
            FilePickerFileType LRCFile = new("LRC File")
            {
                Patterns = ["*.lrc"],
                AppleUniformTypeIdentifiers = ["public.plain-text"],
                MimeTypes = ["text/plain"]
            };

            // configure options for file picker
            FilePickerOpenOptions openOptions = new()
            {
                Title = "Import Lyrics...",
                FileTypeFilter = [LRCFile, FilePickerFileTypes.TextPlain, FilePickerFileTypes.All],
                AllowMultiple = false
            };

            // open file picker and get result file
            IReadOnlyList<IStorageFile> files = await StorageProvider.OpenFilePickerAsync(openOptions);

            if (files.Count >= 1)
            {
                // try to import the file
                viewModel.ImportFile(files[0].TryGetLocalPath() ?? throw new FileNotFoundException("Application is not able to get path of the selected file."));

                // manually update the EditorView
                EditorView.Source = null;
                EditorView.Source = viewModel.LyricsGridSource;

                // bind grid to its lyrics source
                subscription = EditorView.Bind(TreeDataGrid.SourceProperty, new Binding("LyricsGridSource"));
            }
        }

        // UI interaction on "Import >> From Clipboard" menu item clicked
        // check if current workspace is modified and run import from clipboard command
        public async void ImportClipboardMenu_Click(object? sender, RoutedEventArgs e)
        {
            // get view model of current window
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");

            // close the current workspace
            if (!await CloseWorkspace(true)) return;

            // try to import content from the clipboard
            viewModel.ImportClipboard();

            // manually update the EditorView
            EditorView.Source = null;
            EditorView.Source = viewModel.LyricsGridSource;

            // bind grid to its lyrics source
            subscription = EditorView.Bind(TreeDataGrid.SourceProperty, new Binding("LyricsGridSource"));
        }

        // UI interaction on "Export >> As File..." menu item clicked
        // check if current workspace is modified and open export as file dialog
        public async void ExportFileMenu_Click(object? sender, RoutedEventArgs e)
        {
            // get view model of current window
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");

            // define audio file type that app can use
            //TODO: define AppleUniformTypeIdentifiers
            FilePickerFileType LRCFile = new("LRC File")
            {
                Patterns = ["*.lrc"],
                AppleUniformTypeIdentifiers = ["public.plain-text"],
                MimeTypes = ["text/plain"]
            };

            // configure options for file picker
            FilePickerSaveOptions exportOptions = new()
            {
                Title = "Export Lyrics...",
                SuggestedFileName = fileName,
                DefaultExtension = "lrc"
            };

            // open save file picker and get result file
            IStorageFile file = await StorageProvider.SaveFilePickerAsync(exportOptions) ?? throw new FileNotFoundException("Application is not able to get path of the selected file.");

            // try to export the file
            viewModel.ExportFile(file.TryGetLocalPath() ?? throw new FileNotFoundException("Application is not able to get path of the selected file."));
        }

        // UI interaction on "Quit" menu item clicked
        // close the current workspace
        public async void CloseWorkspace_Click(object? sender, RoutedEventArgs e)
        {
            await CloseWorkspace();
        }

        // UI interaction on "Quit" menu item clicked
        // ask to close the application
        public void QuitMenu_Click(object? sender, RoutedEventArgs e) => Close();

        // Event on Application closing
        // confirm user if workspace is modified and exit the application
        private async void OnClosing(object? sender, WindowClosingEventArgs e)
        {
            // get view model of current window
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");

            // ask user to continue if file was opened and modified
            if (viewModel.FileModified())
            {
                // cancel the application close
                // this is required to run asyncronous job on OnClosing() function
                e.Cancel = true;

                // exit function when user request not to close
                if (!await CloseWorkspace()) return;

                // mark file as unmodified and retry to close the application
                viewModel.Modified = false;
                Close();
            }
        }

        // UI interaction on hotkey pressed
        private void MainWindow_KeyDown(object? sender, KeyEventArgs e)
        {
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");
            
            if (e.KeyModifiers == KeyModifiers.Shift && e.Key == Key.A) 
            {
                // (default)Shift + A: move cell timestamp selection up
                viewModel.MoveTimeSelection("up");
            }
            else if (e.KeyModifiers == KeyModifiers.Shift && e.Key == Key.S)
            {
                // (default)Shift + S: move cell timestamp selection down
                viewModel.MoveTimeSelection("down");
            }
        }

        private void Player_SetTimeClick(object? sender, RoutedEventArgs e)
        {
            MainWindowViewModel viewModel = DataContext as MainWindowViewModel ?? throw new MemberAccessException("Failed to load view model.");
            viewModel.SetTime();

            LyricsDataChanged(this, EventArgs.Empty);
        }
    }
}