using Avalonia.Media.Imaging;

namespace ti_Lyricstudio.Models
{
    internal class AudioInfo : IAudioInfo
    {
        // title of the current media
        public string Title { get => _title; }
        private string _title;

        // artist of the current media
        public string Artist { get => _artist; }
        private string _artist;

        // album of the current media
        public string Album { get => _album; }
        private string _album;

        // artwork of the current media
        public Bitmap Artwork { get => _artwork; }
        private Bitmap _artwork;

        // create empty AudioInfo object
        public AudioInfo()
        {
            _title = "Unknown Song";
            _artist = "Unknown Artist";
            _album = "Unknown Album";
            _artwork = new("/Assets/Images/UnknownArtwork.png");
        }

        // create new AudioInfo object with artwork
        public AudioInfo(string title, string artist, string album, Bitmap artwork)
        {
            _title = title;
            _artist = artist;
            _album = album;
            _artwork = artwork;
        }
    }
}
