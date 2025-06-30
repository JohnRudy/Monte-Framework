// Just make it a static class... This is stupid using structs...
// Or rather delegates or something else...

namespace Monte.Tweening
{
    /// <summary>
    /// Easings static class for each Easing enumerator preset. Returns the out c of 0-1 t value.
    /// </summary>
    public static class Easings
    {
        public static void Linear(double a, double b, out double c, double t) => c = a + (b - a) * t;
        public static void SineIn(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Cos(t * Math.PI / 2));
        public static void SineOut(double a, double b, out double c, double t) => c = a + (b - a) * Math.Sin(t * Math.PI / 2);
        public static void SineInOut(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Cos(t * Math.PI)) / 2;
        public static void QuadIn(double a, double b, out double c, double t) => c = a + (b - a) * Math.Pow(t, 2);
        public static void QuadOut(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(1 - t, 2));
        public static void QuadInOut(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? 2 * Math.Pow(t, 2) : 1 - Math.Pow(-2 * t + 2, 2) / 2);
        public static void CubicIn(double a, double b, out double c, double t) => c = a + (b - a) * Math.Pow(t, 3);
        public static void CubicOut(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(1 - t, 3));
        public static void CubicInOut(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? 4 * Math.Pow(t, 3) : 1 - Math.Pow(-2 * t + 2, 3) / 2);
        public static void QuartIn(double a, double b, out double c, double t) => c = a + (b - a) * Math.Pow(t, 4);
        public static void QuartOut(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(1 - t, 4));
        public static void QuartInOut(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? 8 * Math.Pow(t, 4) : 1 - Math.Pow(-2 * t + 2, 4) / 2);
        public static void QuintIn(double a, double b, out double c, double t) => c = a + (b - a) * Math.Pow(t, 5);
        public static void QuintOut(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(1 - t, 5));
        public static void QuintInOut(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? 16 * Math.Pow(t, 5) : 1 - Math.Pow(-2 * t + 2, 5) / 2);
        public static void ExpoIn(double a, double b, out double c, double t) => c = a + (b - a) * Math.Pow(2, 10 * (t - 1));
        public static void ExpoOut(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(2, -10 * t));
        public static void ExpoInOut(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? Math.Pow(2, 10 * (2 * t - 1)) / 2 : (2 - Math.Pow(2, -10 * (2 * t - 1))) / 2);
        public static void BackIn(double a, double b, out double c, double t) => c = a + (b - a) * (t * t * ((1.70158 + 1) * t - 1.70158));
        public static void BackOut(double a, double b, out double c, double t) => c = a + (b - a) * ((t - 1) * (t - 1) * ((1.70158 + 1) * (t - 1) + 1.70158) + 1);
        public static void BackInOut(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? 0.5 * (t * t * ((1.70158 + 1) * t - 1.70158)) : 0.5 * ((2 * t - 2) * (2 * t - 2) * ((1.70158 + 1) * (2 * t - 2) + 1.70158) + 2));
        public static void CircIn(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Sqrt(1 - Math.Pow(t, 2)));
        public static void CircOut(double a, double b, out double c, double t) => c = a + (b - a) * Math.Sqrt(1 - Math.Pow(t - 1, 2));
        public static void CircInOut(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? (1 - Math.Sqrt(1 - Math.Pow(2 * t, 2))) / 2 : (Math.Sqrt(1 - Math.Pow(-2 * t + 2, 2)) + 1) / 2);
        public static void ElasticIn(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(2, -10 * t) * Math.Sin((t - 0.075) * (2 * Math.PI) / 0.3));
        public static void ElasticOut(double a, double b, out double c, double t) => c = a + (b - a) * (1 + Math.Pow(2, 10 * (t - 1)) * Math.Sin((t - 0.075) * (2 * Math.PI) / 0.3));
        public static void ElasticInOut(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? (1 - Math.Pow(2, -20 * t) * Math.Sin((t * 2 - 0.075) * (2 * Math.PI) / 0.3)) / 2 : (1 + Math.Pow(2, 10 * (2 * t - 2)) * Math.Sin((2 * t - 0.075) * (2 * Math.PI) / 0.3)) / 2);
        public static void BounceIn(double a, double b, out double c, double t) => c = Bounce.In(a, b, t);
        public static void BounceOut(double a, double b, out double c, double t) => c = Bounce.Out(a, b, t);
        public static void BounceInOut(double a, double b, out double c, double t) => c = Bounce.InOut(a, b, t);

