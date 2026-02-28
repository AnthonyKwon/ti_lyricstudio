using Avalonia.Media;
using Avalonia.Media.Imaging;
using ImageMagick;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ti_Lyricstudio.Models
{
    public class AudioInfo : IAudioInfo
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
        
        /// <summary>
        /// Parse information of the media file.
        /// </summary>
        /// <param name="filePath">Path to the audio file</param>
        public static IAudioInfo Parse(string filePath)
        {
            // load tag from current file
            TagLib.File file = TagLib.File.Create(filePath);

            // parse current song and album data from tag
            string title = file.Tag.Title;
            string artist = file.Tag.FirstPerformer;
            string album = file.Tag.Album;

            // parse raw first artwork data from tag
            byte[] rawArtwork = file.Tag.Pictures[0].Data.Data;
            MemoryStream rawArtworkStream = new(rawArtwork);

            // create artwork bitmap from raw data
            Bitmap artwork = new(rawArtworkStream);

            return new AudioInfo(title, artist, album, artwork);
        }

        /// <summary>
        /// Parse dominant colors used by gradient from artwork.
        /// </summary>
        /// <param name="filePath">Path to the audio file</param>
        public static List<Color>? GetGradientColors(string filePath)
        {
            // load tag from current file
            TagLib.File file = TagLib.File.Create(filePath);

            // parse raw first artwork data from tag
            byte[] rawArtwork = file.Tag.Pictures[0].Data.Data;

            // create ImageMagick instance of artwork
            MagickImage magick = new(rawArtwork);
            // define K-means settings
            KmeansSettings kmeansSettings = new();
            kmeansSettings.NumberColors = 5;
            // scale down image to 32x32
            magick.Scale(32, 32);
            // apply K-means to reduce palette and extract dominant color
            magick.Kmeans(kmeansSettings);
            // create histogram of artwork
            IReadOnlyDictionary<IMagickColor<byte>, uint> histogram = magick.Histogram();
            List<IMagickColor<byte>> colors = histogram.Keys.ToList();

            // add extracted colors to return list
            List<Color> avaColors = new();
            for (int i = 0; i < (colors.Count > 5 ? 5 : colors.Count); i++)
                avaColors.Add(new(colors[i].A, colors[i].R, colors[i].G, colors[i].B));

            // return the extracted color
            return avaColors;
        }
    }
}
