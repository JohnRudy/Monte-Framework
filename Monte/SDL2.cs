using System.Numerics;
using System.Runtime.InteropServices;
using static SDL2.SDL;


namespace SDL
{
    public static class SDL2
    {
        public static int SDL_FALSE = 0;
        public static int SDL_TRUE = 1;
    }

    public static class SDL2TtfFixes
    {
        [DllImport("SDL2_ttf", CallingConvention = CallingConvention.Cdecl, EntryPoint = "TTF_Linked_Version")]
        private static extern IntPtr INTERNAL_TTF_LinkedVersion();

        public static SDL_version TTF_Linked_Version()
        {
#pragma warning disable CS8605
            return (SDL_version)Marshal.PtrToStructure(INTERNAL_TTF_LinkedVersion(), typeof(SDL_version));
#pragma warning restore CS8605
        }
    }

    public static class SDL2Extension
    {
        public static Vector2 ToVector2(this SDL_Point point) => new Vector2(point.x, point.y);
        public static SDL_Point ToSDLPoint(this Vector2 point) => new() { x = (int)Math.Round(point.X), y = (int)Math.Round(point.Y) };
        public static SDL_Point Addition(this SDL_Point a, SDL_Point b) => new() { x = a.x + b.x, y = a.y + b.y };
        public static SDL_FPoint Addition(this SDL_FPoint a, SDL_FPoint b) => new() { x = a.x + b.x, y = a.y + b.y };
    }
}