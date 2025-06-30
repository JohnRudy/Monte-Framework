using System.Numerics;


namespace Monte.Lib
{
    /// <summary>
    /// Custom Matrix3x3 class to help with translation and scaling.
    /// </summary>
    public class Matrix3x3
    {
        public float[,] Values { get; private set; }

        public Matrix3x3()
        {
            // Initialize as identity matrix
            Values = new float[3, 3] {
            { 1, 0, 0 },
            { 0, 1, 0 },
            { 0, 0, 1 }
        };
        }

        public static Matrix3x3 CreateScalingAndTranslation(float scaleX, float scaleY, float translateX, float translateY)
        {
            var matrix = new Matrix3x3();
            matrix.Values[0, 0] = scaleX;
            matrix.Values[1, 1] = scaleY;
            matrix.Values[0, 2] = translateX;
            matrix.Values[1, 2] = translateY;
            return matrix;
        }

        public Vector2 Multiply(Vector2 vector)
        {
            float x = Values[0, 0] * vector.X + Values[0, 1] * vector.Y + Values[0, 2];
            float y = Values[1, 0] * vector.X + Values[1, 1] * vector.Y + Values[1, 2];
            return new Vector2(x, y);
        }
    }
}