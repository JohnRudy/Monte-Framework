using System.Numerics;
using Monte.Abstractions;
using Monte.Components;
using Monte.Scenes;
using static SDL2.SDL;
using SDL;
using Monte.Lib;


namespace Monte.Physics
{
    public enum PhysicsShape
    {
        Circle,
        Rectangle,
        Polygon,
    }

    public enum Force
    {
        Force, // Add a continuous force to the physicsObject, using its mass.
        Acceleration, // Add a continuous acceleration to the physicsObject, ignoring its mass.
        Impulse, //	Add an instant force impulse to the physicsObject, using its mass.
        VelocityChange, // Add an instant velocity change to the physicsObject, ignoring its mass.
    }

    public static class Physics2D
    {

        // '########::'########::'######:::'#######::'##:::::::'##::::'##:'########:'########::
        //  ##.... ##: ##.....::'##... ##:'##.... ##: ##::::::: ##:::: ##: ##.....:: ##.... ##:
        //  ##:::: ##: ##::::::: ##:::..:: ##:::: ##: ##::::::: ##:::: ##: ##::::::: ##:::: ##:
        //  ########:: ######:::. ######:: ##:::: ##: ##::::::: ##:::: ##: ######::: ########::
        //  ##.. ##::: ##...:::::..... ##: ##:::: ##: ##:::::::. ##:: ##:: ##...:::: ##.. ##:::
        //  ##::. ##:: ##:::::::'##::: ##: ##:::: ##: ##::::::::. ## ##::: ##::::::: ##::. ##::
        //  ##:::. ##: ########:. ######::. #######:: ########:::. ###:::: ########: ##:::. ##:
        // ..:::::..::........:::......::::.......:::........:::::...:::::........::..:::::..::

        private delegate void CollisionResolver(Collider a, Collider b, out Vector2 move);
        private static readonly Dictionary<(PhysicsShape, PhysicsShape), CollisionResolver> CollisionResolveMap = new()
        {
            {(PhysicsShape.Rectangle, PhysicsShape.Rectangle), RectangleOnRectangleResolve},
            {(PhysicsShape.Rectangle, PhysicsShape.Circle), RectangleOnCircleResolve},
            {(PhysicsShape.Rectangle, PhysicsShape.Polygon), RectangleOnPolygonResolve},

            {(PhysicsShape.Circle, PhysicsShape.Rectangle), RectangleOnCircleResolve},
            {(PhysicsShape.Circle, PhysicsShape.Circle), CircleOnCircleResolve},
            {(PhysicsShape.Circle, PhysicsShape.Polygon), CircleOnPolygonResolve},

            {(PhysicsShape.Polygon, PhysicsShape.Rectangle), RectangleOnPolygonResolve},
            {(PhysicsShape.Polygon, PhysicsShape.Circle), CircleOnPolygonResolve},
            {(PhysicsShape.Polygon, PhysicsShape.Polygon), PolygonOnPolygonResolve},
        };

        private static readonly Dictionary<(PhysicsShape, PhysicsShape), Func<Collider, Collider, bool>> CollisionCheckMap = new()
        {
            {(PhysicsShape.Rectangle, PhysicsShape.Rectangle), RectangleOnRectangleCheck},
            {(PhysicsShape.Rectangle, PhysicsShape.Circle), RectangleOnCircleCheck},
            {(PhysicsShape.Rectangle, PhysicsShape.Polygon), RectangleOnPolygonCheck},

            {(PhysicsShape.Circle, PhysicsShape.Rectangle), RectangleOnCircleCheck},
            {(PhysicsShape.Circle, PhysicsShape.Circle), CircleOnCircleCheck},
            {(PhysicsShape.Circle, PhysicsShape.Polygon), CircleOnPolygonCheck},

            {(PhysicsShape.Polygon, PhysicsShape.Rectangle), RectangleOnPolygonCheck},
            {(PhysicsShape.Polygon, PhysicsShape.Polygon), PolygonOnPolygonCheck},
            {(PhysicsShape.Polygon, PhysicsShape.Circle), CircleOnPolygonCheck},
        };

