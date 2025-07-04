using static SDL.SDL;
using static SDL.SDL_mixer;

using Monte.Components;

namespace Monte.Core
{

    public static class AudioManager
    {
        private static readonly int NumChannels = 32;
        private static AudioSource?[] Channels = Array.Empty<AudioSource>();
        private static bool[] ChannelAvailable = Array.Empty<bool>();

        internal static void Initialize()
        {
            if (Mix_OpenAudio(22050, MIX_DEFAULT_FORMAT, 2, 4096) != 0)
                Debug.Log($"Unable to load MIXER: : {SDL_GetError()}");

            if (Mix_AllocateChannels(NumChannels) != NumChannels)
                Debug.Log($"Could not allocate channels: : {SDL_GetError()}");

            Channels = new AudioSource?[NumChannels];
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
                Debug.Log($"Unable to set panning: {SDL_GetError()}");
        }

        private static void MixSetVolume(int channel, double volume)
        {
            int converted_volume = (int)Math.Floor(128 * volume);
            if (Mix_Volume(channel, converted_volume) == 0)
                Debug.Log($"Unable to set volume: {SDL_GetError()}");
        }

        internal static void MixPlayAudio(AudioSource src)
        {
            if (src.CurrentClip is null)
                return;

            if (src.CurrentClip.ChannelID == -1)
            {
                src.CurrentClip.ChannelID = Array.FindIndex(ChannelAvailable, x => x == true);
                if (src.CurrentClip.ChannelID == -1)
                {
                    Debug.Log("No available channels!");
                    return;
                }
            }

            Channels[src.CurrentClip.ChannelID] = src;
            ChannelAvailable[src.CurrentClip.ChannelID] = false;
            Mix_PlayChannel(src.CurrentClip.ChannelID, src.CurrentClip.mix_wav, 0);
        }

        /// <summary>
        /// A paused channel is considered as playing
        /// </summary>
        internal static bool MixIsPlaying(AudioSource src)
        {
            if (src.CurrentClip is null)
                return false;
            else
                return Mix_Playing(src.CurrentClip.ChannelID) != 0;
        }
        
        internal static void MixStopAudio(AudioSource src)
        {
            if (src.CurrentClip is null)
                return;

            if (MixIsPlaying(src))
            {
                if (Mix_HaltChannel(src.CurrentClip.ChannelID) != 0)
                {
                    Debug.Log($"Unable to halt channel!: {SDL_GetError()}");
                    return;
                }
                ResetAC(src);
            }
        }

        public static void MixPauseAudio(AudioSource src)
        {
            if (src.CurrentClip is null)
                return;

            if (src.CurrentClip.ChannelID != -1)
                if (Mix_Playing(src.CurrentClip.ChannelID) != 0)
                    Mix_Pause(src.CurrentClip.ChannelID);
        }

        public static void MixResumeAudio(AudioSource src)
        {
            if (src.CurrentClip is null)
                return;

            if (src.CurrentClip.ChannelID != -1)
                if (Mix_Playing(src.CurrentClip.ChannelID) != 0)
                    Mix_Resume(src.CurrentClip.ChannelID);
        }

        private static void ResetAC(AudioSource src)
        {
            if (src.CurrentClip is null)
                return;

            Channels[src.CurrentClip.ChannelID] = null;
            ChannelAvailable[src.CurrentClip.ChannelID] = true;
            src.CurrentClip.ChannelID = -1;
        }

        internal static void Update()
        {
            foreach (AudioSource? src in Channels)
            {
                if (src != null)
                {
                    if (!MixIsPlaying(src))
                        ResetAC(src);
                    else
                    {
                        if (src.CurrentClip is null)
                            continue;

                        MixSetPanning(src.CurrentClip.ChannelID, src.CurrentClip.Panning);
                        MixSetVolume(src.CurrentClip.ChannelID, src.CurrentClip.Volume);
                    }
                }
            }
        }
    }
}