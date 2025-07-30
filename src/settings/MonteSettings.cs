using System.Numerics;

using static SDL.SDL_render;
using static SDL.SDL_video;
using static SDL.SDL;
using static SDL.SDL_blendmode;
using static SDL.SDL_image;
using static SDL.SDL_mixer;

using Monte.Core;

namespace Monte.Settings
{
    public static class DebugSettings
    {
        /// <summary>
        /// Render all colliders in current scene. 
        /// </summary>
        public static bool ShowColliders = false;

        /// <summary>
        /// Render the origin point (0,0) of the world
        /// </summary>
        public static bool DebugOrigin = false;

        /// <summary>
        /// Debugs logical cursor positions and lengths. 
        /// </summary>
        public static bool CursorDebug = false;

        /// <summary>
        /// Show any debug messages.
        /// </summary>
        public static bool ShowDebugMessages = true;

        /// <summary>
        /// Show current frames per second, wait time. 
        /// </summary>
        public static bool FrameInfo = false;
    }

    /// <summary>
    /// Main window settings
    /// </summary>
    public static class WindowSettings
    {
        /// <summary>
        /// Keep the window focused at all time.
        /// </summary>
        public static bool AlwaysKeepFocus = false;
        
        /// <summary>
        /// Initial windowWidth
        /// </summary>
        public static int WindowWidth = 640;
        /// <summary>
        /// Initial WidnwoHeight
        /// </summary>
        public static int WindowHeight = 360;
        /// <summary>
        /// SDL WindowFlags, see SDL Wiki
        /// </summary>
        public static SDL_WindowFlags WindowFlags = SDL_WindowFlags.SDL_WINDOW_SHOWN | SDL_WindowFlags.SDL_WINDOW_MOUSE_CAPTURE;
        /// <summary>
        /// Initial windowposition during initialization
        /// </summary>
        public static uint WindowPositionX = SDL_WINDOWPOS_UNDEFINED;
        /// <summary>
        /// Initial windowposition during initialization
        /// </summary>
        public static uint WindowPositionY = SDL_WINDOWPOS_UNDEFINED;
        public static string WindowTitle = "Monte";
    }


    /// <summary>
    /// Main renderer settings
    /// </summary>
    public static class RendererSettings
    {
        /// <summary>
        /// Virtual width of screenspace
        /// </summary>
        public static int VirtualWidth = 640;
        /// <summary>
        /// Virtual height of sccreenspace
        /// </summary>
        public static int VirtualHeight = 360;
        /// <summary>
        /// Margin of camera culling of objects
        /// </summary>
        public static int Margin = 128;
        /// <summary>
        /// SDL_image init flags see SDL2_image wiki
        /// </summary>
        public static IMG_InitFlags ImgInitFlags = IMG_InitFlags.IMG_INIT_PNG;
        /// <summary>
        /// Render order of sprites on screen
        /// </summary>
        public static RenderOrder RenderOrder = RenderOrder.LEFTDOWN;
        /// <summary>
        /// Blendmode the renderer is set to during initialization.
        /// </summary>
        public static SDL_BlendMode BlendMode = SDL_BlendMode.SDL_BLENDMODE_BLEND;
        /// <summary>
        /// main SDL2 RenderFLags see SDL2 wiki
        /// </summary>
        public static SDL_RendererFlags RenderFlags = SDL_RendererFlags.SDL_RENDERER_ACCELERATED | SDL_RendererFlags.SDL_RENDERER_PRESENTVSYNC;
    }

    /// <summary>
    /// Main engine settings
    /// </summary>
    public static class EngineSettings
    {
        /// <summary>
        /// Boolean that allows showing of logos
        /// </summary>
        public static bool ShowMonteLogo = true;
        /// <summary>
        /// Desired frames per second of the application
        /// </summary>
        public static uint DesiredFPS = 60;
        /// <summary>
        /// World gravity. y+ equals down.
        /// </summary>
        public static Vector2 Gravity = new(0, 9.82f);
        /// <summary>
        /// Main SDL2 initialization flags see SDL2 wiki
        /// </summary>
        public static uint SDLInitFlags = SDL_INIT_EVERYTHING;
        /// <summary>
        /// content folder name and path. 
        /// </summary>
        public static string ContentFolder = "content";
        /// <summary>
        /// Main SDL2_mixer init flags see SDL2_mixer wiki
        /// </summary>
        public static MIX_InitFlags MixerInitFlags = MIX_InitFlags.MIX_INIT_OGG | MIX_InitFlags.MIX_INIT_WAVPACK;
    }
}