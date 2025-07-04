using System.Collections;


namespace Monte.Core
{
    public static class CoroutineHandler
    {
        private static Coroutine? currentCoroutine;

        public static Coroutine StartCoroutine(IEnumerator routine)
        {
            currentCoroutine = new Coroutine(routine);
            currentCoroutine.MoveNext();
            return currentCoroutine;
        }
    }

    public class Coroutine
    {
        private readonly IEnumerator _routine;

        public Coroutine(IEnumerator routine) => _routine = routine;

        public async void MoveNext()
        {
            while (_routine.MoveNext())
            {
                object result = _routine.Current;

                if (result is WaitForSeconds waitTime)
                {
                    await Task.Delay((int)(waitTime.Seconds * 1000));
                }
                else if (result is WaitFor waitFor)
                {
                    // Danger danger
                    while (!waitFor.Check())
                    {
                        await Task.Delay(16);
                    }
                }
            }
        }
    }

    public class WaitFor
    {
        private readonly Func<bool> _check;
        public Action? Callback { get; set; }

        public WaitFor(Func<bool> check) => _check = check;

        public bool Check() => _check();
    }

    public class WaitForSeconds
    {
        public double Seconds { get; private set; }
        public WaitForSeconds(double seconds) => Seconds = seconds;
    }
}