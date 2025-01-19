using System.Collections.ObjectModel;
using System.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using ti_Lyricstudio.Models;

namespace ti_Lyricstudio.ViewModels
{
    public partial class LyricsPreviewViewModel : ViewModelBase
    {
        [ObservableProperty]
        private string _currentLine = string.Empty;

        [ObservableProperty]
        private string _nextLine1 = string.Empty;

        [ObservableProperty]
        private string _nextLine2 = string.Empty;

        public void Load()
        {
            if (DataStore.Instance.Lyrics == null || DataStore.Instance.Player == null) return;

            CurrentLine = DataStore.Instance.Lyrics[0].Text;
            NextLine1 = DataStore.Instance.Lyrics[1].Text;
            NextLine2 = DataStore.Instance.Lyrics[2].Text;
        }
    }
}
