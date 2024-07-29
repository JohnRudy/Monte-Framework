using Monte.Interfaces;
using static SDL2.SDL;
using Monte.Physics;
using System.Numerics;


namespace Monte.Abstractions
{
    public abstract class Collider : Enablable, IComponent
    {
        public PhysicsShape Shape = PhysicsShape.Rectangle;

        public abstract Entity Parent { get; set; }
        public bool IsTrigger = false;
        public bool IsStatic = false;
        public abstract SDL_Rect WorldBoundingBox { get; }
        public abstract SDL_Point WorldCenter { get; }
        public List<Collider> OtherColliders { get; private set; } = new List<Collider>();
        public bool IsColliding => OtherColliders.Any();

        // Simplifying getting colliders that are active
        public bool IsEnabled => Parent is not null && Parent.Enabled && Enabled;

        public Collider(Entity parent)
        {
            Parent = parent;
        }

        public void Initialize() { }

        public void AddCollision(Collider collider)
        {
            if (!OtherColliders.Contains(collider))
            {
                OtherColliders.Add(collider);

                if (IsTrigger || collider.IsTrigger) Parent?.OnTriggerEnter(collider);
                else
                {
                    Parent.OnCollision(collider);
                    if (!IsStatic)
                    {
                        Physics2D.ResolveCollision(this, collider, out Vector2 move);
                    }
                }
            }
            else
                UpdateCollision(collider);
        }
        public void RemoveCollision(Collider collider)
        {
            if (OtherColliders.Contains(collider))
            {
                OtherColliders.Remove(collider);

                if (IsTrigger || collider.IsTrigger) Parent.OnTriggerExit(collider);
                else Parent.OnCollisionExit(collider);
            }
        }

        public void UpdateCollision(Collider collider)
        {
            if (OtherColliders.Contains(collider))
            {
                if (IsTrigger || collider.IsTrigger) Parent.OnTriggerStay(collider);
                else
                {
                    Parent.OnCollisionStay(collider);
                    if (!IsStatic)
                    {
                        Physics2D.ResolveCollision(this, collider, out Vector2 move);
                    }
                }
            }
        }

        // public virtual void OnRenderGizmos(IntPtr sdlRenderer) { }CircleCollider

        public void Update()
        {
        }

        public void Destroy()
        {
        }
    }
}