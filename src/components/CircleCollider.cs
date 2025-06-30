using Monte.Abstractions;
using Monte.Core;

using static SDL.SDL_rect;


namespace Monte.Components
{
    public class CircleCollider : Collider
    {
        private MonteBehaviour? _parent;
        public override MonteBehaviour? Parent { get => _parent; set => _parent = value; }

        /// <summary>
        /// Radius of this circle collider in logical pixels
        /// </summary>
        public float Radius = 32;

        /// <summary>
        /// Origin point of this collider.
        /// </summary>
        public SDL_Point Origin;

        public override SDL_FPoint WorldCenter
        {
            get
            {
                if (Parent == null) throw new Exception("Parent is null");
                return new()
                {
                    x = Origin.x + Parent.Transform.Position.X,
                    y = Origin.y + Parent.Transform.Position.Y
                };
            }
        }
       
        public override SDL_FRect WorldBoundingBox
        {
            get
            {
                float minX = WorldCenter.x - Radius;
                float minY = WorldCenter.y - Radius;
                SDL_FRect rect = new()
                {
                    x = minX,
                    y = minY,
                    w = Radius * 2,
                    h = Radius * 2,
                };
                return rect;
            }
        }

        public CircleCollider(SDL_Point origin, float radius) : base()
        {
            Shape = PhysicsShape.Circle;
            Radius = radius / 2;
            Origin = origin;
        }
    }
}