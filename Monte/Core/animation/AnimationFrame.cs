namespace Monte.Animation
{
    public struct AnimationFrame
    {
        public int FrameIndex;
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public AnimationFrame(int frameIndex, int x, int y, int width, int height)
        {
            FrameIndex = frameIndex;
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}