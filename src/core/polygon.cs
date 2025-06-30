using System.Numerics;

using static SDL.SDL_rect;


namespace Monte.Core
{
    /// <summary>
    /// Class that is used in Polygon Checks that checks wheter two axises overlap. 
    /// </summary>
    public class AxisInterval
    {
        public double Min { get; }
        public double Max { get; }

        public AxisInterval(double min, double max)
        {
            Min = min;
            Max = max;
        }

        /// <summary>
        /// Checks the overlapping axises of polygons. SAT.
        /// </summary>
        /// <param name="other">The other axis</param>
        /// <returns>bool if overlap does occur</returns>
        public bool Overlaps(AxisInterval other)
        {
            return !(Max < other.Min || other.Max < Min);
        }
    }

    /// <summary>
    /// Helper struct to contain edge information
    /// </summary>
    public struct Edge
    {
        public SDL_FPoint a;
        public SDL_FPoint b;

        /// <summary>
        /// Get perpendicular axis of the given edge object.
        /// </summary>
        /// <param name="edge">The edge object to get axis from</param>
        /// <returns>Vector2 axis</returns>
        public static Vector2 GetPerpendicularAxis(Edge edge)
        {
            Vector2 axis = new(edge.b.y - edge.a.y, -(edge.b.x - edge.a.x));
            return Vector2.Normalize(axis); // Ensure the axis is normalized
        }

        public static List<Edge> EdgesFromPoints(List<SDL_FPoint> points)
        {
            List<Edge> edges = new List<Edge>();
            for (int i = 0; i < points.Count; i++)
            {
                edges.Add(new Edge() { a = points[i], b = points[(i + 1) % points.Count] });
            }
            return edges;
        }
    }

    /// <summary>
    /// Helper struct that contains polygon data.
    /// </summary>
    public struct Polygon
    {
        public List<SDL_FPoint> Vertices;

        public Polygon(List<SDL_FPoint> Vertices)
        {
            if (Vertices.Count < 3)
                throw new ArgumentException("Must have more than or equal to 3 vertices for a polygon");

            this.Vertices = Vertices;
        }

        /// <summary>
        /// Helper method to quickly make a convex polygon triangulation
        /// </summary>
        /// <param name="Vertices">List of vertices SDL_Point of the convex polygon</param>
        /// <returns>A list of triangulated polygons of the convex shape</returns>
        /// <exception cref="ArgumentException">If not enough vertices precent</exception>
        public static List<Polygon> SimpleTriangulateConvex(List<SDL_FPoint> Vertices)
        {
            if (Vertices.Count < 3)
                throw new ArgumentException("Too little vertices to create a polygon");

            if (Vertices.Count == 3)
                return new List<Polygon>() { new(Vertices) };

            List<Polygon> polygons = new();
            for (int i = 1; i < Vertices.Count - 1; i++)
            {
                polygons.Add(
                    new Polygon(
                        new List<SDL_FPoint>(){
                            Vertices[0],
                            Vertices[i],
                            Vertices[i + 1],
                        }
                    )
                );
            }
            return polygons;
        }

        public SDL_FPoint PolygonCenter()
        {
            HashSet<SDL_FPoint> uniquePoints = new();
            Vertices.ForEach(x => uniquePoints.Add(x));

            float cX = uniquePoints.Select(p => p.x).Sum() / uniquePoints.Count;
            float cY = uniquePoints.Select(p => p.y).Sum() / uniquePoints.Count;

            return new() { x = (int)Math.Round(cX), y = (int)Math.Round(cY) };
        }

        public Polygon Move(SDL_Point by)
        {
            List<SDL_FPoint> newVerts = new();
            foreach (var p in Vertices)
            {
                newVerts.Add(new SDL_FPoint() { x = p.x + by.x, y = p.y + by.y });
            }
            return new Polygon(newVerts);
        }

        /// <summary>
        /// Checks wheter a given point is within a convex polygon shape.
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <param name="polygons">Polygon shape to check</param>
        /// <returns>bool</returns>
        public static bool PointInsidePolygon(SDL_Point point, List<Polygon> polygons)
        {
            foreach (var polygon in polygons)
            {
                bool inside = false;
                int vertexCount = polygon.Vertices.Count;

                // Ray casting algorithm to check if the point is inside the polygon
                for (int i = 0, j = vertexCount - 1; i < vertexCount; j = i++)
                {
                    if (((polygon.Vertices[i].y > point.y) != (polygon.Vertices[j].y > point.y)) &&
                        (point.x < (polygon.Vertices[j].x - polygon.Vertices[i].x) * (point.y - polygon.Vertices[i].y) /
                        (polygon.Vertices[j].y - polygon.Vertices[i].y) + polygon.Vertices[i].x))
                    {
                        inside = !inside;
                    }
                }

                if (inside)
                    return true;
            }
            return false;
        }

        /// <summary>
        /// SAT projection helper. Projects polygon edges to axis and returns a AxisInterval object.
        /// </summary>
        /// <param name="polygons">Polygon shape</param>
        /// <param name="axis">axis to project onto</param>
        /// <returns>AxisInterval object</returns>
        public static AxisInterval ProjectOntoAxis(List<Polygon> polygons, Vector2 axis)
        {
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            foreach (var vertex in polygons.SelectMany(p => p.Vertices))
            {
                double projection = Vector2.Dot(axis, new Vector2(vertex.x, vertex.y));

                if (projection < min)
                    min = projection;
                if (projection > max)
                    max = projection;
            }

            return new AxisInterval(min, max);
        }
    }
}