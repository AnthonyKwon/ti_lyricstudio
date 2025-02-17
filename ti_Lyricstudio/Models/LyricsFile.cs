using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace ti_Lyricstudio.Models
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
        public List<LyricData> Open()
        {
            // throw exception if file not found on disk
            if (!File.Exists(file)) { throw new FileNotFoundException(); }

            // create lyrics list for return
            List<LyricData> lyrics = [];

            // initialize StreamReader
            StreamReader FileStream = new(file);
            while (FileStream.Peek() != -1)  // repeat until file end
            {
                // read single line of file
                string line = FileStream.ReadLine();
                // end of line is reached but parsing not finished for unknown reason,
                // break out of loop manually
                if (line == null) break;
                // parse line to LRC handler
                object parsedLine = LRCHandler.From(line);

                // if lyric data is returned, append it to list
                if (parsedLine.GetType() == typeof(LyricData)) lyrics.Add((LyricData)parsedLine);
                // if string(LRC header) is returned, save it to additional data array
                else AdditionalData.Add((string)parsedLine);
            }

            // close the file
            FileStream.Close();

            return lyrics;
        }

        /// <summary>
        /// Save the lyrics file.
        /// </summary>
        /// <param name="lyrics">lyrics data to save</param>
        public void Save(Collection<LyricData> lyrics)
        {
            Save(lyrics, file);
        }

        /// <summary>
        /// Save the lyrics file.
        /// </summary>
        /// <param name="lyrics">lyrics data to save</param>
        /// <param name="path">file path to save</param>
        public void Save(Collection<LyricData> lyrics, string path)
        {
            // create a backup of original file
            if (File.Exists(path))
            {
                string backupPath = Path.ChangeExtension(path, ".lrc.bak");
                // remove existing backup file
                if (File.Exists(backupPath)) File.Delete(backupPath);
                File.Copy(path, backupPath);
            }

            // initialise StreamWriter
            StreamWriter writer = new(path);

            // write additional data to disk
            foreach (string data in AdditionalData)
            {
                // write each line of additional data to disk
                writer.WriteLine(data);
            }

            // write lyrics to disk
            foreach (LyricData lyric in lyrics)
            {
                string lrc = LRCHandler.To(lyric);
                writer.WriteLine(lrc);
            }

            // close the file
            writer.Close();
        }
    }
}
