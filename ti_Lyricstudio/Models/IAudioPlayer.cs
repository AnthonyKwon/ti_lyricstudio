using System.Threading.Tasks;
using System;

namespace ti_Lyricstudio.Models
{
    public interface IAudioPlayer
    {
        long Duration { get; }
        PlayerState State { get; }
        long Time { get; set; }

        event EventHandler<PlayerState>? PlayerStateChangedEvent;

        void Close();
        void FastForward();
        Task Open(string file);
        void Pause();
        void Play();
        void Rewind();
        void Stop();
        IAudioInfo ParseAudioInfo();
    }
}
