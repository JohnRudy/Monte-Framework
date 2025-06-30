using System.Numerics;
using static SDL.SDL_rect;
using Monte.Core;


namespace Monte.Lib
{
    /// <summary>
    /// Polygon shape helper class
    /// </summary>
    public static class ShapeHelper
    {
        public static Vector2 ClosestDirectionFromCircleToRectangle(SDL_FPoint circleCenter, SDL_FRect rectangle)
        {
            // Calculate the center of the circle relative to the rectangle
            float circleX = circleCenter.x - rectangle.x;
            float circleY = circleCenter.y - rectangle.y;

            // Calculate the closest point on the rectangle to the circle
            float closestX = Math.Clamp(circleX, 0, rectangle.w);
            float closestY = Math.Clamp(circleY, 0, rectangle.h);

            float distanceX = circleX - closestX;
            float distanceY = circleY - closestY;
            return new Vector2(distanceX, distanceY);
        }
        public static Vector2 FindRepresentativeDirection(List<Polygon> polygonsA, List<Polygon> polygonsB)
        {
            // Take the first vertex from each polygon as a representative point
            SDL_FPoint pointA = polygonsA.First().Vertices.First();
            SDL_FPoint pointB = polygonsB.First().Vertices.First();

            return new Vector2(pointB.x - pointA.x, pointB.y - pointA.y);
        }
        public static float DistanceSquaredFromCircleToRectangle(Vector2 closestPointToRectangle)
        {
            return (closestPointToRectangle.X * closestPointToRectangle.X) + (closestPointToRectangle.Y * closestPointToRectangle.Y);
        }

        public static bool IsCircleWithinSquare(float radius, float distanceToRectangleSquared)
        {
            return distanceToRectangleSquared < (radius * radius);
        }

        public static bool IsPointOnLineSegment(Vector2 start, Vector2 end, Vector2 point)
        {
            return Math.Abs((end - start).Length() - ((point - start).Length() + (end - point).Length())) < 0.0001f;
        }
    }
}