        internal static bool IsColliding(Collider a, Collider b)
        {
            if (!CollisionCheckMap.TryGetValue((a.Shape, b.Shape), out var collisionHandler))
                throw new InvalidOperationException("Invalid shape combination.");

            SDL_Rect ab = a.WorldBoundingBox;
            SDL_Rect bb = b.WorldBoundingBox;

            if (SDL_HasIntersection(ref ab, ref bb) == SDL_bool.SDL_TRUE)
            {
                return collisionHandler(a, b);
            }
            return false;
        }

        internal static void ResolveCollision(Collider a, Collider b, out Vector2 move)
        {
            if (!CollisionResolveMap.TryGetValue((a.Shape, b.Shape), out var collisionHandler))
                throw new InvalidOperationException("Invalid shape combination.");

            collisionHandler(a, b, out move);
        }


        // '##::::'##:'########::'########:::::'###::::'########:'########::'######::
        //  ##:::: ##: ##.... ##: ##.... ##:::'## ##:::... ##..:: ##.....::'##... ##:
        //  ##:::: ##: ##:::: ##: ##:::: ##::'##:. ##::::: ##:::: ##::::::: ##:::..::
        //  ##:::: ##: ########:: ##:::: ##:'##:::. ##:::: ##:::: ######:::. ######::
        //  ##:::: ##: ##.....::: ##:::: ##: #########:::: ##:::: ##...:::::..... ##:
        //  ##:::: ##: ##:::::::: ##:::: ##: ##.... ##:::: ##:::: ##:::::::'##::: ##:
        // . #######:: ##:::::::: ########:: ##:::: ##:::: ##:::: ########:. ######::
        // :.......:::..:::::::::........:::..:::::..:::::..:::::........:::......:::

        // Main simple collision solver for none physics based actions.
        internal static void DoCollisionUpdate()
        {
            List<Collider> cols = new();
            SceneManager.CurrentScene.entities.ForEach(x => cols.AddRange(x.GetComponentsOfType<Collider>().Where(x => x.IsEnabled).ToList()));

            if (cols.Count > 0)
            {
                for (int a = 0; a < cols.Count; a++)
                {
                    for (int b = a + 1; b < cols.Count; b++)
                    {
                        Collider cola = cols[a];
                        Collider colb = cols[b];

                        if (cola.Parent == colb.Parent || cola.IsStatic && colb.IsStatic) continue;

                        if (IsColliding(cola, colb))
                        {
                            cola.AddCollision(colb);
                            colb.AddCollision(cola);
                        }
                        else
                        {
                            cola.RemoveCollision(colb);
                            colb.RemoveCollision(cola);
                        }
                    }
                }
            }
        }


        // Main Physics based collision resolver and physics update
        // Acts with ticks. 
        // Physics get calculated based on set delta time and independantly of frame update times.

        private static float delta = 0;
        internal static void DoPhysicsUpdate()
        {
            delta += Time.DeltaTime;

            if (delta / PhysicsSettings.FixedDeltaTime < 1) return;

            delta %= PhysicsSettings.FixedDeltaTime;

            List<PhysicsObject> physObjects = new();
            SceneManager.CurrentScene.entities.ForEach(x => physObjects.AddRange(x.GetComponentsOfType<PhysicsObject>().Where(x => x.IsEnabled).ToList()));

            List<Collider> cols = new();
            SceneManager.CurrentScene.entities.ForEach(x => cols.AddRange(x.GetComponentsOfType<Collider>().Where(x => x.IsEnabled).ToList()));

            if (physObjects.Count > 0)
            {
                for (int a = 0; a < physObjects.Count; a++)
                {
                    for (int b = a + 1; b < physObjects.Count; b++)
                    {

                    }
                }
            }
        }


