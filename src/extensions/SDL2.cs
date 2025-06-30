using System.Collections.Concurrent;
using System.Numerics;
using static SDL.SDL_rect;

namespace Monte.Extensions
{
    /// <summary>
    /// Main generic Extensions class for internal classes 
    /// </summary>
    public static class Extensions
    {
        public static Vector2 ToVector2(this SDL_Point point) => new Vector2(point.x, point.y);
        public static Vector2 ToVector2(this SDL_FPoint point) => new Vector2(point.x, point.y);
        public static SDL_Point ToSDLPoint(this Vector2 point) => new() { x = (int)Math.Round(point.X), y = (int)Math.Round(point.Y) };
        public static SDL_FPoint ToSDLFPoint(this Vector2 point) => new() { x = point.X, y = point.Y };

        public static SDL_Point Addition(this SDL_Point a, SDL_Point b) => new() { x = a.x + b.x, y = a.y + b.y };
        public static SDL_Point Addition(this SDL_Point a, SDL_FPoint b) => new() { x = (int)(a.x + b.x), y = (int)(a.y + b.y) };
        public static SDL_Point Addition(this SDL_FPoint a, SDL_Point b) => new() { x = (int)(a.x + b.x), y = (int)(a.y + b.y) };
        public static SDL_Point Addition(this SDL_FPoint a, SDL_FPoint b) => new() { x = (int)(a.x + b.x), y = (int)(a.y + b.y) };

        public static SDL_FPoint FAddition(this SDL_FPoint a, SDL_FPoint b) => new() { x = a.x + b.x, y = a.y + b.y };
        public static SDL_FPoint FAddition(this SDL_FPoint a, SDL_Point b) => new() { x = a.x + b.x, y = a.y + b.y };
        public static SDL_FPoint FAddition(this SDL_Point a, SDL_FPoint b) => new() { x = a.x + b.x, y = a.y + b.y };
        public static SDL_FPoint FAddition(this SDL_Point a, SDL_Point b) => new() { x = a.x + b.x, y = a.y + b.y };

        // Expects both being in same space
        public static bool PointOverRect(this SDL_FRect frect, SDL_FPoint point)
        {
            return frect.x <= point.x && frect.x + frect.w >= point.x && frect.y <= point.y && frect.y + frect.h >= point.y;
        }
        public static bool PointOverRect(this SDL_Rect frect, SDL_Point point)
        {
            return frect.x <= point.x && frect.x + frect.w >= point.x && frect.y <= point.y && frect.y + frect.h >= point.y;
        }
    }
}