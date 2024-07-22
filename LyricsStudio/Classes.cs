using Microsoft.VisualBasic.CompilerServices;

namespace com.stu_tonyk_dio.ti_LyricsStudio
{
    public class FileInfo // Data of Working Files (Working Directory, Extension File, Location File)
    {
        public FileInfo(bool AudioLoaded, bool LyricsLoaded, string Location, string FileName, string Extension)
        {
            _AudioLoaded = Conversions.ToString(AudioLoaded);
            _LyricsLoaded = Conversions.ToString(LyricsLoaded);
            _Location = Location;
            _FileName = FileName;
            _Extension = Extension;
        }

        private string _AudioLoaded;
        public string AudioLoaded
        {
            get
            {
                return _AudioLoaded;
            }
            set
            {
                _AudioLoaded = value;
            }
        }

        private string _LyricsLoaded;
        public string LyricsLoaded
        {
            get
            {
                return _LyricsLoaded;
            }
            set
            {
                _LyricsLoaded = value;
            }
        }

        private string _Location;
        public string Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
            }
        }

        private string _FileName;
        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }

        private string _Extension;
        public string Extension
        {
            get
            {
                return _Extension;
            }
            set
            {
                _Extension = value;
            }
        }
    }
}