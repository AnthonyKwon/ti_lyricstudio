using Avalonia.Controls;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.ObjectModel;
using System.IO;
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

        // 
        [ObservableProperty]
        private Color _gradientColor1;
        [ObservableProperty]
        private Color _gradientColor2;
        [ObservableProperty]
        private Color _gradientColor3;
        [ObservableProperty]
        private Color _gradientColor4;

        // lyrics data used by application
        private readonly ObservableCollection<LyricData> _lyrics = [];

        public PlayerUIWindowViewModel()
        {
            // calling this ViewModel without any param is not intended except designer,
            // throw exception when that situation happened
            if (!Design.IsDesignMode) throw new InvalidOperationException();

            // set gradient color to background color
            GradientColor1 = GradientColor2 = GradientColor3 = GradientColor4 = BgBrush.Color;

            PlayerDataContext = new(_player);
            EditorDataContext = new(_lyrics, _player);
        }

        public PlayerUIWindowViewModel(AudioPlayer player)
        {
            // set gradient color to background color
            GradientColor1 = GradientColor2 = GradientColor3 = GradientColor4 = BgBrush.Color;

            _player = player;
            PlayerDataContext = new(_player);
            EditorDataContext = new(_lyrics, _player);
        }

        // UI interaction on "Open" button clicked
        // check if current workspace is modified and open file dialog
        public void OpenFile(string audioPath)
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
            PlayerDataContext.Open(audioPath);

            // load the preview
            EditorDataContext.FileOpened();

            // extract dominent color from artwork
            System.Collections.Generic.List<Color> colors = _player.GetGradientColors(audioPath);
            GradientColor1 = colors?.Count > 0 ? colors[0] : BgBrush.Color;
            GradientColor2 = colors?.Count > 1 ? colors[1] : BgBrush.Color;
            GradientColor3 = colors?.Count > 2 ? colors[2] : BgBrush.Color;
            GradientColor4 = colors?.Count > 3 ? colors[3] : BgBrush.Color;

            // mark file as opened
            Opened = true;
        }

        // UI interaction on file close
        public void CloseFile()
        {
            // destroy the audio session
            PlayerDataContext.Close();
            // stop the lyrics preview
            EditorDataContext.FileClosed();
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
