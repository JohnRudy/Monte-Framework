using System.Numerics;

using Monte.Lib;
using Monte.Interfaces;
using Monte.Abstractions;

using static SDL.SDL_rect;


namespace Monte.Components
{
    /// <summary>
    /// Main transform component with helpers to easily get the required rects and points that SDL rendering requires.
    /// </summary>
    public class Transform : IComponent
    {
        MonteBehaviour? _parent;
        public MonteBehaviour? Parent { get => _parent; set => _parent = value; }
        public string? File { get => null; set => throw new NotImplementedException(); }

        /// <summary>
        /// Position of this transform in 2D space
        /// </summary>
        public Vector2 Position = Vector2.Zero;

        /// <summary>
        /// Scale of the object
        /// </summary>
        public float Scale = 1;

        /// <summary>
        /// Rotation of the object.
        /// </summary>
        public double Rotation = 0;

        public Transform() { }


        public void Initialize() { }
        public void Update() { }
        public void Destroy() { }

    }
}