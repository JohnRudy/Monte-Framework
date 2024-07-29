using static SDL2.SDL_image;
using static SDL2.SDL;
using System.Runtime.CompilerServices;


namespace Monte
{
    public static class WindowSettings
    {
        public static int WindowWidth = 640;
        public static int WindowHeight = 480;
        public static SDL_WindowFlags WindowFlags = SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_MOUSE_CAPTURE;
        public static int WindowIndex = -1;
        public static int WindowPositionX = SDL_WINDOWPOS_UNDEFINED;
        public static int WindowPositionY = SDL_WINDOWPOS_UNDEFINED;
        public static string WindowTitle = "Monte";
    }

    public static class DebugSettings
    {
        public static bool DrawDebugGizmos = false;
        public static bool DisplayInformation = false;
    }

    public static class RendererSettings
    {
        public static int VirtualWidth = 640;
        public static int VirtualHeight = 480;
        public static int Margin = 128;
        public static int RendererIndex = -1;
        public static IMG_InitFlags ImgInitFlags = IMG_InitFlags.IMG_INIT_PNG;
        public static RenderOrder RenderOrder = RenderOrder.LEFTDOWN;
        public static SDL_BlendMode BlendMode = SDL_BlendMode.SDL_BLENDMODE_BLEND;
        public static SDL_RendererFlags RenderFlags = SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
    }

    public static class EngineSettings
    {
        public static bool ShowLogoAtStart = true;
        public static bool ShowEngineLogo = true;
        public static float LogoDurationsInSeconds = 5;
        public static SDL_Color LogoBackgroundColor = new SDL_Color() { r = 21, g = 29, b = 40, a = 255 };
        public static List<Tuple<string, int, int>> LogoFiles = new() { };
        public static int DesiredFPS = 60;
        public static uint SDLInitFlags = SDL_INIT_TIMER | SDL_INIT_VIDEO | SDL_INIT_EVENTS | SDL_INIT_AUDIO;
    }

    public static class PhysicsSettings
    {
        public static float Gravity = 9.82f;
        public static float FixedDeltaTime = 0.2f;
        public static bool UsePhysics = false;
    }
}