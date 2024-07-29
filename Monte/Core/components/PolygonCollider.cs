using Monte.Abstractions;
using Monte.Physics;
using static SDL2.SDL;
using Monte.Lib;
using System.Numerics;


namespace Monte.Components
{
    public class PolygonCollider : Collider
    {
        private Entity _parent;
        public override Entity Parent { get => _parent; set => _parent = value; }

        private int width = 0;
        private int height = 0;

        public Vector2 Origin { get; set; }
        
        // Use this with caution. 
        // The center is not actually the center point of the collider
        // it fucks off to god knows where because of how the vertices are handled in XML
        // But it still has to be like this so that the other locations stay correct 
        // between user made polygons and map tile collider polygons
        internal SDL_Point _fauxCenter
        {
            get
            {
                return new SDL_Point()
                {
                    x = (int)(Parent.Transform.Position.X + Origin.X + (width / 2)),
                    y = (int)(Parent.Transform.Position.Y + Origin.Y + (height / 2)),
                };
            }
        }
        public override SDL_Point WorldCenter
        {
            get
            {
                return new SDL_Point()
                {
                    x = (int)(WorldBoundingBox.x + (WorldBoundingBox.w / 2)),
                    y = (int)(WorldBoundingBox.y + (WorldBoundingBox.h / 2)),
                };
            }
        }

        // Use this for a more reliable center point in world
        // Actual center point of the shape
        public SDL_Point ShapeMeanCenter
        {
            get
            {
                List<SDL_Point> centers = _Polygons.Select(p => p.PolygonCenter()).ToList();
                int x = centers.Select(c => c.x).Sum() / centers.Count;
                int y = centers.Select(c => c.y).Sum() / centers.Count;

                SDL_Point center = new SDL_Point()
                {
                    x = x + _fauxCenter.x - width / 2,
                    y = y + _fauxCenter.y - height / 2,
                };
                return center;
            }
        }


        public override SDL_Rect WorldBoundingBox
        {
            get
            {
                float minX = _Polygons.SelectMany(p => p.Vertices).Select(v => v.x).Min();
                float minY = _Polygons.SelectMany(p => p.Vertices).Select(v => v.y).Min();

                return new SDL_Rect()
                {
                    x = (int)(_fauxCenter.x - (width / 2) + minX),
                    y = (int)(_fauxCenter.y - (height / 2) + minY),
                    w = width,
                    h = height
                };
            }
        }
        public List<Polygon> _Polygons;
        public List<Polygon> Polygons
        {
            get
            {
                return _Polygons.Select(
                    p => new Polygon()
                    {
                        Vertices = p.Vertices.Select(
                            v => new SDL_Point()
                            {
                                x = (int)(v.x + _fauxCenter.x - (width / 2)),
                                y = (int)(v.y + _fauxCenter.y - (height / 2)),
                            }
                        ).ToList()
                    }
                ).ToList();
            }
            private set => _Polygons = value;
        }

        private List<Edge> _Edges;
        public List<Edge> Edges
        {
            get
            {
                return _Edges.Select(
                    e => new Edge()
                    {
                        a = new SDL_Point()
                        {
                            x = e.a.x + _fauxCenter.x - (width / 2),
                            y = e.a.y + _fauxCenter.y - (height / 2),
                        },
                        b = new SDL_Point()
                        {
                            x = e.b.x + _fauxCenter.x - (width / 2),
                            y = e.b.y + _fauxCenter.y - (height / 2)
                        }
                    }
                ).ToList();
            }
            private set => _Edges = value;
        }

        public PolygonCollider(Entity parent, List<Polygon> polygons, List<Edge> edges, Vector2 origin) : base(parent)
        {
            _parent = parent;
            Shape = PhysicsShape.Polygon;
            _Polygons = polygons;
            _Edges = edges;
            Origin = origin;

            List<int> xPositions = Polygons.SelectMany(polygons => polygons.Vertices).Select(vertice => vertice.x).ToList();
            List<int> yPositions = Polygons.SelectMany(polygons => polygons.Vertices).Select(vertice => vertice.y).ToList();

            // top left corner
            width = Math.Abs(xPositions.Max() - xPositions.Min());
            height = Math.Abs(yPositions.Max() - yPositions.Min());

            _parent.Components.Add(this);
        }
    }
}