        internal static class Bounce
        {
            internal static double In(double a, double b, double t)
            {
                return b - Out(0, b - a, 1 - t) + a;
            }

            internal static double Out(double a, double b, double t)
            {
                if (t < (1 / 2.75))
                {
                    return b * (7.5625 * t * t) + a;
                }
                else if (t < (2 / 2.75))
                {
                    t -= 1.5 / 2.75;
                    return b * (7.5625 * t * t + 0.75) + a;
                }
                else if (t < (2.5 / 2.75))
                {
                    t -= 2.25 / 2.75;
                    return b * (7.5625 * t * t + 0.9375) + a;
                }
                else
                {
                    t -= 2.625 / 2.75;
                    return b * (7.5625 * t * t + 0.984375) + a;
                }
            }
            internal static double InOut(double a, double b, double t)
            {
                if (t < 0.5)
                    return 0.5 * In(0, b - a, t * 2) + a;
                else
                    return 0.5 * Out(0, b - a, t * 2 - 1) + 0.5 * (b - a) + a;
            }
        }
    }

    public enum Easing
    {
        Linear,
        SineIn,
        SineOut,
        SineInOut,
        QuadIn,
        QuadOut,
        QuadInOut,
        CubicIn,
        CubicOut,
        CubicInOut,
        QuartIn,
        QuartOut,
        QuartInOut,
        QuintIn,
        QuintOut,
        QuintInOut,
        ExpoIn,
        ExpoOut,
        ExpoInOut,
        BackIn,
        BackOut,
        BackInOut,
        CircIn,
        CircOut,
        CircInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut,
        BounceIn,
        BounceOut,
        BounceInOut,
    }

    internal static class EasingMap
    {
        internal delegate void Easingf(double a, double b, out double c, double t);
        internal static Dictionary<Easing, Easingf> Maps = new(){
            {Easing.Linear, Easings.Linear},
            {Easing.SineIn, Easings.SineIn },
            {Easing.SineOut, Easings.SineOut },
            {Easing.SineInOut, Easings.SineInOut },
            {Easing.QuadIn, Easings.QuadIn },
            {Easing.QuadOut, Easings.QuadOut },
            {Easing.QuadInOut, Easings.QuadInOut },
            {Easing.CubicIn, Easings.CubicIn },
            {Easing.CubicOut, Easings.CubicOut },
            {Easing.CubicInOut, Easings.CubicInOut },
            {Easing.QuartIn, Easings.QuartIn },
            {Easing.QuartOut, Easings.QuartOut },
            {Easing.QuartInOut, Easings.QuartInOut },
            {Easing.QuintIn, Easings.QuintIn },
            {Easing.QuintOut, Easings.QuintOut },
            {Easing.QuintInOut, Easings.QuintInOut },
            {Easing.ExpoIn, Easings.ExpoIn },
            {Easing.ExpoOut, Easings.ExpoOut },
            {Easing.ExpoInOut, Easings.ExpoInOut },
            {Easing.BackIn, Easings.BackIn },
            {Easing.BackOut, Easings.BackOut },
            {Easing.BackInOut, Easings.BackInOut },
            {Easing.CircIn, Easings.CircIn },
            {Easing.CircOut, Easings.CircOut },
            {Easing.CircInOut, Easings.CircInOut },
            {Easing.ElasticIn, Easings.ElasticIn },
            {Easing.ElasticOut, Easings.ElasticOut },
            {Easing.ElasticInOut, Easings.ElasticInOut },
            {Easing.BounceIn, Easings.BounceIn },
            {Easing.BounceOut, Easings.BounceOut },
            {Easing.BounceInOut, Easings.BounceInOut }
        };
    }
}