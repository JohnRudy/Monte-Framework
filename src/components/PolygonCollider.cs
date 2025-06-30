using Monte.Abstractions;
using Monte.Core;

using static SDL.SDL_rect;

using System.Numerics;


namespace Monte.Components
{
    public class PolygonCollider : Collider
    {
        private MonteBehaviour? _parent;
        public override MonteBehaviour? Parent { get => _parent; set => _parent = value; }

        /// <summary>
        /// The width of the bounding box of this collider
        /// </summary>
        private float width = 0;
        
        /// <summary>
        /// The height of the bounding box of this collider.
        /// </summary>
        private float height = 0;

        /// <summary>
        /// Origin point of the collider
        /// </summary>
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
                if (Parent == null) throw new Exception("Parent is null");
                return new SDL_Point()
                {
                    x = (int)(Parent.Transform.Position.X + Origin.X + (width / 2)),
                    y = (int)(Parent.Transform.Position.Y + Origin.Y + (height / 2)),
                };
            }
        }

        public override SDL_FPoint WorldCenter
        {
            get
            {
                return new SDL_FPoint()
                {
                    x = WorldBoundingBox.x + (WorldBoundingBox.w / 2),
                    y = WorldBoundingBox.y + (WorldBoundingBox.h / 2),
                };
            }
        }

        /// <summary>
        /// More reliable center location in world than worldCenter.
        /// </summary>
        public SDL_FPoint ShapeMeanCenter
        {
            get
            {
                List<SDL_FPoint> centers = _Polygons.Select(p => p.PolygonCenter()).ToList();
                float x = centers.Select(c => c.x).Sum() / centers.Count;
                float y = centers.Select(c => c.y).Sum() / centers.Count;

                SDL_FPoint center = new SDL_FPoint()
                {
                    x = x + _fauxCenter.x - width / 2,
                    y = y + _fauxCenter.y - height / 2,
                };
                return center;
            }
        }


        public override SDL_FRect WorldBoundingBox
        {
            get
            {
                float minX = _Polygons.SelectMany(p => p.Vertices).Select(v => v.x).Min();
                float minY = _Polygons.SelectMany(p => p.Vertices).Select(v => v.y).Min();

                return new SDL_FRect()
                {
                    x = _fauxCenter.x - (width / 2) + minX,
                    y = _fauxCenter.y - (height / 2) + minY,
                    w = width,
                    h = height
                };
            }
        }
        private List<Polygon> _Polygons;

        /// <summary>
        /// List of polygon objects in this collier
        /// </summary>
        public List<Polygon> Polygons
        {
            get
            {
                return _Polygons.Select(
                    p => new Polygon()
                    {
                        Vertices = p.Vertices.Select(
                            v => new SDL_FPoint()
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

        /// <summary>
        /// All edge data of this polygon collider.
        /// </summary>
        public List<Edge> Edges
        {
            get
            {
                return _Edges.Select(
                    e => new Edge()
                    {
                        a = new SDL_FPoint()
                        {
                            x = e.a.x + _fauxCenter.x - (width / 2),
                            y = e.a.y + _fauxCenter.y - (height / 2),
                        },
                        b = new SDL_FPoint()
                        {
                            x = e.b.x + _fauxCenter.x - (width / 2),
                            y = e.b.y + _fauxCenter.y - (height / 2)
                        }
                    }
                ).ToList();
            }
            private set => _Edges = value;
        }

        public PolygonCollider(List<Polygon> polygons, List<Edge> edges, Vector2 origin)
        {
            Shape = PhysicsShape.Polygon;
            _Polygons = polygons;
            _Edges = edges;
            Origin = origin;

            List<float> xPositions = Polygons.SelectMany(polygons => polygons.Vertices).Select(vertice => vertice.x).ToList();
            List<float> yPositions = Polygons.SelectMany(polygons => polygons.Vertices).Select(vertice => vertice.y).ToList();

            // top left corner
            width = Math.Abs(xPositions.Max() - xPositions.Min());
            height = Math.Abs(yPositions.Max() - yPositions.Min());
        }
    }
}
