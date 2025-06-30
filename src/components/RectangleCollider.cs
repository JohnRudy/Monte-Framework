using Monte.Abstractions;
using Monte.Core;

using static SDL.SDL_rect;

using System.Numerics;


namespace Monte.Components
{
    public class RectangleCollider : Collider
    {
        private MonteBehaviour? parent;
        public override MonteBehaviour? Parent { get => parent; set => parent = value; }

        public Vector2 Origin { get; set; }
        private SDL_FRect _rect;

        public override SDL_FPoint WorldCenter
        {
            get
            {
                if (Parent == null) throw new Exception("Parent is null");
                return new SDL_FPoint()
                {
                    x = _rect.x + Parent.Transform.Position.X,
                    y = _rect.y + Parent.Transform.Position.Y,
                };
            }
        }

        /// <summary>
        /// Acts as the actual collider. 
        /// </summary>
        public override SDL_FRect WorldBoundingBox
        {
            get
            {
                if (Parent == null) throw new Exception("Parent is null");

                SDL_FRect positioned = new()
                {
                    x = _rect.x + Parent.Transform.Position.X - (Origin.X * _rect.w),
                    y = _rect.y + Parent.Transform.Position.Y - (Origin.Y * _rect.h),
                    w = _rect.w,
                    h = _rect.h,
                };
                return positioned;
            }
        }

        public RectangleCollider(SDL_FRect rect, Vector2 origin)
        {
            Origin = origin;
            _rect = rect;
            Shape = PhysicsShape.Rectangle;
        }
    }
}