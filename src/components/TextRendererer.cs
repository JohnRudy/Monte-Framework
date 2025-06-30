using Monte.Abstractions;
using Monte.Interfaces;
using Monte.Core;
using Monte.Components;

using static SDL.SDL_rect;
using static SDL.SDL_render;
using static SDL.SDL_surface;
using static SDL.SDL_ttf;
using static SDL.SDL;


namespace Monte.Rendering
{
    /// <summary>
    /// Main text renderer. 
    /// </summary>
    public class TextRenderer : RenderObject, IComponent
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
        /// Text to render
        /// </summary>
        public string Text = "";

        /// <summary>
        /// Font size
        /// </summary>
        public int FontSize;
        private string? _file;
        public string? File { get => _file; set => _file = value; }

        /// <summary>
        /// Main SDL_Font pointer of loaded TTF font.
        /// </summary>
        public IntPtr Font;

        /// <summary>
        /// Flip the rendered texture by value.
        /// </summary>
        public SDL_RendererFlip RenderFlip = SDL_RendererFlip.SDL_FLIP_NONE;

        /// <summary>
        /// Determines wheter this text renderer gets any frustrum culling
        /// </summary>
        public bool DoCulling = false;

        public TextRenderer(string file, int fontSize) : base()
        {
            File = file;
            FontSize = fontSize;
        }

        public void Initialize()
        {
            if (File != null)
                Font = ContentManager.LoadFont(File, FontSize);

            Renderer.RenderObjects.Add(this);
        }


        public override void Render()
        {
            if (Parent == null) return;

            if (TTF_SetFontSize(Font, FontSize) != 0)
                throw new Exception($"{this} could not set font size, is the font name correct?: {SDL_GetError()}");

            if (TTF_SizeText(Font, Text, out int w, out int h) != 0)
                throw new Exception($"{this} could not get text size, is the font name correct?: {SDL_GetError()}");

            X = 0;
            Y = 0;
            Width = w;
            Height = h;

            if (DoCulling)
                if (Camera.IsCulled(X, X, Width, Height))
                    return;

            IntPtr surf = TTF_RenderText_Blended(Font, Text, Color);
            IntPtr tex = SDL_CreateTextureFromSurface(MonteEngine.SDL_Renderer, surf);
            SDL_FreeSurface(surf);
            SDL_FPoint rotOrigin = new()
            {
                x = Width / 2,
                y = Height / 2
            };

            if (SDL_SetTextureAlphaMod(tex, Color.a) != 0)
                Debug.Log(SDL_GetError());

            if (SDL_SetTextureColorMod(tex, Color.r, Color.g, Color.b) != 0)
                Debug.Log(SDL_GetError());

            SDL_Rect source = SourceRect;
            SDL_FRect destination = DestinationFRect;

            if (SDL_RenderCopyExF(MonteEngine.SDL_Renderer, tex, ref source, ref destination, Transform.Rotation, ref rotOrigin, RenderFlip) != 0)
                Debug.Log($"There was an issue drawing texture. {SDL_GetError()}");

            SDL_DestroyTexture(tex);
        }

        public void Update() { }
        public void Destroy()
        {
            Renderer.RenderObjects.Remove(this);
        }
    }
}