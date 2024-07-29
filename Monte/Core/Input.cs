using static SDL2.SDL;


namespace Monte
{
    public static class Input
    {
        public struct KeyState
        {
            public bool isPressed;
            public bool isReleased;
            public uint StartFrame;
            public uint EndFrame;
        }

        public static Dictionary<SDL_Keycode, bool> Keys = new();
        static Dictionary<SDL_Keycode, KeyState> _states = new();
        static Dictionary<uint, bool> _mouse = new();
        private static SDL_Point _MouseScreenPosition;
        private static SDL_FPoint _MouseWindowPosition;
        public static SDL_Point MouseScreenPosition => _MouseScreenPosition;
        public static SDL_FPoint MouseWindowPosition => _MouseWindowPosition;


        internal static void Setup()
        {
            foreach (SDL_Keycode key in SDL_Keycode.GetValues(typeof(SDL_Keycode)))
            {
                Keys.Add(key, false);
            }

            _mouse.Add(SDL_BUTTON_LEFT, false);
            _mouse.Add(SDL_BUTTON_RIGHT, false);
            _mouse.Add(SDL_BUTTON_MIDDLE, false);
            _mouse.Add(SDL_BUTTON_X1, false);
            _mouse.Add(SDL_BUTTON_X2, false);
        }

        internal static void HandleKeyboard(SDL_Event e, uint gameTick)
        {

            var keyCode = e.key.keysym.sym;
            switch (e.type)
            {
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    _mouse[e.button.button] = true;
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    _mouse[e.button.button] = false;
                    break;
                case SDL_EventType.SDL_KEYDOWN:
                    if (!Keys[keyCode])
                    {
                        Keys[keyCode] = true;
                        _states[keyCode] = new KeyState
                        {
                            isPressed = true,
                            isReleased = false,
                            StartFrame = gameTick,
                            EndFrame = 0
                        };
                    }
                    break;

                case SDL_EventType.SDL_KEYUP:
                    if (Keys[keyCode])
                    {
                        Keys[keyCode] = false;
                        _states[keyCode] = new KeyState
                        {
                            isPressed = false,
                            isReleased = true,
                            StartFrame = 0,
                            EndFrame = gameTick
                        };
                    }
                    break;
            }
        }

        // Returns true if the key is pressed or held. 
        public static bool GetKey(SDL_Keycode key) => Keys[key];

        // returns the state of the key on this frame
        public static KeyState GetKeyState(SDL_Keycode key) => _states[key];

        public static bool GetMouseButton(uint mb) => _mouse[mb];

        internal static void HandleMouse()
        {
            _ = SDL_GetMouseState(out int x, out int y);
            _MouseScreenPosition = new() { x = x, y = y };

            SDL_RenderWindowToLogical(Renderer.Instance.SDL_Renderer, x, y, out float logicalX, out float logicalY);
            _MouseWindowPosition = new() { x = logicalX, y = logicalY };
        }

        public static bool IsMouseWithinScreen(SDL_Rect elementRect)
        {
            bool isWithinRectX = _MouseScreenPosition.x > elementRect.x && _MouseScreenPosition.x < elementRect.x + elementRect.w;
            bool isWithinRectY = _MouseScreenPosition.y > elementRect.y && _MouseScreenPosition.y < elementRect.y + elementRect.h;

            return isWithinRectX && isWithinRectY;
        }

        public static bool IsMouseWithinLogical(SDL_Rect elementRect)
        {
            bool isWithinRectX = _MouseWindowPosition.x > elementRect.x && _MouseWindowPosition.x < elementRect.x + elementRect.w;
            bool isWithinRectY = _MouseWindowPosition.y > elementRect.y && _MouseWindowPosition.y < elementRect.y + elementRect.h;

            return isWithinRectX && isWithinRectY;
        }
    }
}