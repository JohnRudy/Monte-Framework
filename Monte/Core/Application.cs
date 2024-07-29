namespace Monte
{
    public static class Application
    {
        private static MonteEngine? monteEngine;
        internal static void Initialize(MonteEngine engine){
            monteEngine = engine;
        }
        public static void Close()
        {
            if (monteEngine == null) throw new Exception("No engine set!");
            monteEngine.CloseApplication();
        }
    }
}