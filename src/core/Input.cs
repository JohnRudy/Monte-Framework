using System.Numerics;

using static SDL.SDL_keycode;
using static SDL.SDL_Gamecontroller;
using static SDL.SDL_render;
using static SDL.SDL_events;
using static SDL.SDL_mouse;
using static SDL.SDL_rect;
using Monte.Settings;
using System.Runtime.InteropServices.Marshalling;

namespace Monte.Core
{
    public class ControllerInfo
    {
        public int id;
        public IntPtr controllerPointer;
        public Vector2 leftStick;
        public Vector2 rightStick;
        public float leftTrigger;
        public float rightTrigger;
        public float deadZone = 0.2f;
        public Dictionary<SDL_GameControllerButton, bool> buttons = new();
    }

    public static class Input
    {

        private static Dictionary<SDL_Keycode, bool> _previousKeys = new();
        private static Dictionary<SDL_Keycode, bool> _keys = new();

        private static Dictionary<byte, bool> _previousMouse = new();
        private static Dictionary<byte, bool> _mouse = new();

        private static Dictionary<int, ControllerInfo> _previousControllers = new();
        private static Dictionary<int, ControllerInfo> _controllers = new();

        private static SDL_FPoint _windowPosition;
        private static SDL_FPoint _logicalPosition;
        private static SDL_FPoint _worldPosition;
        private static SDL_FPoint _globalPosition;

        /// <summary>
        /// Plus means forward rotation
        /// </summary>
        public static int MouseWheelDelta { get; private set; } = 0;

        /// <summary>
        /// Current pixel location of cursor in window space
        /// </summary>
        public static SDL_FPoint MouseWindowPosition => _windowPosition;

        /// <summary>
        /// Current logical position of cursor in render space.
        /// </summary>
        public static SDL_FPoint MouseLogicalPosition => _logicalPosition;

        /// <summary>
        /// Current world position of the cursor
        /// </summary>
        public static SDL_FPoint MouseWorldPosition => _worldPosition;

        /// <summary>
        /// Current global position of the cursor
        /// </summary>
        public static SDL_FPoint MouseGlobalPosition => _globalPosition;

        internal static void Initialize()
        {
            foreach (SDL_Keycode key in SDL_Keycode.GetValues(typeof(SDL_Keycode)))
            {
                _keys.Add(key, false);
                _previousKeys.Add(key, false);
            }

            _mouse.Add(SDL_BUTTON_LEFT, false);
            _mouse.Add(SDL_BUTTON_RIGHT, false);
            _mouse.Add(SDL_BUTTON_MIDDLE, false);
            _mouse.Add(SDL_BUTTON_X1, false);
            _mouse.Add(SDL_BUTTON_X2, false);

            for (int i = 0; i < 3; i++)
            {
                if (SDL_IsGameController(i) == SDL.Constants.SDL_Bool.SDL_TRUE)
                {
                    IntPtr controller = SDL_GameControllerOpen(i);

                    if (controller == IntPtr.Zero)
                        break;
                    else
                    {
                        SetupGameController(controller, i);
                        Console.WriteLine("MonteEngine: Gamecontroller opened!");
                    }
                }
            }
        }

        internal static void BeginFrame()
        {
            _previousMouse = new Dictionary<byte, bool>(_mouse);
            _previousKeys = new Dictionary<SDL_Keycode, bool>(_keys);
            _previousControllers = new Dictionary<int, ControllerInfo>(_controllers);

            MouseWheelDelta = 0;
        }

        internal static void SetupGameController(IntPtr controller, int deviceIndex)
        {
            _controllers.TryGetValue(deviceIndex, out ControllerInfo? ci);
            if (ci != null) return;

            ci = new()
            {
                id = deviceIndex,
                controllerPointer = controller,
                leftStick = new(),
                rightStick = new(),
                leftTrigger = 0,
                rightTrigger = 0,
                buttons = []
            };

            foreach (SDL_GameControllerButton button in SDL_GameControllerButton.GetValues(typeof(SDL_GameControllerButton)))
                ci.buttons.Add(button, false);

            _controllers.Add(deviceIndex, ci);
        }

