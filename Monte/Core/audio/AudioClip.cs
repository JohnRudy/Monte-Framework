using static SDL2.SDL;


namespace Monte.Audio
{
    public class AudioClip
    {
        public string File;
        public SDL_AudioSpec? spec = null;
        public IntPtr buf;
        public uint len;
        public IntPtr Wav;
        public int ChannelID = -1;
        public double Panning = 0.0;
        public double Volume = 1.0;

        public AudioClip(string file, SDL_AudioSpec? audioSpec, IntPtr buf, uint len, IntPtr mix_wav)
        {
            File = file;
            spec = audioSpec;
            this.buf = buf;
            this.len = len;
            Wav = mix_wav;
        }
    }
}