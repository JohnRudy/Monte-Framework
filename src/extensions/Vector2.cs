using System.Numerics;

namespace Monte.Extensions
{
    /// <summary>
    /// Vector2 assistance methods
    /// </summary>
    public static class Vec2
    {
        /// <summary>
        /// Returns boolean value if two vector2 lines intersect and the point of intersection
        /// </summary>
        /// <param name="line1Start"></param>
        /// <param name="line1End"></param>
        /// <param name="line2Start"></param>
        /// <param name="line2End"></param>
        /// <param name="intersection"></param>
        /// <returns>boolean if intersection happens</returns>
        public static bool LinesIntersect(Vector2 line1Start, Vector2 line1End, Vector2 line2Start, Vector2 line2End, out Vector2 intersection)
        {
            intersection = Vector2.Zero;

            // Calculate the vectors of the lines
            Vector2 line1Vec = line1End - line1Start;
            Vector2 line2Vec = line2End - line2Start;

            // Calculate the determinant
            float determinant = line1Vec.X * line2Vec.Y - line1Vec.Y * line2Vec.X;

            // If the determinant is zero, lines are parallel and don't intersect
            if (Math.Abs(determinant) < 0.0001f)
                return false;

            // Calculate the intersection point
            float t = ((line2Start.X - line1Start.X) * line2Vec.Y - (line2Start.Y - line1Start.Y) * line2Vec.X) / determinant;
            intersection = line1Start + t * line1Vec;

            return true;
        }
    }
}