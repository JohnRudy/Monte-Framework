using static SDL2.SDL;


namespace Monte
{
    public class Window
    {
        readonly IntPtr _sdl_window;
        readonly int _width;
        readonly int _height;
        public IntPtr SDLWindow
        {
            get => _sdl_window;
        }
        public int Width { get => _width; }
        public int Height { get => _height; }


        public Window()
        {
            _width = WindowSettings.WindowWidth;
            _height = WindowSettings.WindowHeight;

            _sdl_window = SDL_CreateWindow(
                WindowSettings.WindowTitle,
                WindowSettings.WindowPositionX,
                WindowSettings.WindowPositionY,
                _width,
                _height,
                WindowSettings.WindowFlags
            );

            if (_sdl_window == IntPtr.Zero)
                Debug.Log($"Could not initialize SDL Window: {SDL_GetError()}");
        }

        internal void HandleWindowEvent(SDL_Event e) { }

        public void SetTitle(string title) => SDL_SetWindowTitle(_sdl_window, title);
    }
}