        internal static void Update(SDL_Event e)
        {
            var keyCode = e.key.keysym.sym;

            switch (e.type)
            {
                // Mouse Buttons
                case SDL_EventType.SDL_MOUSEBUTTONDOWN:
                    _mouse[e.button.button] = true;
                    // Debug.Log(e.button.button);
                    // Debug.Log($"{Time.frame}: {_mouse[e.button.button]}  {_previousMouse[e.button.button]}");
                    break;
                case SDL_EventType.SDL_MOUSEBUTTONUP:
                    _mouse[e.button.button] = false;
                    // Debug.Log($"{Time.frame}: {_mouse[e.button.button]}  {_previousMouse[e.button.button]}");
                    break;

                // Mouse wheel
                case SDL_EventType.SDL_MOUSEWHEEL:
                    MouseWheelDelta = e.wheel.y;
                    break;

                // Keyboard keys
                case SDL_EventType.SDL_KEYDOWN:
                    _keys[keyCode] = true;
                    break;

                case SDL_EventType.SDL_KEYUP:
                    _keys[keyCode] = false;
                    break;

                case SDL_EventType.SDL_CONTROLLERAXISMOTION:
                    UpdateControllerAxis(e);
                    break;

                case SDL_EventType.SDL_CONTROLLERBUTTONDOWN:
                case SDL_EventType.SDL_CONTROLLERBUTTONUP:
                    UpdateControllerButtons(e);
                    break;
            }

            _ = SDL_GetMouseState(out int x, out int y);
            _windowPosition = new() { x = x, y = y };

            SDL_RenderWindowToLogical(MonteEngine.SDL_Renderer, x, y, out float logicalX, out float logicalY);
            _logicalPosition = new() { x = logicalX, y = logicalY };

            _worldPosition = _logicalPosition + Camera.Transform.Position - new SDL_FPoint(RendererSettings.VirtualWidth / 2, RendererSettings.VirtualHeight / 2);

            _ = SDL_GetGlobalMouseState(out int globalX, out int globalY);
            _globalPosition = new() { x = globalX, y = globalY };
        }

        // This is fucking annoying to do but seems you cannot get this any other way.
        // If this ever changes order then this is fucked.
        public static SDL_GameControllerButton ButtonValueAsSDL_GameControllerButton(byte value)
        {
            int index = (int)value;

            List<SDL_GameControllerButton> enumList = new()
            {
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_A,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_B,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_X,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_Y,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_BACK,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_GUIDE,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_START,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSTICK,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSTICK,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_LEFTSHOULDER,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_RIGHTSHOULDER,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_UP,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_DOWN,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_LEFT,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_DPAD_RIGHT,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_MISC1,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_PADDLE1,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_PADDLE2,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_PADDLE3,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_PADDLE4,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_TOUCHPAD,
                SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_MAX,
            };

            // Because of this fucker, 
            // this needs to be done like this. 
            // I don't know what SDL2 people were thinking adding this as -1 when it's a byte value received...
            // C uint8 is C# byte... and bytes don't have -1....
            if (index == -1) return SDL_GameControllerButton.SDL_CONTROLLER_BUTTON_INVALID;
            else return enumList[value];
        }


        // We shall not support presure sensitive buttons.
        internal static void UpdateControllerButtons(SDL_Event e)
        {
            _controllers.TryGetValue(e.cdevice.which, out ControllerInfo? ci);
            if (ci == null)
            {
                Console.WriteLine("controller info not found");
                return;
            }

            SDL_GameControllerButton b = ButtonValueAsSDL_GameControllerButton(e.cbutton.button);

            if (e.type == SDL_EventType.SDL_CONTROLLERBUTTONDOWN)
                ci.buttons[b] = true;
            else if (e.type == SDL_EventType.SDL_CONTROLLERBUTTONUP)
                ci.buttons[b] = false;
        }

