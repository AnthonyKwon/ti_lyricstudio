using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

namespace ti_Lyricstudio.Class
{
    /// <summary>
    /// Information of the lyrics file.
    /// </summary>
    /// <param name="file">Path to the lyrics file.</param>
    public class LyricsFile(string file)
    {
        /// <summary>
        /// Path to the lyrics file.
        /// </summary>
        public string FilePath => file;
        private List<string> AdditionalData = [];

        /// <summary>
        /// Opens the lyrics file.
        /// </summary>
        /// <returns>List of the lyrics data.</returns>
        public List<LyricData> open() {
            // throw exception if file not found on disk
            if (!System.IO.File.Exists(file)) { throw new System.IO.FileNotFoundException(); }

            // create lyrics list for return
            List<LyricData> lyrics = [];

            // initialize StreamReader and LRCHandler
            StreamReader FileStream = new(file);
            while (FileStream.Peek() != -1)  // repeat until file end
            {
                // read single line of file
                string line = FileStream.ReadLine();
                // parse line to LRC handler
                object parsedLine = LRCHandler.From(line);

                // if lyric data is returned, append it to list
                if (parsedLine.GetType() == typeof(LyricData)) lyrics.Add((LyricData)parsedLine);
                // if string(LRC header) is returned, save it to additional data array
                else AdditionalData.Add((string)parsedLine);
            }

            return lyrics;
        }
    }
}
