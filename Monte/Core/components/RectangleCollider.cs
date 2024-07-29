using Monte.Abstractions;
using Monte.Scenes;
using static SDL2.SDL;
using Monte.Physics;
using System.Numerics;


namespace Monte.Components
{
    public class RectangleCollider : Collider
    {
        private Entity _parent;
        public override Entity Parent { get => _parent; set => _parent = value; }
        public Vector2 Origin { get; set; }
        private SDL_Rect _rect;
        public override SDL_Point WorldCenter
        {
            get
            {
                return new SDL_Point()
                {
                    x = (int)(_rect.x + Parent.Transform.Position.X + Origin.X + _rect.w / 2),
                    y = (int)(_rect.y + Parent.Transform.Position.Y + Origin.Y + _rect.h / 2),
                };
            }
        }
        public override SDL_Rect WorldBoundingBox
        {
            get
            {
                SDL_Rect positioned = new()
                {
                    x = (int)(_rect.x + Parent.Transform.Position.X + Origin.X),
                    y = (int)(_rect.y + Parent.Transform.Position.Y + Origin.Y),
                    w = _rect.w,
                    h = _rect.h,
                };
                return positioned;
            }
        }

        public RectangleCollider(Entity parent, SDL_Rect rect, Vector2 origin) : base(parent)
        {
            Origin = origin;
            _parent = parent;
            _rect = rect;
            Shape = PhysicsShape.Rectangle;

            _parent.Components.Add(this);
        }
    }
}