        /*
            ..%%%%....%%%%...%%......%%......%%%%%%...%%%%...%%%%%%...%%%%...%%..%%.
            .%%..%%..%%..%%..%%......%%........%%....%%........%%....%%..%%..%%%.%%.
            .%%......%%..%%..%%......%%........%%.....%%%%.....%%....%%..%%..%%.%%%.
            .%%..%%..%%..%%..%%......%%........%%........%%....%%....%%..%%..%%..%%.
            ..%%%%....%%%%...%%%%%%..%%%%%%..%%%%%%...%%%%...%%%%%%...%%%%...%%..%%.
            ........................................................................
            ..%%%%...%%..%%..%%%%%%...%%%%...%%..%%...%%%%..
            .%%..%%..%%..%%..%%......%%..%%..%%.%%...%%.....
            .%%......%%%%%%..%%%%....%%......%%%%.....%%%%..
            .%%..%%..%%..%%..%%......%%..%%..%%.%%.......%%.
            ..%%%%...%%..%%..%%%%%%...%%%%...%%..%%...%%%%..
            ................................................

            - Check if the bounding boxes are clipping.  
            - Check if any point is within the shape
        */

        internal static bool RectangleOnRectangleCheck(Collider a, Collider b)
        {
            // Bounding box check is enough for rectangles
            return true;
        }


        internal static bool RectangleOnCircleCheck(Collider a, Collider b)
        {
            RectangleCollider rectangle = (a is CircleCollider) ? (RectangleCollider)b : (RectangleCollider)a;
            CircleCollider circle = (a is CircleCollider) ? (CircleCollider)a : (CircleCollider)b;

            Vector2 closestDirectionToRectangle = ShapeHelper.ClosestDirectionFromCircleToRectangle(circle.WorldCenter, circle.Radius, rectangle.WorldBoundingBox);
            float distanceSquared = ShapeHelper.DistanceSquaredFromCircleToRectangle(closestDirectionToRectangle);

            if (ShapeHelper.IsCircleWithinSquare(circle.Radius, distanceSquared))
            {
                return true;
            }
            return false;
        }


        internal static bool CircleOnCircleCheck(Collider a, Collider b)
        {
            CircleCollider ea = (CircleCollider)a;
            CircleCollider eb = (CircleCollider)b;

            float dist = (ea.WorldCenter.ToVector2() - eb.WorldCenter.ToVector2()).Length();
            float radius = ea.Radius + eb.Radius;
            return dist <= radius;
        }


        internal static bool RectangleOnPolygonCheck(Collider a, Collider b)
        {
            RectangleCollider rectangle = (a is PolygonCollider) ? (RectangleCollider)b : (RectangleCollider)a;
            PolygonCollider polygon = (a is RectangleCollider) ? (PolygonCollider)b : (PolygonCollider)a;

            SDL_Rect rb = rectangle.WorldBoundingBox;
            SDL_Rect pb = polygon.WorldBoundingBox;

            List<bool> possibleCollisions = new List<bool>();
            foreach (Polygon p in polygon.Polygons)
            {
                SDL_Point ap = p.Vertices[0];
                SDL_Point bp = p.Vertices[1];
                SDL_Point cp = p.Vertices[2];

                List<bool> collisions = new()
                    {
                        SDL_IntersectRectAndLine(ref rb, ref ap.x, ref ap.y, ref bp.x, ref bp.y) == SDL_bool.SDL_TRUE,
                        SDL_IntersectRectAndLine(ref rb, ref bp.x, ref bp.y, ref cp.x, ref cp.y) == SDL_bool.SDL_TRUE,
                        SDL_IntersectRectAndLine(ref rb, ref cp.x, ref cp.y, ref ap.x, ref ap.y) == SDL_bool.SDL_TRUE,
                    };
                possibleCollisions.AddRange(collisions);
            }
            return possibleCollisions.Any(x => x);
        }


