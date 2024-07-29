using static SDL2.SDL;
using Monte.Components;
using Monte.Abstractions;
using Monte.Audio;


namespace Monte
{
    public class Camera : Entity
    {
        public override void OnInitialize()
        {
            _ = new AudioListener(this);
        }

        public SDL_Rect TransfromDSTRect(SDL_Rect rect)
        {
#pragma warning disable
            int newX = (int)Math.Round(rect.x - Transform.Position.X + (RendererSettings.VirtualWidth / 2));
            int newY = (int)Math.Round(rect.y - Transform.Position.Y + (RendererSettings.VirtualHeight / 2));
            int newWidth = (int)Math.Round(rect.w + Transform.Scale.X);
            int newHeight = (int)Math.Round(rect.h + Transform.Scale.Y);
            return new() { x = (int)newX, y = (int)newY, h = newHeight, w = newWidth };
#pragma warning restore
        }

        public SDL_Point TransformPoint(SDL_Point point)
        {
            int newX = (int)Math.Round(point.x - Transform.Position.X + (RendererSettings.VirtualWidth / 2));
            int newY = (int)Math.Round(point.y - Transform.Position.Y + (RendererSettings.VirtualHeight / 2));
            return new SDL_Point() { x = newX, y = newY };
        }
    }
}