using Monte.Abstractions;
using static SDL2.SDL;
using Monte.Scenes;
using System.Numerics;


namespace Monte.Components
{
    public class CircleCollider : Collider
    {
        private Entity _parent;
        public float Radius = 32;
        public SDL_Point Origin;
        public override SDL_Point WorldCenter
        {
            get
            {
                return new()
                {
                    x = (int)(Origin.x + Parent.Transform.Position.X),
                    y = (int)(Origin.y + Parent.Transform.Position.Y)
                };
            }
        }
       
        public override SDL_Rect WorldBoundingBox
        {
            get
            {
                float minX = WorldCenter.x - Radius;
                float minY = WorldCenter.y - Radius;
                SDL_Rect rect = new()
                {
                    x = (int)minX,
                    y = (int)minY,
                    w = (int)Radius * 2,
                    h = (int)Radius * 2,
                };
                return rect;
            }
        }
        public override Entity Parent { get => _parent; set => _parent = value; }

        public CircleCollider(Entity parent, SDL_Point origin, float radius) : base(parent)
        {
            Shape = Physics.PhysicsShape.Circle;
            _parent = parent;
            Radius = radius / 2;
            Origin = origin;
            _parent.Components.Add(this);
        }
    }
}