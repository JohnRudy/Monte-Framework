using System.Numerics;
using Monte.Abstractions;
using Monte.Interfaces;
using Monte.Resource;
using Monte.Scenes;
using static SDL2.SDL;
using static SDL2.SDL_ttf;


namespace Monte.Rendering
{
    public class TextRenderer : RenderProducer, IComponent
    {
        public RenderSpace RenderSpace = RenderSpace.WORLD;
        public SDL_Point Position = new();
        public Vector2 Scale = new();
        public double Rotation;
        public SDL_Color Color = new() { r = 255, g = 255, b = 255, a = 255 };
        public string Text = "";
        public int FontSize;
        public TTFFont TTFFont;
        private SDL_Rect _srcRect;
        Entity _parent;
        public Entity Parent { get => _parent; set => _parent = value; }
        public Vector2 Origin = Vector2.Zero;
        public int Priority = 0;
        public int RenderLayer = 0;
        public TextRenderer(Entity entity, TTFFont font)
        {
            TTFFont = font;
            FontSize = font.PtSize;
            _parent = entity;
            _parent.Components.Add(this);
        }

        private SDL_Rect GetDSTRect(int w, int h)
        {
            SDL_Rect transformed = new()
            {
                x = Position.x,
                y = Position.y,
                w = w,
                h = h
            };

            // Dependig on space settings, we need to determine how the text will interract with other render objects.
            if (RenderSpace == RenderSpace.WORLD || RenderSpace == RenderSpace.SCREEN)
            {
                transformed = Parent.Transform.RectTranslate(transformed);
                Rotation = Parent.Transform.Rotation;
                transformed = SceneManager.CurrentScene.Camera.TransfromDSTRect(transformed);
            }
            return transformed;
        }

        internal override List<RenderObject> Produce(IntPtr SDLRenderer)
        {
            List<RenderObject> result = new();
            // Disgusting way of resizing a font,
            // TTF.SetFontSize does not work.
            if (FontSize != TTFFont.PtSize)
                ReLoadFont();

            if (TTF_SizeText((IntPtr)TTFFont.Font, Text, out int w, out int h) != 0)
                throw new Exception($"{this} could not get text size, is the font name correct?: {SDL_GetError()}");

            _srcRect.w = w;
            _srcRect.h = h;

            SDL_Rect _dstRect = GetDSTRect(w, h);

            IntPtr surf = TTF_RenderText_Blended((IntPtr)TTFFont.Font, Text, Color);
            IntPtr tex = SDL_CreateTextureFromSurface(SDLRenderer, surf);
            SDL_FreeSurface(surf);
            SDL_Point rotOrigin = new()
            {
                x = (int)(_dstRect.w * Origin.X),
                y = (int)(_dstRect.h * Origin.Y)
            };

            result.Add(new RenderObject()
            {
                Texture = tex,
                DSTRect = _dstRect,
                SRCRect = _srcRect,
                Color = Color,
                RotOrigin = rotOrigin,
                RenderSpace = RenderSpace,
                RenderFlip = SDL_RendererFlip.SDL_FLIP_NONE,
                Priority = Priority,
                RenderLayer = RenderLayer,
                Rotation = (float)Rotation
            });

            return result;
        }
        public override void OnEnable() => Renderer.Instance.AddProducer(this);
        public override void OnDisable() => Renderer.Instance.RemoveProducer(this);

        private void ReLoadFont()
        {
            if (ContentManager.IsContentManagerResource(TTFFont.File))
            {
                ContentManager.UnloadFont(TTFFont.Font);
                ContentManager.LoadFont(TTFFont.File, FontSize, out IntPtr _f);
                TTFFont.Font = _f;
            }
            else
            {
                MonteResource.UnloadFont(TTFFont.File);
                MonteResource.LoadFont(TTFFont.File, FontSize, out IntPtr _f);
                TTFFont.Font = _f;
            }
        }
        void IComponent.Initialize()
        {
            if (Enabled)
                Renderer.Instance.AddProducer(this);
        }

        void IComponent.Update() { }

        void IComponent.Destroy()
        {
            Renderer.Instance.RemoveProducer(this);
            ContentManager.UnloadFont(TTFFont.Font);
        }
    }
}