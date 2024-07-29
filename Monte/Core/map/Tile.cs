using Monte.Animation;


namespace Monte.Map
{
    public class Tile
    {
        public int ID;
        public int SheetX;
        public int SheetY;
        public string Tag;
        public AnimationClip? Clip;

        private int animationIndex = 0;
        private List<int>? frameTimes = new();
        private int NextFrameAt = 0;

        public Tile(int ID, int X, int Y, string Tag = "Default", AnimationClip? Clip = null, List<int>? frameTimes = null)
        {
            this.ID = ID;
            SheetX = X;
            SheetY = Y;
            this.Tag = Tag;
            this.Clip = Clip;
            this.frameTimes = frameTimes;

            if (Clip != null && frameTimes != null && frameTimes.Count > 0)
                NextFrameAt = frameTimes[0];
        }

        internal void Update(uint gameTimeInMS)
        {
            if (Clip != null && frameTimes != null && frameTimes.Count > 0)
            {
                if (gameTimeInMS >= NextFrameAt)
                {
                    animationIndex += 1;

                    NextFrameAt += frameTimes[animationIndex % Clip.Value.FrameCount];
                    SheetX = Clip.Value.Frames[animationIndex % Clip.Value.FrameCount].X;
                    SheetY = Clip.Value.Frames[animationIndex % Clip.Value.FrameCount].Y;
                }
            }
        }
    }
}