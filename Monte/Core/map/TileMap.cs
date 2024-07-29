using System.Numerics;
using System.Globalization;
using System.Xml;
using Monte.Abstractions;
using Monte.Animation;
using Monte.Components;
using static SDL2.SDL;
using Monte.Lib;


namespace Monte.Map
{
    public class TileMap : Entity
    {
        public string MapFile;
        public string MapName;
        public int MapHeight;
        public int MapWidth;
        public int TileWidth;
        public int TileHeight;
        public Vector2 MapOffset = new();

        // Disabling a whole bunch of warnings because of XML again...
#pragma warning disable CS8600
#pragma warning disable CS8602

        public TileMap(string MapName, string MapFile, Vector2 mapOffset = new Vector2())
        {
            this.MapName = MapName;
            MapOffset = mapOffset;
            this.MapFile = MapFile;

            // Parsing map files
            XmlDocument mapDoc = ContentManager.LoadMapFile(MapFile);

            // Fetching tile source from mapfiles image source, the .tsx file associated with the map
            XmlNode mapNode = mapDoc.SelectSingleNode("/map");

            MapWidth = Convert.ToInt32(mapNode.Attributes["width"].InnerText.Trim());
            MapHeight = Convert.ToInt32(mapNode.Attributes["height"].InnerText.Trim());

            XmlNode tilesetNode = mapNode.SelectSingleNode("//tileset");
            string tilesSource = tilesetNode.Attributes.GetNamedItem("source").InnerText.Trim();

            // For readability purposes
            var tileInfo = GetTileSheetInfo(tilesSource);
            // Debug.Log(tileInfo);
            IntPtr tileTex = tileInfo.Item1;

            TileWidth = tileInfo.Item2;
            TileHeight = tileInfo.Item3;

            int imageWidth = tileInfo.Item4;
            int imageHeight = tileInfo.Item5;

            List<Tile> mapTiles = ParseTiles(tilesSource, TileWidth, TileHeight, imageWidth, imageHeight);

            // Debug.Log(mapTiles.Count);

            Dictionary<int, List<int>> MapData = ParseMapData(mapDoc);

            List<Collider> colliders = ParseMapColliders(mapDoc, mapOffset);

            Components.AddRange(colliders);

            foreach (KeyValuePair<int, List<int>> kvp in MapData)
            {
                _ = new TileRenderer(
                    parent: this,
                    renderLayer: kvp.Key,
                    mapFile: this.MapFile,
                    tex: tileTex,
                    tileWidth: TileWidth,
                    tileHeight: TileHeight,
                    mapWidth: MapWidth,
                    mapHeight: MapHeight,
                    tiles: mapTiles,
                    mapData: kvp.Value,
                    mapOffset: MapOffset
                );
            }
        }

        private List<KeyValuePair<string, decimal>> GetColliderData(XmlNode node_col)
        {
            List<KeyValuePair<string, decimal>> pairs = new();
            string xs = node_col.Attributes["x"].InnerText.Trim();
            string ys = node_col.Attributes["y"].InnerText.Trim();

            decimal xd = Convert.ToDecimal(xs, CultureInfo.InvariantCulture);
            decimal yd = Convert.ToDecimal(ys, CultureInfo.InvariantCulture);

            pairs.AddRange(new List<KeyValuePair<string, decimal>>(){
                new ("x", xd),
                new ("y", yd)
            });

            try
            {
                string heights = node_col.Attributes["height"].InnerText.Trim();
                string widths = node_col.Attributes["width"].InnerText.Trim();

                decimal widthd = Convert.ToDecimal(widths, CultureInfo.InvariantCulture);
                decimal heightd = Convert.ToDecimal(heights, CultureInfo.InvariantCulture);

                pairs.AddRange(new List<KeyValuePair<string, decimal>>(){
                    new ("width", widthd),
                    new ("height", heightd)
                });
            }
            catch { }

            return pairs;
        }

        private Collider RectangleColliderParse(XmlNode node_col, Vector2 mapOffset)
        {
            List<KeyValuePair<string, decimal>> colData = GetColliderData(node_col);

            decimal x = colData.FirstOrDefault(item => item.Key == "x").Value;
            decimal y = colData.FirstOrDefault(item => item.Key == "y").Value;
            decimal width = colData.FirstOrDefault(item => item.Key == "width").Value;
            decimal height = colData.FirstOrDefault(item => item.Key == "height").Value;

            bool isTrigger = false;
            if (node_col.Attributes["type"] != null)
                isTrigger = node_col.Attributes["type"].InnerText.Trim().ToLower() == "trigger";

            return new RectangleCollider(
                this, new()
                {
                    x = (int)((float)x + mapOffset.X),
                    y = (int)((float)y + mapOffset.Y),
                    w = (int)width,
                    h = (int)height,
                },
                new(0, 0)
            )
            { IsTrigger = isTrigger, IsStatic = true };
        }

