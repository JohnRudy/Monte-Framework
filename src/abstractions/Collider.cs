using static SDL.SDL_rect;

using Monte.Core;
using Monte.Interfaces;

using System.Numerics;


namespace Monte.Abstractions
{
    /// <summary>
    /// Abstract class inherited by colliders. 
    /// Usage should be limited to only use the component colliders rather than the abstract class. 
    /// This class shows what the other colliders have in common. 
    /// </summary>
    public abstract class Collider : Enablable, IComponent
    {
        /// <summary>
        /// The currently supported physics shape.
        /// </summary>
        public PhysicsShape Shape = PhysicsShape.Rectangle;
        public abstract MonteBehaviour? Parent { get; set; }

        /// <summary>
        /// Is this collider a trigger area. 
        /// </summary>
        public bool IsTrigger = false;
        public string? File { get => null; set => throw new NotImplementedException(); }

        /// <summary>
        /// Is this collider static in world and does not move
        /// </summary>
        public bool IsStatic = false;

        /// <summary>
        /// User method to implement. Must contain all the vertices of the collider. 
        /// </summary>
        public abstract SDL_FRect WorldBoundingBox { get; }

        /// <summary>
        /// Usermethod to implement. Must be the center of mass of the collider.
        /// </summary>
        public abstract SDL_FPoint WorldCenter { get; }

        /// <summary>
        /// List of other colliders this collider is colliding or interacting with.
        /// </summary>
        public List<Collider> OtherColliders { get; private set; } = new List<Collider>();

        /// <summary>
        /// Boolean if this collider is colliding with anything. 
        /// </summary>
        public bool IsColliding => OtherColliders.Any();

        /// <summary>
        /// Simplyfied way of getting is this collider active
        /// </summary>
        public bool IsEnabled => Parent is not null && Parent.Enabled && Enabled;

        public Collider() { }

        public void Initialize() { }

        internal void AddCollision(Collider collider)
        {
            if (Parent == null) return;

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
        internal void RemoveCollision(Collider collider)
        {
            if (OtherColliders.Contains(collider))
            {
                OtherColliders.Remove(collider);

                if (Parent == null) return;

                if (IsTrigger || collider.IsTrigger) Parent.OnTriggerExit(collider);
                else Parent.OnCollisionExit(collider);
            }
        }

        internal void UpdateCollision(Collider collider)
        {
            if (Parent == null) return;

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

        public void Update() { }
        public void Destroy() { }
    }
}