namespace Monte.Tweening
{
    public class Tween
    {
        public double Value = 0;
        private readonly double a;
        private readonly double b;
        private readonly double len = 0;
        private double t;
        private double passedTime = 0;
        private IEasing _easing;

        public event Action? OnCompleteAction = null;
        public event Action<double>? OnValueChangeAction = null;

        public Tween(double a, double b, double lengthInSeconds, Easing easing = Easing.Linear)
        {
            _easing = EasingMap.Maps[easing];

            this.a = a;
            this.b = b;

            len = lengthInSeconds;
            Value = a;
            TweenCore.AddTween(this);
        }
        public void ResetTween()
        {
            Value = a;
            passedTime = 0;
        }

        public void Update(double deltaTime)
        {
            if (deltaTime == 0)
                return;

            if (len == 0)
            {
                Value = b;
                OnValueChangeAction?.Invoke(Value);
                OnCompleteAction?.Invoke();
            }
            if (Value == b)
                return;

            passedTime += deltaTime;
            t = Math.Clamp(passedTime / len, 0, 1);

            _easing.Ease(a, b, out Value, t);

            Value = Math.Clamp(Value, Math.Min(a, b), Math.Max(a, b));

            if (Value == b)
                OnCompleteAction?.Invoke();
            else
                OnValueChangeAction?.Invoke(Value);
        }
    }
}