        internal static bool CircleOnPolygonCheck(Collider a, Collider b)
        {
            CircleCollider circle = (a is PolygonCollider) ? (CircleCollider)b : (CircleCollider)a;
            PolygonCollider polygon = (a is CircleCollider) ? (PolygonCollider)b : (PolygonCollider)a;

            Vector2 dir = polygon.WorldCenter.ToVector2() - circle.WorldCenter.ToVector2();
            Vector2 normalized = Vector2.Normalize(dir);
            SDL_Point point = (normalized * circle.Radius).ToSDLPoint();
            point = point.Addition(circle.WorldCenter);
            bool isColliding = Polygon.PointInsidePolygon(point, polygon.Polygons);
            return isColliding;
        }


        internal static bool PolygonOnPolygonCheck(Collider a, Collider b)
        {
            PolygonCollider polygonA = (PolygonCollider)a;
            PolygonCollider polygonB = (PolygonCollider)b;

            SDL_Rect par = polygonA.WorldBoundingBox;
            SDL_Rect pbr = polygonB.WorldBoundingBox;

            if (SDL_HasIntersection(ref par, ref pbr) == SDL_bool.SDL_TRUE)
            {
                // Check edges of polygon A
                foreach (Edge edge in polygonA.Edges)
                {
                    var axis = Edge.GetPerpendicularAxis(edge);

                    var projectionA = Polygon.ProjectOntoAxis(polygonA.Polygons, axis);
                    var projectionB = Polygon.ProjectOntoAxis(polygonB.Polygons, axis);

                    if (!projectionA.Overlaps(projectionB))
                    {
                        return false; // Separating axis found, no collision
                    }
                }

                // Check edges of polygon B
                foreach (Edge edge in polygonB.Edges)
                {
                    var axis = Edge.GetPerpendicularAxis(edge);

                    var projectionA = Polygon.ProjectOntoAxis(polygonA.Polygons, axis);
                    var projectionB = Polygon.ProjectOntoAxis(polygonB.Polygons, axis);

                    if (!projectionA.Overlaps(projectionB))
                    {
                        return false; // Separating axis found, no collision
                    }
                }
                return true; // No separating axis found, collision
            }
            return false;
        }

        /*
            ..%%%%....%%%%...%%......%%......%%%%%%...%%%%...%%%%%%...%%%%...%%..%%.
            .%%..%%..%%..%%..%%......%%........%%....%%........%%....%%..%%..%%%.%%.
            .%%......%%..%%..%%......%%........%%.....%%%%.....%%....%%..%%..%%.%%%.
            .%%..%%..%%..%%..%%......%%........%%........%%....%%....%%..%%..%%..%%.
            ..%%%%....%%%%...%%%%%%..%%%%%%..%%%%%%...%%%%...%%%%%%...%%%%...%%..%%.
            ........................................................................
            .%%%%%...%%%%%%...%%%%....%%%%...%%......%%..%%..%%%%%%.
            .%%..%%..%%......%%......%%..%%..%%......%%..%%..%%.....
            .%%%%%...%%%%.....%%%%...%%..%%..%%......%%..%%..%%%%...
            .%%..%%..%%..........%%..%%..%%..%%.......%%%%...%%.....
            .%%..%%..%%%%%%...%%%%....%%%%...%%%%%%....%%....%%%%%%.
            ........................................................

            Moves colliders automatically by the minimum amount to not be intersecting or colliding anymore.
            Always a small gap between colliders.
        */


