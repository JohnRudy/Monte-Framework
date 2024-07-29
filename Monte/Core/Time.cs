namespace Monte
{
    public static class Time
    {
        // Initial value, gets updated after first frame.
        private static float _deltaTime = 0.016f;
        public static float DeltaTime => _deltaTime;
        internal static void SetDelta(uint delta)
        {
            _deltaTime = (float)delta / 1000;
            _gameTime += delta;
        }
        public static float CurrentFPS => 1 / DeltaTime;
        private static uint _gameTime = 0;
        public static uint GameTimeMS => _gameTime;
    }
}