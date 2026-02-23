using CommunityToolkit.Mvvm.ComponentModel;

namespace ti_Lyricstudio.Models
{
    /// <summary>
    /// State of the lyrics line associated with the editor.
    /// </summary>
    public enum LyricLineState
    {
        Normal,
        Selected,
        Editing
    }

    public class FlattenedLyricData(LyricTime time, string text, int line, bool isLinked) : ObservableObject
    {
        private LyricTime _time = time;  // time of the current line
        private string _text = text;  // text of the current line
        private int _line = line;   // line of the current line
        private bool _isLinked = isLinked;  // check if current line is linked to other line
        private bool _active = true;  // current line is marked as active
        private LyricLineState _lineState = LyricLineState.Normal;  // state of the current line

        /// <summary>
        /// Text of the lyric. (as string)
        /// </summary>
        public string Text
        {
            get => _text;
            set => SetProperty(ref _text, value);
        }

        /// <summary>
        /// Time of the lyric.
        /// </summary>
        public LyricTime Time
        {
            get => _time;
            set => SetProperty(ref _time, value);
        }

        /// <summary>
        /// Actual line number of the flattened line.
        /// </summary>
        public int Line
        {
            get => _line;
            set => SetProperty(ref _line, value);
        }

        /// <summary>
        /// Checks if current line is linked to other line in original lyrics.
        /// Returns true if current line is linked to other.
        /// </summary>
        public bool IsLinked
        {
            get => _isLinked;
            set => SetProperty(ref _isLinked, value);
        }

        /// <summary>
        /// Checks if current line is playing line.
        /// </summary>
        public bool Active
        {
            get => _active;
            set => SetProperty(ref _active, value);
        }

        /// <summary>
        /// Gets or sets the state of current line.
        /// </summary>
        public LyricLineState LineState
        {
            get => _lineState;
            set
            {
                if (SetProperty(ref _lineState, value))
                {
                    OnPropertyChanged(nameof(IsSelected));
                    OnPropertyChanged(nameof(IsEditing));
                }
            }
        }

        /// <summary>
        /// Checks if current line is selected.
        /// </summary>
        public bool IsSelected => _lineState is LyricLineState.Selected or LyricLineState.Editing;

        /// <summary>
        /// Checks if current line is editing.
        /// </summary>
        public bool IsEditing => _lineState == LyricLineState.Editing;

        /// <summary>
        /// Get current lyric as LRC-formatted string.
        /// </summary>
        /// <returns>LRC-formatted string of current lyric</returns>
        public override string ToString()
        {
            return $"[{_time}]{_text}";
        }
    }
}
