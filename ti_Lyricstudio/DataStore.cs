// Use singletone to store shared data
// This is a temporary measure, move to MVVM-friendly measure when ready

using System.Collections.Generic;
using System.Collections.ObjectModel;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio
{
    public sealed class DataStore
    {
        private static readonly DataStore instance = new();

        public static DataStore Instance => instance;

        public AudioPlayer? Player { get; set; }
        public ObservableCollection<LyricData>? Lyrics { get; set; }
    }
}