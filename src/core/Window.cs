using System.ComponentModel;
using Monte.Settings;
using static SDL.SDL_video;

namespace Monte.Core
{
    /// <summary>
    /// Main window class.
    /// </summary>
    public static class Window
    {
        /// <summary>
        /// Set the main windows title
        /// </summary>
        /// <param name="title">string to set the title to</param>
        public static void SetWindowTitle(string title) => SDL_SetWindowTitle(MonteEngine.SDL_Window, title);

        /// <summary>
        /// Set the window size of the main window
        /// </summary>
        /// <param name="width">new width of the window</param>
        /// <param name="height">new height of the window</param>
        public static void SetWindowSize(int width, int height) => SDL_SetWindowSize(MonteEngine.SDL_Window, width, height);

        /// <summary>
        /// boolean value whether the window is focused.
        /// </summary>
        public static bool IsFocused { get; private set; }

        /// <summary>
        /// Event called each time a window event happens. With the event in arguments
        /// </summary>
        public static event Action<SDL.SDL_events.SDL_Event>? OnWindowEvent = null;
        internal static void WindowEvent(SDL.SDL_events.SDL_Event e)
        {
            switch (e.type)
            {
                case SDL.SDL_events.SDL_EventType.SDL_WINDOWEVENT:
                    OnWindowEvent?.Invoke(e);

                    if (e.window.windowEvent == SDL.SDL_events.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_LOST)
                        IsFocused = false;

                    if (e.window.windowEvent == SDL.SDL_events.SDL_WindowEventID.SDL_WINDOWEVENT_FOCUS_GAINED)
                        IsFocused = true;

                    break;
            }

            if (WindowSettings.AlwaysKeepFocus && !IsFocused)
            {
                SDL_RaiseWindow(MonteEngine.SDL_Window);
            }
        }
    }
}