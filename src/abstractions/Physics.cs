using System.Numerics;
using Monte.Abstractions;
using Monte.Core;

namespace Monte.Interfaces
{
    public abstract class Physics : IComponent
    {
        public abstract MonteBehaviour? Parent { get; set; }
        public string? File { get => null; set => throw new NotImplementedException(); }

        /// <summary>
        /// Current velocoty of this object
        /// </summary>
        public Vector2 Velocity { get; set; }

        /// <summary>
        /// Current mass off this object
        /// </summary>
        public float Mass { get; set; }

        public Physics() => Physics2D.PhysicsObjects.Add(this);
        public void Destroy() => Physics2D.PhysicsObjects.Remove(this);
        
        public void Initialize() { }
        public void Update() { }


        /// <summary>
        /// User implemented method to act on physics update
        /// </summary>
        public abstract void PhysicsUpdate();
    }
}