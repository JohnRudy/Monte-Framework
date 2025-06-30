using Monte.Core;
using Monte.Abstractions;
using Monte.Interfaces;

using static SDL.SDL_rect;
using static SDL.SDL_render;
using static SDL.SDL;


namespace Monte
{
    /// <summary>
    /// Main texture render component. Used by Animator to animate spritesheets
    /// </summary>
    public class SpriteRenderer : RenderObject, IComponent
    {
        private MonteBehaviour? parent;
        public MonteBehaviour? Parent
        {
            get => parent;
            set
            {
                parent = value;

                if (parent != null)
                    Transform = parent.Transform;
            }
        }

        /// <summary>
        /// Sets wether this sprite renderer should cull elements out of view. 
        /// </summary>
        public bool DoCulling = false;
        private string? _file;
        public string? File { get => _file; set => _file = value; }

        /// <summary>
        /// SDL_Renderflip reference. Can flip the texture by setting flip value.
        /// </summary>
        public SDL_RendererFlip RenderFlip = SDL_RendererFlip.SDL_FLIP_NONE;

        public SpriteRenderer(string File, int Width, int Height, int x = 0, int y = 0) : base()
        {
            Parent = parent;

            this.File = File;
            this.Width = Width;
            this.Height = Height;

            X = x;
            Y = y;
        }

        public void Initialize()
        {
            Debug.Log($"{this} initialization");
            if (File != null)
            {
                Texture = ContentManager.LoadImage(File);
                Debug.Log($"{this} Texture fetched");
            }

            Renderer.RenderObjects.Add(this);
        }

        public override void Render()
        {
            if (Parent == null) return;
            if (!Parent.Enabled) return;
            if (!Enabled) return;

            SDL_FRect Destination = Camera.GetDestinationFRect(DestinationFRect);
            SDL_Rect Source = SourceRect;

            if (DoCulling)
                if (Camera.IsCulled(Destination.x, Destination.y, Destination.w, Destination.h))
                    return;

            SDL_FPoint RotOrigin = new() { x = Width * Origin.X, y = Height * Origin.Y };

            if (SDL_SetTextureAlphaMod(Texture, Color.a) != 0)
                Debug.Log(SDL_GetError());

            if (SDL_SetTextureColorMod(Texture, Color.r, Color.g, Color.b) != 0)
                Debug.Log(SDL_GetError());

            if (SDL_RenderCopyExF(MonteEngine.SDL_Renderer, Texture, ref Source, ref Destination, Transform.Rotation, ref RotOrigin, RenderFlip) != 0)
                Debug.Log($"There was an issue drawing texture. {SDL_GetError()}");

            // Debug
            // SDL_QueryTexture(Texture, out uint format, out int access, out int w, out int h);
            // Debug.Log($"w:{w}, h:{h}");
        }

        public void Destroy()
        {
            Renderer.RenderObjects.Remove(this);
        }

        public void Update() { }
    }
}