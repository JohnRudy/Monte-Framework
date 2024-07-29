using Monte.Abstractions;
using Monte.Interfaces;
using static SDL2.SDL_mixer;


namespace Monte.Audio
{
    public class AudioSource : IComponent
    {
        Entity _parent;
        public Entity Parent { get => _parent; set => _parent = value; }
        public AudioClip? CurrentClip;

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
        public double Volume
        {
            get => CurrentClip != null ? CurrentClip.Volume : 1.0f;
            set
            {
                if (CurrentClip != null)
                    CurrentClip.Volume = Math.Min(1, Math.Max(0, value));
            }
        }
        public double Pan
        {
            get => CurrentClip != null ? CurrentClip.Panning : 0.0f;
            set
            {
                if (CurrentClip != null)
                    CurrentClip.Panning = Math.Min(1, Math.Max(-1, value));
            }
        }

        public AudioSource(Entity parent) {
            _parent = parent;
            _parent.Components.Add(this);
        }

        public void Play()
        {
            if (CurrentClip != null)
                AudioManager.MixPlayAudio(CurrentClip);
        }

        public void Pause()
        {
            if (CurrentClip != null)
                AudioManager.MixPauseAudio(CurrentClip);
        }

        public void Resume()
        {
            if (CurrentClip != null)
                AudioManager.MixResumeAudio(CurrentClip);
        }
        public void Stop()
        {
            if (CurrentClip != null)
                AudioManager.MixStopAudio(CurrentClip);
        }

        public void Destroy() { 
            if (CurrentClip != null){
                ContentManager.UnloadAudio(CurrentClip.File);
            }
        }
        public void Initialize() { }
        public void Update() { }
    }
}