        internal static void RectangleOnRectangleResolve(Collider a, Collider b, out Vector2 move)
        {
            RectangleCollider recA = (RectangleCollider)a;
            RectangleCollider recB = (RectangleCollider)b;

            move = Vector2.Zero;

            int xOverlap = Math.Min(recA.WorldBoundingBox.x + recA.WorldBoundingBox.w, recB.WorldBoundingBox.x + recB.WorldBoundingBox.w) - Math.Max(recA.WorldBoundingBox.x, recB.WorldBoundingBox.x);
            int yOverlap = Math.Min(recA.WorldBoundingBox.y + recA.WorldBoundingBox.h, recB.WorldBoundingBox.y + recB.WorldBoundingBox.h) - Math.Max(recA.WorldBoundingBox.y, recB.WorldBoundingBox.y);
            if (xOverlap > 0 && yOverlap > 0)
            {
                Vector2 correction;
                if (xOverlap < yOverlap)
                    correction = new Vector2(recA.WorldBoundingBox.x < recB.WorldBoundingBox.x ? -xOverlap : xOverlap, 0);
                else
                    correction = new Vector2(0, recA.WorldBoundingBox.y < recB.WorldBoundingBox.y ? -yOverlap : yOverlap);

                move += correction;
            }

            MoveCollidersBy(a, b, move);
        }


        internal static void RectangleOnCircleResolve(Collider a, Collider b, out Vector2 move)
        {
            RectangleCollider rectangle = (a is CircleCollider) ? (RectangleCollider)b : (RectangleCollider)a;
            CircleCollider circle = (a is CircleCollider) ? (CircleCollider)a : (CircleCollider)b;

            move = Vector2.Zero;

            Vector2 closestDirectionToRectangle = ShapeHelper.ClosestDirectionFromCircleToRectangle(circle.WorldCenter, circle.Radius, rectangle.WorldBoundingBox);
            if (b is CircleCollider)
            {
                closestDirectionToRectangle *= -1;
            }

            float distanceSquared = ShapeHelper.DistanceSquaredFromCircleToRectangle(closestDirectionToRectangle);

            // It's kinda weird that we need to do this square root calculation. 
            // I'd like not to but for some reason it always then pushes the circle inside the square.
            float distance = (float)Math.Sqrt(distanceSquared);

            // Giggidy
            float penetration = circle.Radius - distance;
            if (distance != 0)
            {
                move = new Vector2(
                    closestDirectionToRectangle.X * penetration / distance,
                    closestDirectionToRectangle.Y * penetration / distance
                );
            }
            MoveCollidersBy(a, b, move);
        }


        internal static void CircleOnCircleResolve(Collider a, Collider b, out Vector2 move)
        {
            CircleCollider ac = (CircleCollider)a;
            CircleCollider bc = (CircleCollider)b;

            Vector2 acenter = ac.WorldCenter.ToVector2();
            Vector2 bcenter = bc.WorldCenter.ToVector2();

            Vector2 direction = acenter - bcenter;

            Vector2 normalized = Vector2.Normalize(direction);
            Vector2 optimal = normalized * (bc.Radius + ac.Radius);

            float overlap = optimal.Length() - direction.Length();
            move = normalized * overlap;
            MoveCollidersBy(a, b, move);
        }


        internal static void CircleOnPolygonResolve(Collider a, Collider b, out Vector2 move)
        {
            CircleCollider circle = (a is PolygonCollider) ? (CircleCollider)b : (CircleCollider)a;
            PolygonCollider polygon = (a is CircleCollider) ? (PolygonCollider)b : (PolygonCollider)a;

            move = Vector2.Zero;

            Vector2 dir = Vector2.Zero;

            if (a.IsStatic)
            {
                dir = polygon.WorldCenter.ToVector2() - circle.WorldCenter.ToVector2();
            }
            else if (b.IsStatic)
            {
                dir = circle.WorldCenter.ToVector2() - polygon.WorldCenter.ToVector2();
            }
            Vector2 normalized = Vector2.Normalize(dir);

            for (int i = 0; i < polygon.Edges.Count; i++)
            {
                Vector2 edgeStart = polygon.Edges[i].a.ToVector2();
                Vector2 edgeEnd = polygon.Edges[i].b.ToVector2();

                if (Vector2Extensions.LinesIntersect(edgeStart, edgeEnd, circle.WorldCenter.ToVector2(), polygon.WorldCenter.ToVector2(), out Vector2 intersection))
                {
                    if (ShapeHelper.IsPointOnLineSegment(edgeStart, edgeEnd, intersection))
                    {
                        float distToIntersection = (circle.WorldCenter.ToVector2() - intersection).Length();
                        float overlap = circle.Radius - distToIntersection;

                        if (overlap > 0)
                        {
                            move = normalized * overlap;
                        }
                    }
                }
            }
            MoveCollidersBy(a, b, move);
        }