        private Collider CircleColliderParse(XmlNode node_col, Vector2 mapOffset)
        {
            List<KeyValuePair<string, decimal>> colData = GetColliderData(node_col);
            decimal x = colData.FirstOrDefault(item => item.Key == "x").Value;
            decimal y = colData.FirstOrDefault(item => item.Key == "y").Value;
            decimal width = colData.FirstOrDefault(item => item.Key == "width").Value;
            decimal height = colData.FirstOrDefault(item => item.Key == "height").Value;

            bool isTrigger = false;
            if (node_col.Attributes["type"] != null)
                isTrigger = node_col.Attributes["type"].InnerText.Trim().ToLower() == "trigger";

            if (width == height)
            {
                return new CircleCollider(
                    this,
                    new SDL_Point()
                    {
                        x = (int)((float)x + mapOffset.X),
                        y = (int)((float)y + mapOffset.Y),
                    },
                    (float)width
                )
                { IsTrigger = isTrigger, IsStatic = true };
            }
            else
            {
                throw new ArgumentException("Monte does not support ellipse colliders, make sure map colliders are perfect circles in width and height!");
            }
        }
        private Collider ParsePolygonCollider(XmlNode node_col, Vector2 mapOffset)
        {
            List<KeyValuePair<string, decimal>> colData = GetColliderData(node_col);

            // These two values do not represent the actual location of the bounding box
            // Rather they are the initial X and Y location of the first vertex.
            // Why? Fuck if I know. Kinda idiotic to not be consistent but what can you do...
            decimal x = colData.FirstOrDefault(item => item.Key == "x").Value;
            decimal y = colData.FirstOrDefault(item => item.Key == "y").Value;

            bool isTrigger = false;
            if (node_col.Attributes["type"] != null)
                isTrigger = node_col.Attributes["type"].InnerText.Trim().ToLower() == "trigger";

            XmlNode polygon_node = node_col.SelectSingleNode("polygon");
            string pointsString = polygon_node.Attributes["points"].InnerText.Trim();
            List<string> split = pointsString.Split(new char[] { ' ', ',' }).ToList();

            List<SDL_Point> points = new();
            for (int i = 0; i < split.Count; i += 2)
            {
                points.Add(
                    new()
                    {
                        x = (int)Convert.ToDecimal(split[i], CultureInfo.InvariantCulture),
                        y = (int)Convert.ToDecimal(split[i + 1], CultureInfo.InvariantCulture)
                    }
                );
            }

            int topLeftX = points.Select(x => x.x).Min();
            int topLeftY = points.Select(y => y.y).Min();


            List<Edge> edges = new();
            for (int i = 0; i < points.Count; i++)
            {
                edges.Add(new Edge() { a = points[i], b = points[(i + 1) % points.Count] });
            }

            return new PolygonCollider(
                this,
                Polygon.SimpleTriangulateConvex(points),
                edges,
                new Vector2()
                {
                    X = (float)x + mapOffset.X,
                    Y = (float)y + mapOffset.Y
                }
            )
            {
                IsTrigger = isTrigger,
                IsStatic = true
            };
        }

        private List<Collider> ParseMapColliders(XmlDocument mapDoc, Vector2 mapOffset)
        {
            List<Collider> colliders = new();
            XmlNode objectLayer = mapDoc.SelectSingleNode("//objectgroup");

            if (objectLayer != null)
            {
                XmlNodeList node_colliders = objectLayer.SelectNodes("//object");

                foreach (XmlNode node_col in node_colliders)
                {
                    XmlNode? ellipse_node = node_col.SelectSingleNode("ellipse");
                    XmlNode? polygon_node = node_col.SelectSingleNode("polygon");

                    if (ellipse_node != null)
                        colliders.Add(CircleColliderParse(node_col, mapOffset));
                    else if (polygon_node != null)
                        colliders.Add(ParsePolygonCollider(node_col, mapOffset));
                    else
                        colliders.Add(RectangleColliderParse(node_col, mapOffset));
                }
            }
            return colliders;
        }

        private Dictionary<int, List<int>> ParseMapData(XmlDocument mapDoc)
        {
            Dictionary<int, List<int>> mapData = new();
            XmlNodeList layers = mapDoc.SelectNodes("//layer");
            foreach (XmlNode layer in layers)
            {
                XmlNode data = layer.SelectSingleNode("data");
                string mapIdsString = data.InnerText.Trim();
                string[] mapParsed = mapIdsString.Split(",");
                List<int> mapIntParsed = mapParsed.ToList().Select(int.Parse).ToList();

                // By default, layer starts at 1
                mapData.Add(Convert.ToInt32(layer.Attributes["id"].InnerText.Trim()) - 1, mapIntParsed);
            }
            return mapData;
        }

