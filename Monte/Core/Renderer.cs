using Monte.Scenes;
using Monte.Rendering;
using static SDL2.SDL;
using Monte.Abstractions;


namespace Monte
{
    public abstract class RenderProducer : Enablable
    {
        internal abstract List<RenderObject> Produce(IntPtr SDLRenderer);
    }

    public class RenderObject
    {
        public SDL_Rect DSTRect;
        public SDL_Rect SRCRect;
        public int Priority = 0;
        public IntPtr Texture = IntPtr.Zero;
        public SDL_Color Color = new() { r = 255, g = 255, b = 255, a = 255 };
        public float Rotation = 0;
        public SDL_Point RotOrigin = new SDL_Point() { x = 0, y = 0 };
        public RenderSpace RenderSpace = RenderSpace.WORLD;
        public SDL_RendererFlip RenderFlip = SDL_RendererFlip.SDL_FLIP_NONE;
        public int RenderLayer = 0;

        internal void Render(IntPtr sdlRenderer)
        {
            SDL_Rect _dst = DSTRect;
            SDL_Rect _src = SRCRect;

            if (SDL_SetTextureColorMod(Texture, Color.r, Color.g, Color.b) != 0)
                throw new Exception($"{this} COLOR: could not render: {SDL_GetError()}");

            if (SDL_SetTextureAlphaMod(Texture, Color.a) != 0)
                throw new Exception($"{this} ALPHA: could not render: {SDL_GetError()}");

            if (SDL_RenderCopyEx(sdlRenderer, Texture, ref _src, ref _dst, Rotation, ref RotOrigin, RenderFlip) != 0)
                throw new Exception($"{this} TEXTURE: could not render: {SDL_GetError()}");
        }
    }

    public enum RenderOrder
    {
        LEFTDOWN,
        DOWN,
        USER
    }

    public class Renderer
    {
        // A poormans version of a "singleton". 
        // It's supposed to be a static reference to the current Renderer object to easily assing and handle render buffers.
        public static Renderer Instance
        {
            get
            {
                if (_instance is null) throw new InvalidOperationException("Renderer has not been assigned!");
                return _instance;
            }
        }
        private static Renderer? _instance;

        readonly Window _Window;
        readonly IntPtr _SDLRenderer;
        readonly List<RenderObject> _RenderObjects = new();
        readonly List<RenderProducer> _Producers = new();
        public IntPtr SDL_Renderer { get => _SDLRenderer; }

        private Dictionary<RenderOrder, Func<List<RenderObject>, List<RenderObject>>> RenderOrders = new();
        internal static void AssignRenderer(Renderer renderer) => _instance = renderer;

        public Renderer(Window window)
        {
            _Window = window;


            _SDLRenderer = SDL_CreateRenderer(
                _Window.SDLWindow,
                RendererSettings.RendererIndex,
                RendererSettings.RenderFlags
            );

            if (_SDLRenderer == IntPtr.Zero)
                Debug.Log($"Could not initialize SDL Renderer: {SDL_GetError()}");

            // This is not the actual logical size, it's currently placeholder
            if (SDL_RenderSetLogicalSize(_SDLRenderer, RendererSettings.VirtualWidth, RendererSettings.VirtualHeight) != 0)
                Debug.Log($"Could not set logical size: {SDL_GetError()}");

            if (SDL_SetRenderDrawBlendMode(_SDLRenderer, RendererSettings.BlendMode) != 0)
                Debug.Log($"Could not set Blend Mode: {SDL_GetError()}");

            RenderOrders = new Dictionary<RenderOrder, Func<List<RenderObject>, List<RenderObject>>>
            {
                {RenderOrder.LEFTDOWN, LeftDown},
                {RenderOrder.DOWN, Down}
            };
        }

        private List<RenderObject> LeftDown(List<RenderObject> renderObjects)
        {
            return renderObjects.OrderBy(ro => ro.RenderLayer)
                .ThenByDescending(ro => ro.DSTRect.x)
                .ThenBy(ro => ro.DSTRect.y)
                .ThenByDescending(ro => ro.Priority)
                .ToList();
        }

        private List<RenderObject> Down(List<RenderObject> renderObjects)
        {
            return renderObjects.OrderBy(ro => ro.RenderLayer)
                .ThenBy(ro => ro.DSTRect.y)
                .ThenByDescending(ro => ro.Priority)
                .ToList();
        }


        private List<RenderObject> DoRenderOrdering(List<RenderObject> renderObjects)
        {
            if (!RenderOrders.TryGetValue(RendererSettings.RenderOrder, out var order))
                throw new InvalidOperationException("No ordering set for given RenderOrder");

            return order(renderObjects);
        }

