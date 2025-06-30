using Monte.Abstractions;
using Monte.Interfaces;
using Monte.Core;

using static SDL.SDL_mixer;
using static SDL.SDL_audio;
using static SDL.SDL_rect;
using Monte.Settings;
using System.Runtime.InteropServices;


namespace Monte.Components
{
    /// <summary>
    /// Wrapper for SDL_mixer required audio info. 
    /// You should never need to touch Audioclips themselves, rather control everything throug the Audiosource.
    /// </summary>
    public class AudioClip
    {
        /// <summary>
        /// Sound file this clip represents
        /// </summary>
        public string File;

        /// <summary>
        /// SDL_Audiospec reference
        /// </summary>
        public SDL_AudioSpec? spec = null;

        /// <summary>
        /// buffer pointer reference
        /// </summary>
        public IntPtr buf;

        /// <summary>
        /// Length of the clip.
        /// </summary>
        public uint len;

        /// <summary>
        /// Main pointer of loaded mix chunk. 
        /// </summary>
        public IntPtr mix_wav;

        /// <summary>
        /// Current channel it's played on
        /// </summary>
        public int ChannelID = -1;

        /// <summary>
        /// Panning value from 1 to -1 left to right
        /// </summary>
        public double Panning = 0.0;
        /// <summary>
        /// Volume value of 0 to 1, where 1 is the loudest.
        /// </summary>
        public double Volume = 1.0;

        public AudioClip(string file, SDL_AudioSpec? audioSpec, IntPtr buf, uint len, IntPtr mix_wav)
        {
            File = file;
            spec = audioSpec;
            this.buf = buf;
            this.len = len;
            this.mix_wav = mix_wav;
        }
    }

    /// <summary>
    /// Main audio component.
    /// Handles calls to audio manager and references to audio clips and pointers. 
    /// </summary>
    public class AudioSource : IComponent
    {
        MonteBehaviour? _parent;
        public MonteBehaviour? Parent { get => _parent; set => _parent = value; }
        private string? _file;
        public string? File { get => _file; set => _file = value; }

        /// <summary>
        /// Should this audiosource pan automatically based on camera location. 
        /// </summary>
        public bool CamerPan = false;

        /// <summary>
        /// Current AudioClip playing.
        /// </summary>
        public AudioClip? CurrentClip;

        /// <summary>
        /// Property value of audio clip if it's playing or not. 
        /// </summary>
        public bool IsPlaying
        {
            get
            {
                if (CurrentClip == null)
                {
                    return false;
                }
                return Mix_Playing(CurrentClip.ChannelID) != 0;
            }
        }

        /// <summary>
        /// Main volume property of audioclip. from 1 to 0
        /// </summary>
        public double Volume
        {
            get => CurrentClip != null ? CurrentClip.Volume : 1.0f;
            set
            {
                if (CurrentClip != null)
                    CurrentClip.Volume = Math.Min(1, Math.Max(0, value));
            }
        }

        /// <summary>
        /// Panning value of Audioclip. from -1 to 1 left to right. 
        /// </summary>
        public double Pan
        {
            get => CurrentClip != null ? CurrentClip.Panning : 0.0f;
            set
            {
                if (CurrentClip != null)
                    CurrentClip.Panning = Math.Min(1, Math.Max(-1, value));
            }
        }

        public AudioSource(string file)
        {
            File = file;
        }

        public void Initialize()
        {
            if (File != null)
                CurrentClip = ContentManager.LoadAudio(File);
        }


        /// <summary>
        /// Load a new file to play. 
        /// </summary>
        /// <param name="file">name of the file</param>
        public void LoadAudioFile(string file)
        {
            File = file;
            Initialize();
        }

        /// <summary>
        /// Start playback of audiofile. Does not resume paused audioclips. 
        /// </summary>
        public void Play()
        {
            if (CurrentClip != null)
                AudioManager.MixPlayAudio(this);
        }

        /// <summary>
        /// Pause the current audioclip. 
        /// </summary>
        public void Pause()
        {
            if (CurrentClip != null)
                AudioManager.MixPauseAudio(this);
        }


        /// <summary>
        /// Resume the playback of the audioclip.
        /// </summary>
        public void Resume()
        {
            if (CurrentClip != null)
                AudioManager.MixResumeAudio(this);
        }

        /// <summary>
        /// Stop the current playback of audioclip.
        /// </summary>
        public void Stop()
        {
            if (CurrentClip != null)
                AudioManager.MixStopAudio(this);
        }

        public void Destroy() { }
        public void Update()
        {
            if (CamerPan && Parent != null)
            {
                float halfWidth = RendererSettings.VirtualWidth / 2;
                float pos = ((Parent.Transform.Position.X + halfWidth) / Camera.Transform.Position.X ) - 1;
                float pan = Math.Max(-1, Math.Min(1, pos));
                Pan = pan;
            }
        }
    }
}