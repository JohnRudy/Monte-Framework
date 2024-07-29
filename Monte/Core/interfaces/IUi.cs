using System.Numerics;
using static SDL2.SDL;


namespace Monte.Interfaces
{
    public interface IUi
    {
        SDL_Point ScreenPosition { get; set; }
        Vector2 Scale { get; set; }
        float Rotation { get; set; }
        bool IsInteractable { get; set; }
        IUi? ChainNext { get; set; }
        IUi? ChainPrevious { get; set; }
        SDL_Rect InteractionArea { get; }
        
        public void Select(bool isSelected);
        public void Pressed(bool isPressed);
    }
}