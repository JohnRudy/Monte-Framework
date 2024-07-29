namespace Monte.Animation
{
    public struct AnimationClip
    {
        public string Name;
        public AnimationFrame[] Frames = Array.Empty<AnimationFrame>();
        public readonly int FrameCount => Frames.Length;

        public AnimationClip(string name, AnimationFrame[] frames)
        {
            Name = name;
            Frames = frames;
        }
    }
}