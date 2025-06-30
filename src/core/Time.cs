using System.Security.Cryptography.X509Certificates;
using Monte.Settings;
using static SDL.SDL;


namespace Monte.Core
{
    /// <summary>
    /// Time class to keep track of timing related intervals.
    /// </summary>
    public static class Time
    {
        /// <summary>
        /// Current delta time between frames
        /// </summary>
        public static float DeltaTime { get; private set; }

        /// <summary>
        /// Current frames per second
        /// </summary>
        public static float CurrentFPS { get; private set; }

        /// <summary>
        /// Last frame render time in MS
        /// </summary>
        public static uint LastFrameTook { get; private set; }

        private static uint _gameTime = 0;

        /// <summary>
        /// Current gametime in ms
        /// </summary>
        public static uint GameTimeMS => _gameTime;

        public static uint frame = 0;
        public static uint delayTime = 0;

        static uint frameStartTick = 0;
        public static void FrameStart()
        {
            frameStartTick = SDL_GetTicks();
            frame++;
        }

        static uint frameEndTick = 0;
        public static void FrameEnd()
        {
            frameEndTick = SDL_GetTicks();

            uint takenTime = frameEndTick - frameStartTick;
            LastFrameTook = takenTime;

            if (takenTime < 1.0f / EngineSettings.DesiredFPS * 1000)
            {
                delayTime = (uint)(1.0f / EngineSettings.DesiredFPS * 1000) - takenTime;
                DeltaTime = (float)1.0f / EngineSettings.DesiredFPS;
                CurrentFPS = EngineSettings.DesiredFPS;
            }
            else
            {
                delayTime = 0;
                DeltaTime = takenTime / 1000f;
                CurrentFPS = 1000f / takenTime;
            }
        }
    }
}