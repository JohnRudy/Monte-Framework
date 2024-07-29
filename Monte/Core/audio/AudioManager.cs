using static SDL2.SDL;
using static SDL2.SDL_mixer;


namespace Monte.Audio
{
    public static class AudioManager
    {
        private static readonly int NumChannels = 32;
        private static AudioClip?[] Channels = Array.Empty<AudioClip>();
        private static bool[] ChannelAvailable = Array.Empty<bool>();

        public static void Init()
        {
            if (Mix_OpenAudio(22050, MIX_DEFAULT_FORMAT, 2, 4096) != 0)
                Debug.Log($"Unable to load MIXER: : {Mix_GetError()} ; {SDL_GetError()}");

            if (Mix_AllocateChannels(NumChannels) != NumChannels)
            {
                Debug.Log($"Could not allocate channels: : {Mix_GetError()} ; {SDL_GetError()}");
            }
            Channels = new AudioClip?[NumChannels];
            ChannelAvailable = Enumerable.Repeat(true, NumChannels).ToArray();
        }
        private static void MixSetPanning(int channel, double panning)
        {
            byte left, right;

            if (panning < -1.0)
            {
                left = 0;
                right = 255;
            }
            else if (panning > 1.0)
            {
                left = 255;
                right = 0;
            }
            else
            {
                double factor = (panning + 1.0) / 2.0;
                left = (byte)(255 * (1.0 - factor));
                right = (byte)(255 * factor);
            }
            if (Mix_SetPanning(channel, left, right) == 0)
                Debug.Log($"Unable to set panning: {Mix_GetError()} ; {SDL_GetError()}");
        }
        private static void MixSetVolume(int channel, double volume)
        {
            int converted_volume = (int)Math.Floor(128 * volume);
            if (Mix_Volume(channel, converted_volume) == 0)
                Debug.Log($"Unable to set volume: {Mix_GetError()} ; {SDL_GetError()}");
        }

        public static void MixPlayAudio(AudioClip ac)
        {
            if (ac.ChannelID == -1)
            {
                ac.ChannelID = Array.FindIndex(ChannelAvailable, x => x == true);
                if (ac.ChannelID == -1)
                {
                    Debug.Log("No available channels!");
                    return;
                }
            }

            Channels[ac.ChannelID] = ac;
            ChannelAvailable[ac.ChannelID] = false;
            Mix_PlayChannel(ac.ChannelID, ac.Wav, 0);
        }

        /// <summary>
        /// A paused channel is considered as playing
        /// </summary>
        public static bool MixIsPlaying(AudioClip ac) => Mix_Playing(ac.ChannelID) != 0;

        public static void MixStopAudio(AudioClip ac)
        {
            if (MixIsPlaying(ac))
            {
                if (Mix_HaltChannel(ac.ChannelID) != 0)
                {
                    Debug.Log($"Unable to halt channel!: {Mix_GetError()} ; {SDL_GetError()}");
                    return;
                }
                ResetAC(ac);
            }
        }

        public static void MixPauseAudio(AudioClip ac)
        {
            if (ac.ChannelID != -1)
                if (Mix_Playing(ac.ChannelID) != 0)
                    Mix_Pause(ac.ChannelID);
        }

        public static void MixResumeAudio(AudioClip ac)
        {
            if (ac.ChannelID != -1)
                if (Mix_Playing(ac.ChannelID) != 0)
                    Mix_Resume(ac.ChannelID);
        }

        private static void ResetAC(AudioClip ac)
        {
            Channels[ac.ChannelID] = null;
            ChannelAvailable[ac.ChannelID] = true;
            ac.ChannelID = -1;
        }

        internal static void Update()
        {
            foreach (AudioClip? ac in Channels)
            {
                if (ac != null)
                {
                    if (!MixIsPlaying(ac))
                        ResetAC(ac);
                    else
                    {
                        MixSetPanning(ac.ChannelID, ac.Panning);
                        MixSetVolume(ac.ChannelID, ac.Volume);
                    }
                }
            }
        }
    }
}