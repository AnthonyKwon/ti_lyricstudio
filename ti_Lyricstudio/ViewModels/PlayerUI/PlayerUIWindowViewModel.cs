using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Media.Imaging;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class PlayerUIWindowViewModel : ViewModelBase
    {
        // audio player to control
        private readonly AudioPlayer _player = new AudioPlayer();

        // ViewModel for VLC player control
        public PlayerPanelViewModel PlayerDataContext { get; }

        // ViewModel for Lyrics Editor
        public EditorViewModel EditorDataContext { get; }

        // currently working file
        private LyricsFile? file;

        // marker to check if file is opened
        [ObservableProperty]
        private bool _opened = false;

        // marker to check if file has modified
        [ObservableProperty]
        private bool _modified = false;
        
        // binding for the title of media artwork
        [ObservableProperty]
        private Bitmap? _songArtwork;

        // lyrics data used by application
        private readonly ObservableCollection<LyricData> _lyrics = [];

        public PlayerUIWindowViewModel()
        {
            // calling this ViewModel without any param is not intended except designer,
            // throw exception when that situation happened
            if (!Design.IsDesignMode) throw new InvalidOperationException();

            PlayerDataContext = new(_player);
            EditorDataContext = new(_lyrics, _player);
        }

        public PlayerUIWindowViewModel(AudioPlayer player)
        {
            _player = player;
            PlayerDataContext = new(_player);
            EditorDataContext = new(_lyrics, _player);
        }

        // UI interaction on "Open" button clicked
        // check if current workspace is modified and open file dialog
        public async void OpenFile(string audioPath)
        {
            // generated lrc file path based on the audio file path
            string lrcPath = Path.ChangeExtension(audioPath, ".lrc");

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

            // open the audio
            await _player.Open(audioPath);

            // parse and set audio info
            IAudioInfo info = AudioInfo.Parse(audioPath);
            PlayerDataContext.SongTitle = info.Title;
            PlayerDataContext.SongAlbumInfo = $"{info.Artist} – {info.Album}";
            PlayerDataContext.SongArtwork = SongArtwork = info.Artwork;

            // mark file as opened
            Opened = true;
        }

        // UI interaction on file close
        public void CloseFile()
        {
            // destroy the audio session
            _player.Close();

            // mark workspace as not opened and unmodified
            Opened = false;
            Modified = false;
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