using System.Numerics;
using Monte.Abstractions;
using Monte.Interfaces;
using Monte.Rendering;
using static SDL2.SDL;


namespace Monte.UI
{
    public abstract class Button : RenderProducer, IComponent, IUi
    {
        private bool _IsInteractable = true;
        public bool IsInteractable
        {
            get => _IsInteractable;
            set
            {
                if (value != _IsInteractable)
                {
                    _IsInteractable = value;
                }
            }
        }
        private bool _isPressed = false;
        private bool _isSelected = false;

        private SDL_Point _ScreenPosition = new() { x = 0, y = 0 };
        private Vector2 _Scale = Vector2.One;
        private float _Rotation = 0;
        public SDL_Point ScreenPosition { get => _ScreenPosition; set => _ScreenPosition = value; }
        public Vector2 Scale { get => _Scale; set => _Scale = value; }
        public float Rotation { get => _Rotation; set => _Rotation = value; }

        public Sprite Image;
        public SDL_Rect? ButtonRect;
        public SDL_Color Color = new() { r = 255, g = 255, b = 255, a = 255 };
        public SDL_RendererFlip RendererFlip = SDL_RendererFlip.SDL_FLIP_NONE;
        public Vector2 Origin = new(0.5f, 0.5f);

        private Entity _Parent;
        public Entity Parent { get => _Parent; set => _Parent = value; }
        private IUi? _ChainNext;
        private IUi? _ChainPrevious;
        public IUi? ChainNext { get => _ChainNext; set => _ChainNext = value; }
        public IUi? ChainPrevious { get => _ChainPrevious; set => _ChainPrevious = value; }

        public SDL_Rect InteractionArea => new() { x = ScreenPosition.x, y = ScreenPosition.y, w = Image.Width, h = Image.Height };

        public EventHandler? onReleaseEvent;
        public EventHandler? onPressedEvent;
        public EventHandler? onHeldEvent;

        public Button(Entity Parent, Sprite? image = null)
        {
            _Parent = Parent;
            Image = image ?? new(null, 64, 64, 0, 0);
            Parent.Components.Add(this);
        }

        private SDL_Rect GetDSTRect()
        {
            return new()
            {
                x = ScreenPosition.x,
                y = ScreenPosition.y,
                w = Image.Width,
                h = Image.Height
            };
        }

        internal override List<RenderObject> Produce(IntPtr SDLRenderer)
        {

            SDL_Rect dstRect = GetDSTRect();
            SDL_Rect srcRect = Image.GetSRCRect();
            SDL_Point rotOrigin = new()
            {
                x = (int)(dstRect.x * Origin.X),
                y = (int)(dstRect.y * Origin.Y)
            };

            return new(){
                new RenderObject(){
                    Texture = Image.Tex,
                    DSTRect = dstRect,
                    SRCRect = srcRect,
                    Rotation = (float)Parent.Transform.Rotation,
                    RenderFlip = RendererFlip,
                    RotOrigin = rotOrigin,
                    Priority = 0,
                    RenderLayer = 0,
                    Color = Color,
                    RenderSpace = RenderSpace.UI
                }
            };
        }


        public void Initialize()
        {
            if (IsInteractable)
                Renderer.Instance.AddProducer(this);
        }
        public void Destroy() => Renderer.Instance.RemoveProducer(this);
        public void Update() { }

        public void Select(bool isSelected)
        {
            if (isSelected != _isSelected)
            {
                if (isSelected)
                    OnSelect();
                else if (!isSelected)
                    OnDeselect();

                _isSelected = isSelected;
            }

            if (_isSelected)
                OnHover();
        }


        public void Pressed(bool isPressed)
        {
            if (isPressed != _isPressed)
            {
                if (isPressed)
                {
                    OnPressed();
                    onPressedEvent?.Invoke(this, EventArgs.Empty);
                }
                else
                {
                    OnRelease();
                    onReleaseEvent?.Invoke(this, EventArgs.Empty);
                }
                _isPressed = isPressed;
            }
            if (_isPressed)
            {
                onHeldEvent?.Invoke(this, EventArgs.Empty);
                OnHeld();
            }
        }

        public virtual void OnSelect() { }
        public virtual void OnHover() { }
        public virtual void OnDeselect() { }

        public virtual void OnPressed() { }
        public virtual void OnHeld() { }
        public virtual void OnRelease() { }
    }
}