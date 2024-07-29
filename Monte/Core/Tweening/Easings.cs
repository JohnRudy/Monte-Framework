// Just make it a static class... This is stupid using structs...
// Or rather delegates or something else...

namespace Monte.Tweening
{
    public interface IEasing
    {
        /// <summary>
        /// Easing interface to make custom easing logic when needed
        /// </summary>
        /// <param name="a">Starting value</param>
        /// <param name="b">Ending value</param>
        /// <param name="c">Current value</param>
        /// <param name="t">time in 0-1 range</param>
        public void Ease(double a, double b, out double c, double t);
    }

    public struct Linear : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * t;
    }
    public struct SineIn : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Cos(t * Math.PI / 2));
    }
    public struct SineOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * Math.Sin(t * Math.PI / 2);
    }
    public struct SineInOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Cos(t * Math.PI)) / 2;
    }
    public struct QuadIn : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * Math.Pow(t, 2);
    }
    public struct QuadOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(1 - t, 2));
    }
    public struct QuadInOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? 2 * Math.Pow(t, 2) : 1 - Math.Pow(-2 * t + 2, 2) / 2);
    }
    public struct CubicIn : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * Math.Pow(t, 3);
    }
    public struct CubicOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(1 - t, 3));
    }
    public struct CubicInOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? 4 * Math.Pow(t, 3) : 1 - Math.Pow(-2 * t + 2, 3) / 2);
    }
    public struct QuartIn : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * Math.Pow(t, 4);
    }
    public struct QuartOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(1 - t, 4));
    }
    public struct QuartInOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? 8 * Math.Pow(t, 4) : 1 - Math.Pow(-2 * t + 2, 4) / 2);
    }
    public struct QuintIn : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * Math.Pow(t, 5);
    }
    public struct QuintOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(1 - t, 5));
    }
    public struct QuintInOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? 16 * Math.Pow(t, 5) : 1 - Math.Pow(-2 * t + 2, 5) / 2);
    }
    public struct ExpoIn : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * Math.Pow(2, 10 * (t - 1));
    }
    public struct ExpoOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(2, -10 * t));
    }
    public struct ExpoInOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? Math.Pow(2, 10 * (2 * t - 1)) / 2 : (2 - Math.Pow(2, -10 * (2 * t - 1))) / 2);
    }
    public struct BackIn : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (t * t * ((1.70158 + 1) * t - 1.70158));
    }
    public struct BackOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * ((t - 1) * (t - 1) * ((1.70158 + 1) * (t - 1) + 1.70158) + 1);
    }
    public struct BackInOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? 0.5 * (t * t * ((1.70158 + 1) * t - 1.70158)) : 0.5 * ((2 * t - 2) * (2 * t - 2) * ((1.70158 + 1) * (2 * t - 2) + 1.70158) + 2));
    }
    public struct CircIn : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Sqrt(1 - Math.Pow(t, 2)));
    }
    public struct CircOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * Math.Sqrt(1 - Math.Pow(t - 1, 2));
    }
    public struct CircInOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? (1 - Math.Sqrt(1 - Math.Pow(2 * t, 2))) / 2 : (Math.Sqrt(1 - Math.Pow(-2 * t + 2, 2)) + 1) / 2);
    }
    public struct ElasticIn : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (1 - Math.Pow(2, -10 * t) * Math.Sin((t - 0.075) * (2 * Math.PI) / 0.3));
    }
    public struct ElasticOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (1 + Math.Pow(2, 10 * (t - 1)) * Math.Sin((t - 0.075) * (2 * Math.PI) / 0.3));
    }
    public struct ElasticInOut : IEasing
    {
        public readonly void Ease(double a, double b, out double c, double t) => c = a + (b - a) * (t < 0.5 ? (1 - Math.Pow(2, -20 * t) * Math.Sin((t * 2 - 0.075) * (2 * Math.PI) / 0.3)) / 2 : (1 + Math.Pow(2, 10 * (2 * t - 2)) * Math.Sin((2 * t - 0.075) * (2 * Math.PI) / 0.3)) / 2);
    }
    public struct BounceIn : IEasing
    {
        public void Ease(double a, double b, out double c, double t) => c = Bounce.In(a, b, t);
    }

    public struct BounceOut : IEasing
    {
        public void Ease(double a, double b, out double c, double t) => c = Bounce.Out(a, b, t);
    }

    public struct BounceInOut : IEasing
    {
        public void Ease(double a, double b, out double c, double t) => c = Bounce.InOut(a, b, t);
    }

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
    public static class EasingMap
    {
        internal static Dictionary<Easing, IEasing> Maps = new(){
            {Easing.Linear, new Linear()},
            {Easing.SineIn, new SineIn() },
            {Easing.SineOut, new SineOut() },
            {Easing.SineInOut, new SineInOut() },
            {Easing.QuadIn, new QuadIn() },
            {Easing.QuadOut, new QuadOut() },
            {Easing.QuadInOut, new QuadInOut() },
            {Easing.CubicIn, new CubicIn() },
            {Easing.CubicOut, new CubicOut() },
            {Easing.CubicInOut, new CubicInOut() },
            {Easing.QuartIn, new QuartIn() },
            {Easing.QuartOut, new QuartOut() },
            {Easing.QuartInOut, new QuartInOut() },
            {Easing.QuintIn, new QuintIn() },
            {Easing.QuintOut, new QuintOut() },
            {Easing.QuintInOut, new QuintInOut() },
            {Easing.ExpoIn, new ExpoIn() },
            {Easing.ExpoOut, new ExpoOut() },
            {Easing.ExpoInOut, new ExpoInOut() },
            {Easing.BackIn, new BackIn() },
            {Easing.BackOut, new BackOut() },
            {Easing.BackInOut, new BackInOut() },
            {Easing.CircIn, new CircIn() },
            {Easing.CircOut, new CircOut() },
            {Easing.CircInOut, new CircInOut() },
            {Easing.ElasticIn, new ElasticIn() },
            {Easing.ElasticOut, new ElasticOut() },
            {Easing.ElasticInOut, new ElasticInOut() },
            {Easing.BounceIn, new BounceIn() },
            {Easing.BounceOut, new BounceOut() },
            {Easing.BounceInOut, new BounceInOut() }
        };
    }
}