        public void AddProducer(RenderProducer producer)
        {
            if (_Producers.Contains(producer)) return;

            _Producers.Add(producer);
        }

        public void RemoveProducer(RenderProducer producer)
        {
            if (_Producers.Contains(producer)) _Producers.Remove(producer);
        }

        public void AddToRenderBuffer(RenderObject renderObject)
        {
            if (_RenderObjects.Contains(renderObject)) return;

            _RenderObjects.Add(renderObject);
        }


        internal void Render()
        {
            OnRender();

            if (SDL_SetRenderDrawColor(
                _SDLRenderer,
                SceneManager.CurrentScene.BackgroundColor.r,
                SceneManager.CurrentScene.BackgroundColor.g,
                SceneManager.CurrentScene.BackgroundColor.b,
                SceneManager.CurrentScene.BackgroundColor.a
            ) != 0)
                Debug.Log($"Could not set renderer background draw color: {SDL_GetError()}");

            if (SDL_RenderClear(_SDLRenderer) != 0)
                Debug.Log($"Could not clear renderer: {SDL_GetError()}");

            if (_Producers.Any())
            {
                _Producers.ForEach(renderer => _RenderObjects.AddRange(renderer.Produce(SDL_Renderer)));

                List<RenderObject> world = _RenderObjects.Where(x => x.RenderSpace == RenderSpace.WORLD).ToList();
                List<RenderObject> screen = _RenderObjects.Where(x => x.RenderSpace == RenderSpace.SCREEN).ToList();
                List<RenderObject> ui = _RenderObjects.Where(x => x.RenderSpace == RenderSpace.UI).ToList();

                foreach (RenderObject render in DoRenderOrdering(world))
                {
                    if (
                        render.DSTRect.x < -RendererSettings.Margin ||
                        render.DSTRect.y < -RendererSettings.Margin ||
                        render.DSTRect.x > WindowSettings.WindowWidth + RendererSettings.Margin ||
                        render.DSTRect.y > WindowSettings.WindowHeight + RendererSettings.Margin
                    )
                        continue;
                    else
                        render.Render(SDL_Renderer);
                }

                foreach (RenderObject render in DoRenderOrdering(screen))
                {
                    if (
                        render.DSTRect.x < -RendererSettings.Margin ||
                        render.DSTRect.y < -RendererSettings.Margin ||
                        render.DSTRect.x > WindowSettings.WindowWidth + RendererSettings.Margin ||
                        render.DSTRect.y > WindowSettings.WindowHeight + RendererSettings.Margin
                    )
                        continue;
                    else
                        render.Render(SDL_Renderer);
                }

                foreach (RenderObject render in DoRenderOrdering(ui))
                {
                    if (
                        render.DSTRect.x < -RendererSettings.Margin ||
                        render.DSTRect.y < -RendererSettings.Margin ||
                        render.DSTRect.x > WindowSettings.WindowWidth + RendererSettings.Margin ||
                        render.DSTRect.y > WindowSettings.WindowHeight + RendererSettings.Margin
                    )
                        continue;
                    else
                        render.Render(SDL_Renderer);
                }
                _RenderObjects.Clear();
            }

            Debug.RenderUpdate(SDL_Renderer);

            SDL_RenderPresent(_SDLRenderer);
        }

        public virtual void OnRender() { }

        // Renders a circle based of it's bounding boxes top left corner
        public static void RenderCircle(IntPtr sdlRenderer, SDL_Point center, float radius, SDL_Color color)
        {
            for (int x = 0; x <= radius * Math.Sqrt(2); x++)
            {
                int y = (int)Math.Sqrt(radius * radius - x * x);

                DrawPoint(sdlRenderer, center.x + x, center.y + y, color);
                DrawPoint(sdlRenderer, center.x + x, center.y - y, color);
                DrawPoint(sdlRenderer, center.x - x, center.y + y, color);
                DrawPoint(sdlRenderer, center.x - x, center.y - y, color);
                DrawPoint(sdlRenderer, center.x + y, center.y + x, color);
                DrawPoint(sdlRenderer, center.x + y, center.y - x, color);
                DrawPoint(sdlRenderer, center.x - y, center.y + x, color);
                DrawPoint(sdlRenderer, center.x - y, center.y - x, color);
            }
        }

        public static void DrawPoint(IntPtr sdlRenderer, int x, int y, SDL_Color color)
        {
            if (SDL_SetRenderDrawColor(sdlRenderer, color.r, color.g, color.b, color.a) != 0)
                Debug.Log($"Could not set render color: {SDL_GetError()}");

            if (SDL_RenderDrawPoint(sdlRenderer, x, y) != 0)
                Debug.Log($"Unable to draw point: {SDL_GetError()}");
        }
    }
}