using System.Numerics;
using Monte.Interfaces;
using Monte.Abstractions;
using static SDL2.SDL;


namespace Monte.Components
{
    public class Transform : IComponent
    {
        public Vector3 Position = Vector3.Zero;
        public Vector2 Scale = Vector2.One;
        public double Rotation = 0;
        Entity _parent;
        public Entity Parent { get => _parent; set => _parent = value; }

        public Transform(Entity parent)
        {
            _parent = parent;
            _parent.Components.Add(this);
        }

        public SDL_Rect RectTranslate(SDL_Rect rect)
        {
            return new()
            {
                x = (int)(rect.x + Position.X),
                y = (int)(rect.y + Position.Y),
                w = rect.w,
                h = rect.h
            };
        }

        public SDL_Rect RectScale(SDL_Rect rect)
        {
            return new()
            {
                x = rect.x,
                y = rect.y,
                w = rect.w + (int)Scale.X + (int)Position.Z,
                h = rect.h + (int)Scale.Y + (int)Position.Z
            };
        }

        public SDL_Rect RectTransformation(SDL_Rect rect)
        {
            SDL_Rect trans = RectTranslate(rect);
            SDL_Rect scaled = RectScale(trans);
            return scaled;
        }

        public void Update() { OnUpdate(); }
        public virtual void OnUpdate() { }
        public void Destroy() { OnDestroy(); }
        public virtual void OnDestroy() { }
        public void Initialize() => OnInitialize();
        public virtual void OnInitialize() { }
    }
}