        internal static void UpdateControllerAxis(SDL_Event e)
        {
            _controllers.TryGetValue(e.cdevice.which, out ControllerInfo? ci);
            if (ci == null)
            {
                Console.WriteLine("controller info not found");
                return;
            }

            float axisValue = (float)Math.Round(e.caxis.axisValue / 32767.0f, 2);

            if (Math.Abs(axisValue) < ci.deadZone)
                axisValue = 0;

            if (e.caxis.axis == 0)
                ci.leftStick = new(axisValue, ci.leftStick.Y);
            if (e.caxis.axis == 1)
                ci.leftStick = new(ci.leftStick.X, axisValue);

            if (e.caxis.axis == 2)
                ci.rightStick = new(axisValue, ci.rightStick.Y);
            if (e.caxis.axis == 3)
                ci.rightStick = new(ci.rightStick.X, axisValue);

            if (e.caxis.axis == 4)
                ci.leftTrigger = axisValue;
            if (e.caxis.axis == 5)
                ci.rightTrigger = axisValue;
        }

        // Danger danger
        public static ControllerInfo GetControllerInfo(int controllerIndex) => _controllers[controllerIndex];

        /// <summary>
        /// Returns true if any key is pressed
        /// </summary>
        /// <returns>Returns true if any key is pressed</returns>
        public static bool AnyKey() => _keys.Values.Any(x => x == true);

        /// <summary>
        /// Returns true if key is pressed
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>true if key is pressed</returns>
        public static bool GetKey(SDL_Keycode key) => _keys[key];

        /// <summary>
        /// Returns true if key is pressed down this frame
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>True if pressed down this frame</returns>
        public static bool GetKeyDown(SDL_Keycode key) => _keys[key] && !_previousKeys[key];

        /// <summary>
        /// Returns true if key is released this frame
        /// </summary>
        /// <param name="key">key to check</param>
        /// <returns>True if released this frame</returns>
        public static bool GetKeyUp(SDL_Keycode key) => !_keys[key] && _previousKeys[key];

        /// <summary>
        /// Returns true if mouse button mb is being held down.
        /// </summary>
        /// <param name="mb">button to check</param>
        /// <returns>true if mb is held down</returns>
        public static bool GetMouseButton(byte mb)
        {
            // This method is opened because during development some debugging needed to be done. 
            // keeping like this for future debugging.

            // Debug.Log($"{_mouse[mb]}  {_previousMouse[mb]}");

            return _mouse[mb];
        }

        /// <summary>
        /// Returns true if mouse button mb was pressed down this frame.
        /// </summary>
        /// <param name="mb">button to check</param>
        /// <returns>true if mb was pressed down this frame</returns>
        public static bool GetMouseButtonDown(byte mb)
        {
            // This method is opened because during development some debugging needed to be done. 
            // keeping like this for future debugging.

            if (_mouse[mb] == true && _previousMouse[mb] == false)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Returns true if mouse button mb was released this frame.
        /// </summary>
        /// <param name="mb">button to check</param>
        /// <returns>true if mb was released this frame</returns>
        public static bool GetMouseButtonUp(byte mb)
        {
            // This method is opened because during development some debugging needed to be done. 
            // keeping like this for future debugging.

            if (_mouse[mb] == false && _previousMouse[mb] == true)
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        ///  Returns true if mouse is withing given box values of window space.
        /// </summary>
        /// <param name="x">topleft x</param>
        /// <param name="y">topleft y</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <returns>true if mouse is within the window space box</returns>
        public static bool IsMouseWithinWindowBox(float x, float y, float w, float h)
        {
            bool isWithinRectX = _windowPosition.x > x && _windowPosition.x < x + w;
            bool isWithinRectY = _windowPosition.y > y && _windowPosition.y < y + h;
            return isWithinRectX && isWithinRectY;
        }

        /// <summary>
        ///  Returns true if mouse is withing given box values of logical render space.
        /// </summary>
        /// <param name="x">topleft x</param>
        /// <param name="y">topleft y</param>
        /// <param name="w">width</param>
        /// <param name="h">height</param>
        /// <returns>true if mouse is within the logical render space</returns>
        public static bool IsMouseWithinLogicalBox(float x, float y, float w, float h)
        {
            bool isWithinRectX = _logicalPosition.x > x && _logicalPosition.x < x + w;
            bool isWithinRectY = _logicalPosition.y > y && _logicalPosition.y < y + h;
            return isWithinRectX && isWithinRectY;
        }
    }
}