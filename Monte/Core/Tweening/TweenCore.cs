namespace Monte.Tweening
{
    public static class TweenCore
    {
        private static List<Tween> tweens = new();

        private static void TweenComplete(Tween tween)
        {
            if (tweens.Contains(tween))
                tweens.Remove(tween);
        }

        public static void AddTween(Tween tween)
        {
            if (!tweens.Contains(tween))
            {
                tweens.Add(tween);
                tween.OnCompleteAction += () => TweenComplete(tween);
            }
        }

        public static void Update(double deltaTime)
        {
            List<Tween> copy = new(tweens);
            copy.ForEach(t => t.Update(deltaTime));
        }
    }
}