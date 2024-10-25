using Microsoft.Win32;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ti_Lyricstudio.Class;

namespace ti_Lyricstudio
{
    public partial class MainWindow : Window
    {
        // import command definition
        public readonly static RoutedCommand ImportCommand = new RoutedCommand();

        // open & save file dialog
        private readonly OpenFileDialog openDialog = new();
        private readonly SaveFileDialog saveDialog = new();

        private void PurgeWorkspace()
        {
            // unbind the data source from EditorView
            EditorView.ItemsSource = null;

            // close the player
            Player.Close();
        }

        // open button is always enabled
        private void CanOpen(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        // other buttons are enabled when workspace is opened
        private void CanImportOrSave(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = opened;
        }

        // Action on "Open" button clicked
        // open and load a existing audio and lyrics file
        private void ExecutedOpen(object sender, ExecutedRoutedEventArgs e)
        {
            // ask user to continue if file was opened and modified
            if (opened == true || modified == true)
            {
                if (modified == true)
                {
                    // ask user to continue
                    MessageBoxResult result = System.Windows.MessageBox.Show("File has been modified. Are you sure to continue without saving?", "File modified", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.No) return;
                }
                // purge the opened workspace
                PurgeWorkspace();
            }

            // configure open file dialog
            openDialog.Title = "Open workspace...";
            openDialog.Filter = "Audio Files|*.alac;*.ape;*.flac;*.m4a;*.mp3;*mp4;*.oga;*.ogg;*.opus;*.wav;*wma|All Files|*.*";

            // action after user chose audio file to load
            if (openDialog.ShowDialog() == true)
            {
                // find LRC file at the audio file location
                string lrcFile = Path.ChangeExtension(openDialog.FileName, ".lrc");
                // open LRC file if exists
                if (File.Exists(lrcFile))
                {
                    // (re-)initialise lyrics file object
                    file = new(lrcFile);
                    // open file and save lyrics to list
                    lyrics = file.Open();

                    // bind the data source from EditorView
                    dataSource = new(lyrics);
                    EditorView.ItemsSource = dataSource;
                }
                else
                {
                    // (re-)initialise lyrics file object
                    file = new(lrcFile);

                    // create empty data for new lyrics file
                    LyricTime emptyTime = new(0, 0, 0);
                    LyricData emptyData = new();
                    emptyData.Time.Add(emptyTime);
                    emptyData.Text = "Start typing your lyrics here!";
                    List<LyricData> emptyList = [emptyData];
                    lyrics = emptyList;

                    // bind the data source from EditorView
                    dataSource = new(lyrics);
                    EditorView.ItemsSource = dataSource;
                }

                // open the audio
                Player.Open(openDialog.FileName, lyrics);

                // mark file as opened
                opened = true;
            }
        }

        private void ExecutedImport(object sender, ExecutedRoutedEventArgs e)
        {
            //
        }

        private void ExecutedSave(object sender, ExecutedRoutedEventArgs e)
        {
            //
        }

        private void ExecutedSaveAs(object sender, ExecutedRoutedEventArgs e)
        {
            //
        }
    }
}
