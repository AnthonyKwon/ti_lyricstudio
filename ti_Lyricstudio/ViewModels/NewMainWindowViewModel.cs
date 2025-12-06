using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.IO;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class NewMainWindowViewModel : ViewModelBase
    {
        // audio player to control
        private readonly AudioPlayer _player = new AudioPlayer();

        // ViewModel for VLC player control
        public PlayerPanelViewModel PlayerDataContext { get; }

        // marker to check if file is opened
        [ObservableProperty]
        private bool _opened = false;

        // marker to check if file has modified
        [ObservableProperty]
        private bool _modified = false;

        public NewMainWindowViewModel()
        {
            // calling this ViewModel without any param is not intended except designer,
            // throw exception when that situation happened
            if (!Design.IsDesignMode) throw new InvalidOperationException();

            PlayerDataContext = new(_player);
        }

        public NewMainWindowViewModel(AudioPlayer player)
        {
            _player = player;
            PlayerDataContext = new(_player);
        }

        // UI interaction on "Open" button clicked
        // check if current workspace is modified and open file dialog
        public void OpenFile(string audioPath)
        {
            // generated lrc file path based on the audio file path
            string lrcPath = Path.ChangeExtension(audioPath, ".lrc");

            /*
            // initialize the file object
            file = new(lrcPath);

            // check if lrc file exists
            if (File.Exists(lrcPath))
            {
                // load the file
                _lyrics.Clear();
                foreach (LyricData line in file.Open())
                {
                    _lyrics.Add(line);
                }
            }
            else
            {
                // create empty data for new lyrics file
                LyricTime emptyTime = new(0, 0, 0);
                LyricData emptyData = new();
                emptyData.Time.Add(emptyTime);
                emptyData.Text = "Start typing your lyrics here!";

                // bind empty data to new list
                _lyrics.Clear();
                _lyrics.Add(emptyData);
            }

            // register the lyrics changed event
            _lyrics.CollectionChanged += LyricsChanged;

            // initialize lyrics source from data
            InitializeSource();
            */

            // open the audio
            PlayerDataContext.Open(audioPath);

            // load the preview
            //PreviewDataContext.Start();

            // mark file as opened
            Opened = true;
        }

        // UI interaction on file close
        public void CloseFile()
        {
            // destroy the audio session
            PlayerDataContext.Close();
            // stop the lyrics preview
            //PreviewDataContext.Stop();
            // unregister the lyrics changed event
            //_lyrics.CollectionChanged -= LyricsChanged;
            // mark workspace as not opened and unmodified
            Opened = false;
            //Modified = false;
        }

        /// <summary>
        /// Checks if lyrics file is opened.
        /// </summary>
        /// <returns>Results of the check</returns>
        public bool FileOpened() => Opened;
        /// <summary>
        /// Checks if lyrics file is opened and modified.
        /// </summary>
        /// <returns>Results of the check</returns>
        public bool FileModified() => Opened && Modified;
    }
}
