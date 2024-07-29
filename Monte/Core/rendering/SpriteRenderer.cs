using System.Numerics;
using Monte.Abstractions;
using Monte.Interfaces;
using Monte.Resource;
using Monte.Scenes;
using static SDL2.SDL;


namespace Monte.Rendering
{
    public class SpriteRenderer : RenderProducer, IComponent
    {
        public RenderSpace RenderSpace = RenderSpace.WORLD;
        public Vector3 Position = new();
        public Vector3 Scale = new(1, 1, 1);
        public double Rotation = 0;
        public int RenderLayer = 0;
        public int Priority = 0;
        private Sprite? _sprite = null;
        public Sprite? Sprite
        {
            get => _sprite;
            set
            {
                _sprite = value;
                if (_sprite != null)
                    SRCRect = new SDL_Rect() { x = _sprite.X, y = _sprite.Y, w = _sprite.Width, h = _sprite.Height };
            }
        }
        Entity _parent;
        public Entity Parent { get => _parent; set => _parent = value; }
        public Vector2 Origin = Vector2.Zero;
        public SDL_Rect SRCRect;
        public SDL_RendererFlip RenderFlip;
        public SDL_Color Color = new() { r = 255, g = 255, b = 255, a = 255 };

        public SpriteRenderer(Entity parent)
        {
            _parent = parent;
            _parent.Components.Add(this);
        }

        public SpriteRenderer(Entity parent, Sprite sprite)
        {
            Sprite = sprite;
            SRCRect = sprite.GetSRCRect();
            _parent = parent;
            _parent.Components.Add(this);
        }

        public override void OnEnable() => Renderer.Instance.AddProducer(this);
        public override void OnDisable() => Renderer.Instance.RemoveProducer(this);

        private SDL_Rect GetDSTRect()
        {
            SDL_Rect transformed = new()
            {
                x = (int)Position.X,
                y = (int)Position.Y,
                w = SRCRect.w + (int)Scale.Y,
                h = SRCRect.h + (int)Scale.Y
            };

            if (RenderSpace == RenderSpace.WORLD || RenderSpace == RenderSpace.SCREEN)
            {
                transformed = Parent.Transform.RectTransformation(transformed);
                Rotation = Parent.Transform.Rotation;
                transformed = SceneManager.CurrentScene.Camera.TransfromDSTRect(transformed);
            }

            // Top Left == 0,0, Bottom Right == 1,1
            // So we need to move the destination TO the top left FROM bottom right
            transformed.x -= (int)(transformed.w * Origin.X);
            transformed.y -= (int)(transformed.h * Origin.Y);
            return transformed;
        }

        internal override List<RenderObject> Produce(IntPtr SDLRenderer)
        {
            if (Sprite != null)
            {
                SDL_Rect dstRect = GetDSTRect();
                SDL_Point rotOrigin = new()
                {
                    x = (int)(dstRect.w * Origin.X),
                    y = (int)(dstRect.h * Origin.Y)
                };

                return new(){
                    new RenderObject(){
                        Texture = Sprite.Tex,
                        DSTRect = dstRect,
                        SRCRect = SRCRect,
                        Rotation = (float)Parent.Transform.Rotation,
                        RenderFlip = RenderFlip,
                        RotOrigin = rotOrigin,
                        Priority = Priority,
                        RenderLayer = RenderLayer,
                        Color = Color,
                        RenderSpace = RenderSpace
                    }
                };
            }
            return new();
        }

        void IComponent.Initialize()
        {
            if (Enabled)
                Renderer.Instance.AddProducer(this);
        }

        void IComponent.Update() { }

        void IComponent.Destroy()
        {
            if (Sprite == null) return;
            Renderer.Instance.RemoveProducer(this);
            if (ContentManager.IsContentManagerResource(Sprite.File))
            {
                ContentManager.UnloadTexture(Sprite.File);
            }
            else
            {
                MonteResource.UnloadTexture(Sprite.File);
            }
        }
    }
}