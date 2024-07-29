using Monte.Abstractions;


namespace Monte.Scenes
{
    public static class SceneManager
    {
        static readonly Dictionary<int, Scene> scenes = new();
        private static Scene _currentScene = new(-1, "OVERRIDEME");
        public static Scene CurrentScene
        {
            get => _currentScene;
            set => _currentScene = value;
        }


        private static void CheckForDontDestroy(Scene newScene)
        {
            List<Entity> extraloadEntitys = CurrentScene.entities.Where(x => x.DestroyOnLoad == false).ToList();
            extraloadEntitys.ForEach(x => CurrentScene.entities.Remove(x));
            newScene.entities.AddRange(extraloadEntitys);
        }

        public static void LoadScene(int sceneIndex)
        {
            Scene nextScene = scenes[sceneIndex];
            Scene oldScene = CurrentScene;
            CheckForDontDestroy(nextScene);
            nextScene.Load();
            CurrentScene = nextScene;
            oldScene?.Unload();
        }

        public static void LoadScene(string sceneName)
        {
            List<Scene> _scenes = scenes.Values.Where(s => s.SceneName == sceneName).ToList();
            if (_scenes.Count == 0) throw new Exception("SceneManager: A scene with given name could not be found");
            Scene? oldScene = CurrentScene;
            CheckForDontDestroy(_scenes[0]);
            _scenes[0].Load();
            CurrentScene = _scenes[0];
            oldScene.Unload();
        }

        public static void LoadScene(Scene scene)
        {
            Scene nextScene = scene;
            Scene? oldScene = CurrentScene;
            CheckForDontDestroy(nextScene);
            nextScene.Load();
            CurrentScene = nextScene;
            oldScene?.Unload();
        }

        public static void AddScene(Scene scene)
        {
            if (scenes.ContainsValue(scene))
                return;

            if (scenes.ContainsKey(scene.SceneIndex))
            {
                scene.SceneIndex = scenes.Count;
                Debug.Log($"A scene in SceneManager all ready contains the same Scene index for {scene.SceneName}, assinging new scene index {scene.SceneIndex}");
            }

            List<Scene> similiar = scenes.Values.Where(s => s.SceneName.StartsWith(scene.SceneName, StringComparison.OrdinalIgnoreCase)).ToList();

            if (similiar.Count != 0)
            {
                scene.SceneName += similiar.Count.ToString();
                Debug.Log($"A scene with the exact name was found all ready. Assinging a new scene name {scene.SceneName}");
            }

            scenes.Add(scene.SceneIndex, scene);
        }
    }
}