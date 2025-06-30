namespace Monte.Tweening
{
    internal static class TweenCore
    {
        private static List<Tween> tweens = new();

        internal static void AddTween(Tween tween)
        {
            if (!tweens.Contains(tween))
                tweens.Add(tween);
        }

        internal static void RemoveTween(Tween tween)
        {
            if (tweens.Contains(tween))
                tweens.Add(tween);
        }

        internal static void Update(double deltaTime)
        {
            List<Tween> copy = new(tweens);
            copy.ForEach(t => t.Update(deltaTime));
        }
    }
}