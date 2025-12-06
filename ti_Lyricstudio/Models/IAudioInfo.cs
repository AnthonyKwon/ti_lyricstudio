using Avalonia.Media.Imaging;

namespace ti_Lyricstudio.Models
{
    public interface IAudioInfo
    {
        string Title { get; }
        string Artist { get; }
        string Album { get; }
        Bitmap Artwork { get; }
    }
}
