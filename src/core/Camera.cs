using Monte.Settings;

using static SDL.SDL_rect;

using Monte.Components;
using Monte.Core;


namespace Monte
{
    /// <summary>
    /// Main Camera class.
    /// </summary>
    public static class Camera
    {
        /// <summary>
        /// Camera Transform
        /// </summary>
        public static Transform Transform = new();

        /// <summary>
        /// Returns a boolean value of if the given point is within the allowed culling limits based on camera position and margins. 
        /// Values given are expected to be normalized to spcreen space.
        /// </summary>
        /// <param name="point">point to check</param>
        /// <param name="width">width of the sprite</param>
        /// <param name="height">height of the sprite</param>
        /// <returns></returns>
        public static bool IsCulled(float x, float y, float width, float height)
        {
            if (x + width < -RendererSettings.Margin || x - width > RendererSettings.VirtualWidth + RendererSettings.Margin ||
                y + height < -RendererSettings.Margin || y - height > RendererSettings.VirtualHeight + RendererSettings.Margin)
                return true;

            return false;
        }

        public static SDL_FRect GetDestinationFRect(SDL_FRect rect)
        {
            float newX = rect.x - Transform.Position.X;
            float newY = rect.y - Transform.Position.Y;
            float newWidth = rect.w + Transform.Scale;
            float newHeight = rect.h + Transform.Scale;
            SDL_FPoint norm = Renderer.FNormalizeScreenSpace(new(newX, newY));
            return new() { x = norm.x, y = norm.y, h = newHeight, w = newWidth };
        }

        public static SDL_FPoint GetDestinationFPoint(SDL_FPoint point)
        {
            SDL_FPoint norm = Renderer.FNormalizeScreenSpace(
                new(
                    x: point.x - Transform.Position.X,
                    y: point.y - Transform.Position.Y
                )
            );
            return norm;
        }
    }
}