        internal static void PolygonOnPolygonResolve(Collider a, Collider b, out Vector2 move)
        {
            PolygonCollider polygonA = (PolygonCollider)a;
            PolygonCollider polygonB = (PolygonCollider)b;

            move = GetMTVFromSAT(polygonA.Polygons, polygonA.Edges, polygonB.Polygons, polygonB.Edges);
            MoveCollidersBy(a, b, move);
        }


        internal static void RectangleOnPolygonResolve(Collider a, Collider b, out Vector2 move)
        {
            RectangleCollider rectangle = (a is PolygonCollider) ? (RectangleCollider)b : (RectangleCollider)a;
            PolygonCollider polygon = (a is RectangleCollider) ? (PolygonCollider)b : (PolygonCollider)a;

            move = Vector2.Zero;

            List<SDL_Point> points = new(){
                new (){
                    x = rectangle.WorldBoundingBox.x,
                    y = rectangle.WorldBoundingBox.y,
                },
                new (){
                    x = rectangle.WorldBoundingBox.x + rectangle.WorldBoundingBox.w,
                    y = rectangle.WorldBoundingBox.y,
                },
                new (){
                    x = rectangle.WorldBoundingBox.x + rectangle.WorldBoundingBox.w,
                    y = rectangle.WorldBoundingBox.y + rectangle.WorldBoundingBox.h,
                },
                new (){
                    x = rectangle.WorldBoundingBox.x,
                    y = rectangle.WorldBoundingBox.y + rectangle.WorldBoundingBox.h,
                },
            };

            List<Polygon> polygons = Polygon.SimpleTriangulateConvex(points);
            List<Edge> edges = Edge.EdgesFromPoints(points);

            move = GetMTVFromSAT(polygons, edges, polygon.Polygons, polygon.Edges);
            MoveCollidersBy(a, b, move);
        }


        /// <summary>
        /// Get Minimum Translation Vector from Separating Axis Theorem
        /// </summary>
        /// <param name="polygonsA"></param>
        /// <param name="polygonAEdges"></param>
        /// <param name="polygonsB"></param>
        /// <param name="polygonBEdges"></param>
        /// <returns></returns>
        internal static Vector2 GetMTVFromSAT(
            List<Polygon> polygonsA,
            List<Edge> polygonAEdges,
            List<Polygon> polygonsB,
            List<Edge> polygonBEdges
        )
        {
            float minOverlap = float.PositiveInfinity;
            Vector2 minAxis = Vector2.Zero;

            // Check edges of polygon A
            foreach (Edge edge in polygonAEdges)
            {
                var axis = Edge.GetPerpendicularAxis(edge);

                var projectionA = Polygon.ProjectOntoAxis(polygonsA, axis);
                var projectionB = Polygon.ProjectOntoAxis(polygonsB, axis);

                if (!projectionA.Overlaps(projectionB))
                {
                    // Separating axis found, no collision
                    return Vector2.Zero;
                }
                else
                {
                    // Find the overlap along this axis
                    float overlap = (float)Math.Min(projectionA.Max, projectionB.Max) - (float)Math.Max(projectionA.Min, projectionB.Min);
                    if (overlap < minOverlap)
                    {
                        minOverlap = overlap;
                        minAxis = axis;
                    }
                }
            }

            // Check edges of polygon B
            foreach (Edge edge in polygonBEdges)
            {
                var axis = Edge.GetPerpendicularAxis(edge);

                var projectionA = Polygon.ProjectOntoAxis(polygonsA, axis);
                var projectionB = Polygon.ProjectOntoAxis(polygonsB, axis);

                if (!projectionA.Overlaps(projectionB))
                {
                    // Separating axis found, no collision
                    return Vector2.Zero;
                }
                else
                {
                    // Find the overlap along this axis
                    float overlap = (float)Math.Min(projectionA.Max, projectionB.Max) - (float)Math.Max(projectionA.Min, projectionB.Min);
                    if (overlap < minOverlap)
                    {
                        minOverlap = overlap;
                        minAxis = axis;
                    }
                }
            }

            // Find a representative direction vector from polygonA to polygonB
            Vector2 direction = ShapeHelper.FindRepresentativeDirection(polygonsA, polygonsB);
            if (Vector2.Dot(minAxis, direction) < 0)
            {
                minAxis = -minAxis;
            }

            // Return the minimum translation vector to resolve the collision
            // NOTE: Had to flip the minAxis to not push it to the center
            return -minAxis * minOverlap;
        }


