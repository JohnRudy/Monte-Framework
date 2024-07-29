using System.Numerics;
using Monte.Abstractions;
using Monte.Interfaces;
using Monte.Map;
using Monte.Rendering;
using Monte.Scenes;
using static SDL2.SDL;


namespace Monte.Components
{
    public class TileRenderer : RenderProducer, IComponent
    {
        private Entity _Parent;
        public string MapFile;
        public Entity Parent { get => _Parent; set => _Parent = value; }
        public IntPtr MapSheetTex;
        public int TileWidth;
        public int TileHeight;
        public int MapWidth;
        public int MapHeight;
        public List<Tile> Tiles = new();
        public List<int> MapData = new();
        public Vector2 MapOffset = new();
        public SDL_Color Color = new SDL_Color() { r = 255, g = 255, b = 255, a = 255 };
        public int Priority = 0;
        public int RenderLayer = 0;
        public RenderSpace RenderSpace = RenderSpace.WORLD;

        public TileRenderer(
            Entity parent,
            int renderLayer,
            string mapFile,
            IntPtr tex,
            int tileWidth,
            int tileHeight,
            int mapWidth,
            int mapHeight,
            List<Tile> tiles,
            List<int> mapData,
            Vector2 mapOffset
        )
        {
            _Parent = parent;
            MapFile = mapFile;
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            MapSheetTex = tex;
            MapWidth = mapWidth;
            MapHeight = mapHeight;
            Tiles = tiles;
            MapData = mapData;
            MapOffset = mapOffset;
            Priority = 0;
            RenderLayer = renderLayer;

            parent.Components.Add(this);
        }

        public void Update() => Tiles.ForEach(x => x.Update(Time.GameTimeMS));

        internal override List<RenderObject> Produce(IntPtr SDLRenderer)
        {
            List<RenderObject> result = new();
            for (int i = 0; i < MapHeight; i++)
            {
                for (int j = 0; j < MapWidth; j++)
                {
                    int index = i * MapWidth + j;

                    if (MapData[index] != 0)
                    {
                        int posY = i * TileHeight + (int)MapOffset.Y;
                        int posX = j * TileWidth + (int)MapOffset.X;

                        SDL_Rect dst_rect = SceneManager.CurrentScene.Camera.TransfromDSTRect(new() { x = posX, y = posY, w = TileWidth, h = TileHeight });

                        Tile? renderTile = Tiles.FirstOrDefault(x => x.ID == MapData[index] - 1);

                        if (renderTile != null)
                        {
                            SDL_Rect SRC_Rect = new()
                            {
                                x = renderTile.SheetX,
                                y = renderTile.SheetY,
                                w = TileWidth,
                                h = TileHeight

                            };

                            SDL_Point rotOrigin = new()
                            {
                                x = (int)(dst_rect.w * 0.5f),
                                y = (int)(dst_rect.h * 0.5f)
                            };

                            result.Add(new RenderObject()
                            {
                                Texture = MapSheetTex,
                                DSTRect = dst_rect,
                                SRCRect = SRC_Rect,
                                Rotation = (float)Parent.Transform.Rotation,
                                RenderFlip = SDL_RendererFlip.SDL_FLIP_NONE,
                                RotOrigin = rotOrigin,
                                RenderLayer = RenderLayer,
                                Priority = Priority,
                                Color = Color,
                                RenderSpace = RenderSpace
                            });
                        }
                    }
                }
            }
            return result;
        }

        public void Destroy()
        {
            Renderer.Instance.RemoveProducer(this);
            ContentManager.UnloadMapTiles(MapFile);
        }

        public void Initialize()
        {
            if (Enabled)
                Renderer.Instance.AddProducer(this);
        }
    }
}