        private (IntPtr, int, int, int, int) GetTileSheetInfo(string source)
        {
            // Loading tileset info
            XmlDocument mapTilesDoc = ContentManager.LoadMapTiles(source);
            XmlNode tileTilesetNode = mapTilesDoc.SelectSingleNode("tileset");
            XmlNode tileImageNode = tileTilesetNode.SelectSingleNode("image");
            int tileWidth = Convert.ToInt32(tileTilesetNode.Attributes.GetNamedItem("tilewidth").InnerText.Trim());
            int tileHeight = Convert.ToInt32(tileTilesetNode.Attributes.GetNamedItem("tileheight").InnerText.Trim());

            // loading tileset sheet and information 
            string imageSource = tileImageNode.Attributes.GetNamedItem("source").InnerText.Trim();
            int imageWidth = Convert.ToInt32(tileImageNode.Attributes.GetNamedItem("width").InnerText.Trim());
            int imageHeight = Convert.ToInt32(tileImageNode.Attributes.GetNamedItem("height").InnerText.Trim());

            IntPtr tileTex = ContentManager.LoadTexture(imageSource);
            return (tileTex, tileWidth, tileHeight, imageWidth, imageHeight);
        }

        // Parses the .tmx xml file for getting the TileMap, not the map itself.
        private List<Tile> ParseTiles(string source, int tileWidth, int tileHeight, int imageWidth, int imageHeight)
        {
            // Return list 
            List<Tile> Tiles = new();
            AnimationClip? ac = null;

            // parsing tiles
            XmlDocument mapTilesDoc = ContentManager.LoadMapTiles(source);
            XmlNode tileTilesetNode = mapTilesDoc.SelectSingleNode("tileset");
            XmlNodeList tileNodes = tileTilesetNode.SelectNodes("tile");

            // The tiles have not been processed in Tiled before importing. So each tile is just "static"
            // and have to be calculated separately. But its still way easier so fine with me. 
            if (tileNodes.Count == 0)
            {
                int rows = imageHeight / tileHeight;
                int columns = imageWidth / tileWidth;
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < columns; x++)
                    {
                        Tile tile = new(
                            x + (y * rows),
                            x * tileWidth,
                            y * tileHeight,
                            "default",
                            null,
                            null
                        );
                        Tiles.Add(tile);
                    }
                }
            }
            else
            {
                int columns = imageWidth / tileWidth;
                foreach (XmlNode tileNode in tileNodes)
                {
                    // id tags and spritesheet coordinates
                    int id = Convert.ToInt32(tileNode.Attributes.GetNamedItem("id").InnerText.Trim());
                    string tag = "default";
                    int sheetX = 0;
                    int sheetY = 0;

                    if (id != 0)
                    {
                        int row = id / columns;
                        int column = id % columns;
                        sheetX = column * tileWidth;
                        sheetY = row * tileHeight;
                    }

                    XmlNode propertiesNode = tileNode.SelectSingleNode("properties");
                    if (propertiesNode != null)
                    {
                        XmlNodeList propertyNodes = propertiesNode.SelectNodes("property");
                        foreach (XmlNode property in propertyNodes)
                        {
                            if (property.Attributes.GetNamedItem("name").InnerText.ToLower() == "tag")
                                tag = property.Attributes["value"].InnerText.Trim().ToLower();
                            break;
                        }
                    }

                    // Animation frames present?
                    AnimationFrame[] frames = Array.Empty<AnimationFrame>();
                    List<int> frameTimes = new();
                    XmlNode animationNode = tileNode.SelectSingleNode("animation");
                    if (animationNode != null)
                    {
                        XmlNodeList frameNodes = animationNode.SelectNodes("frame");
                        // we need to now do the x and y positions correctly based on the tileid of the frame node
                        frames = new AnimationFrame[frameNodes.Count];

                        for (int i = 0; i < frameNodes.Count; i++)
                        {
                            XmlNode frameNode = frameNodes[i];
                            int tileId = Convert.ToInt32(frameNode.Attributes["tileid"].InnerText.Trim());
                            int ms = Convert.ToInt32(frameNode.Attributes["duration"].InnerText.Trim());
                            frameTimes.Add(ms);

                            int fx = 0;
                            int fy = 0;

                            // Might be wrong
                            if (tileId != 0)
                            {
                                int row = tileId / columns;
                                int column = tileId % columns;
                                fx = column * tileWidth;
                                fy = row * tileHeight;
                            }
                            AnimationFrame frame = new(i, fx, fy, tileWidth, tileHeight);
                            frames[i] = frame;
                        }
                    }

                    if (frames.Length > 0)
                        ac = new AnimationClip($"TILE_{id}", frames);

                    Tile mapTile = new(id, sheetX, sheetY, tag, ac, frameTimes);
                    Tiles.Add(mapTile);
                }
            }
            return Tiles;
        }

#pragma warning restore CS8600
#pragma warning restore CS8602

        public Tile? GetMapTile(int x, int y, TileRenderer trend)
        {
            int index = y * MapWidth + x;
            Tile? tile = trend.Tiles.FirstOrDefault(x => x.ID == trend.MapData[index] - 1);
            return tile;
        }

        public void SwitchTile(int x, int y, TileRenderer trend, int tileID)
        {
            trend.MapData[y * MapWidth + x] = tileID;
        }

        public override void OnUpdate() { }

        public override void OnDestroy()
        {
            ContentManager.UnloadMap(MapFile);
        }
    }
}