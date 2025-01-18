using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Avalonia.Threading;
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

        public MainWindow()
        {
            InitializeComponent();
            this.AttachDevTools();
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

                // destroy the audio session
                (Player.DataContext as PlayerControlViewModel).Close();

                // purge the opened workspace
                //PurgeWorkspace();
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
                EditorView.Source = viewModel.Lyrics;

                // allocate DataContext to Player and make it visible
                Player.IsVisible = true;
                Preview.IsVisible = true;

                // mark as opened
                opened = true;
            }
        }
    }
}