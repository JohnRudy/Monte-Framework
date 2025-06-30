namespace Monte.Tweening
{
    /// <summary>
    /// Main tween object to create and read the value of to get the tweens.
    /// </summary>
    public class Tween
    {
        /// <summary>
        /// Current value of the tween
        /// </summary>
        public double Value = 0;
        
        private double a;
        private double b;
        private double lengthInSeconds = 0;
        private double t;
        private double passedTime = 0;
        private Easing usedEasing = Easing.Linear;

        /// <summary>
        /// Event called when tween has completed
        /// </summary>
        public event Action? OnCompleteAction = null;
        
        /// <summary>
        /// Called eachtime Value has changed 
        /// </summary>
        public event Action<double>? OnValueChangeAction = null;
        public bool Done = false;

        
        public Tween(double a, double b, double lengthInSeconds, Easing easing = Easing.Linear)
        {
            this.a = a;
            this.b = b;

            usedEasing = easing;

            this.lengthInSeconds = lengthInSeconds;
            Value = a;
            TweenCore.AddTween(this);
        }
        
        internal void Update(double deltaTime)
        {
            if (deltaTime == 0)
                return;

            passedTime += deltaTime;
            t = Math.Clamp(passedTime / lengthInSeconds, 0, 1);

            EasingMap.Maps[usedEasing](a, b, out Value, t);

            Value = Math.Clamp(Value, Math.Min(a, b), Math.Max(a, b));

            if (Value == b) {
                Done = true;
                OnCompleteAction?.Invoke();
            }
            else
                OnValueChangeAction?.Invoke(Value);
        }

        /// <summary>
        /// Destroy this tween and do not update it anymore
        /// </summary>
        public void Destroy(){
            TweenCore.RemoveTween(this);
        }


        /// <summary>
        /// Restarts the tween with given values.
        /// </summary>
        /// <param name="a">start</param>
        /// <param name="b">end</param>
        /// <param name="lengthInSeconds">length</param>
        /// <param name="easing">what easing to use</param>
        public void ChangeValues(double a, double b, double lengthInSeconds, Easing easing = Easing.Linear){
            this.a = a;
            this.b = b;
            this.lengthInSeconds = lengthInSeconds;
            Done = false;
            passedTime = t = 0;
        }
    }
}
