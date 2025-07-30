using System.Numerics;

using Monte.Components;
using Monte.Core;

using static SDL.SDL_rect;
using static SDL.SDL_pixels;


namespace Monte.Abstractions
{
    /// <summary>
    /// RenderObject abstraction. Tells the main Renderer to call this object during render updates. 
    /// </summary>
    public abstract class RenderObject : Enablable
    {
        /// <summary>
        /// Color that gets multplied with he sprite. 
        /// </summary>
        public SDL_Color Color = new SDL_Color() { r = 255, g = 255, b = 255, a = 255 };

        /// <summary>
        /// SDL_Texture pointer reference
        /// </summary>
        public IntPtr Texture = IntPtr.Zero;

        /// <summary>
        /// Priority of this render object. Higher equals ontop of others. 
        /// </summary>
        public int Priority = 0;

        /// <summary>
        /// Width of the resulting texture
        /// </summary>
        public float Width;

        /// <summary>
        /// Height of the resulting texture
        /// </summary>
        public float Height;

        /// <summary>
        /// X coordinate of the sprites topleft corner in the spritesheet
        /// </summary>
        public float X = 0;

        /// <summary>
        /// Y coordinate of the sprites topleft corner in the spritesheet
        /// </summary>
        public float Y = 0;

        /// <summary>
        /// Origin point of the texture
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// Parent transform
        /// </summary>
        public Transform Transform { get; set; }

        /// <summary>   
        /// Local transform object to move object in screenspace
        /// </summary>
        public Transform LocalTransform { get; set; }


        /// <summary>
        /// Source rect of the texture in the file.
        /// </summary>
        public SDL_Rect SourceRect
        {
            get => new(
                (int)X,
                (int)Y,
                (int)Width,
                (int)Height
            );
        }

        /// <summary>
        /// Destination FRect in screen/world space
        /// </summary>
        public SDL_FRect DestinationFRect
        {
            get => new(
                LocalTransform.Position.X + Transform.Position.X - (Origin.X * Width),
                LocalTransform.Position.Y + Transform.Position.Y - (Origin.Y * Height),
                LocalTransform.Scale + Width * Transform.Scale,
                LocalTransform.Scale + Height * Transform.Scale
            );
        }

        public RenderObject()
        {
            Transform = new();
            LocalTransform = new();
            // Renderer.RenderObjects.Add(this);
        }

        public RenderObject(int priority)
        {
            Transform = new();
            LocalTransform = new();
            Priority = priority;
            // Renderer.RenderObjects.Add(this);
        }

        /// <summary>
        /// User implemented method. Called each render update.
        /// </summary>
        public abstract void Render();
    }
}