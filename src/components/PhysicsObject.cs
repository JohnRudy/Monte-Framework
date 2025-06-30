using System.Numerics;
using Monte.Abstractions;
using Monte.Core;
using Monte.Interfaces;
using Monte.Settings;


namespace Monte.Components
{
    /// <summary>
    /// SImple physics component to add to montebehaviours. Does simple gravity and dampening.
    /// </summary>
    public class PhysicsObject : Physics, IComponent 
    {
        private Vector2 _lastVelocity = Vector2.Zero;

        /// <summary>
        /// Dampening factor of velocity
        /// </summary>
        public float Dampening = 0.1f;
        /// <summary>
        /// Use physics gravity from monte settings?
        /// </summary>
        public bool UseGravity = true;

        private MonteBehaviour? _parent;
        public override MonteBehaviour? Parent { get => _parent; set => _parent = value; }

        public PhysicsObject() : base() {}

        public override void PhysicsUpdate()
        {
            _lastVelocity = Velocity;

            if (Parent == null) return;

            if (_lastVelocity != Velocity)
                Velocity *= Dampening;
           
            if (UseGravity)
                Velocity += EngineSettings.Gravity * Mass * Time.DeltaTime;
           
            Parent.Transform.Position += Velocity;
        }
    }
}