        private static void MoveCollidersBy(Collider a, Collider b, Vector2 move)
        {

            if (a.IsStatic)
            {
                Vector2 inverse = -move;
                b.Parent.Transform.Position += new Vector3(inverse.X, inverse.Y, 0);
            }
            else if (b.IsStatic)
            {
                a.Parent.Transform.Position += new Vector3(move.X, move.Y, 0);
            }
            else
            {
                Vector2 halfMove = move / 2;
                Vector2 inverse = -halfMove;
                a.Parent.Transform.Position += new Vector3(halfMove.X, halfMove.Y, 0);
                b.Parent.Transform.Position += new Vector3(inverse.X, inverse.Y, 0);
            }
        }

        // ##::::'##:'########:'##:::::::'########::'########:'########:::'######::
        // ##:::: ##: ##.....:: ##::::::: ##.... ##: ##.....:: ##.... ##:'##... ##:
        // ##:::: ##: ##::::::: ##::::::: ##:::: ##: ##::::::: ##:::: ##: ##:::..::
        // #########: ######::: ##::::::: ########:: ######::: ########::. ######::
        // ##.... ##: ##...:::: ##::::::: ##.....::: ##...:::: ##.. ##::::..... ##:
        // ##:::: ##: ##::::::: ##::::::: ##:::::::: ##::::::: ##::. ##::'##::: ##:
        // ##:::: ##: ########: ########: ##:::::::: ########: ##:::. ##:. ######::
        // ..:::::..::........::........::..:::::::::........::..:::::..:::......:::

        /// <summary>
        /// Gets all colliders within the area given. Very expensive so use sparingly.
        /// </summary>
        /// <param name="area"></param>
        /// <returns>List of colliders that are within the area</returns>
        public static List<Collider> GetCollidersInArea(SDL_Rect area)
        {
            List<Collider> allColliders = new();
            foreach (Entity ent in SceneManager.CurrentScene.entities)
            {
                allColliders.AddRange(ent.GetComponentsOfType<Collider>().Where(x => x.IsEnabled));
            }

            List<Collider> withinArea = new();
            foreach (Collider col in allColliders)
            {
                SDL_Rect colRect = col.WorldBoundingBox;

                // Tested to work with also rects that are completely within another rect.
                if (SDL_HasIntersection(ref colRect, ref area) == SDL_bool.SDL_TRUE)
                {
                    withinArea.Add(col);
                }
            }
            return withinArea;
        }

        public static void AddForce(this Entity ent, float amount, Force forceType)
        {
            PhysicsObject? po = ent.GetComponentInstance<PhysicsObject>(); 
            if (po != null)
            {

            }
        }
    }
}
