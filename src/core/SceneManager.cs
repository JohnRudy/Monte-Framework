using Monte.Abstractions;
using Monte.Interfaces;


namespace Monte.Core
{
    /// <summary>
    /// Main scene manager to use to load new scenes.
    /// </summary>
    public static class SceneManager
    {
        static readonly Dictionary<int, Scene> scenes = new();
        private static Scene? _currentScene = null;

        /// <summary>
        /// Current active scene
        /// </summary>
        public static Scene? CurrentScene
        {
            get => _currentScene;
            set => _currentScene = value;
        }

        private static void CheckForDontDestroy(Scene newScene)
        {
            if (CurrentScene == null) return;

            List<MonteBehaviour> extraloadMonteBehaviours = CurrentScene.Behaviours.Where(x => x.DestroyOnLoad == false).ToList();
            extraloadMonteBehaviours.ForEach(x => CurrentScene.Behaviours.Remove(x));
            newScene.Behaviours.AddRange(extraloadMonteBehaviours);
        }

        /// <summary>
        /// Load scene by it's index number
        /// </summary>
        /// <param name="sceneIndex">Scene.SceneIndex value</param>
        public static void LoadScene(int sceneIndex)
        {
            LoadScene(scenes[sceneIndex]);
        }

        /// <summary>
        /// Load scene by it's name
        /// </summary>
        /// <param name="sceneName">Scene.SceneName</param>
        /// <exception cref="Exception">If no scene found with name</exception>
        public static void LoadScene(string sceneName)
        {
            List<Scene> _scenes = scenes.Values.Where(s => s.SceneName == sceneName).ToList();
            if (_scenes.Count == 0) throw new Exception("SceneManager: A scene with given name could not be found");
            LoadScene(_scenes[0]);
        }

        /// <summary>
        /// LOad scene immediately with scene reference
        /// </summary>
        /// <param name="scene">Scene to load</param>
        public static void LoadScene(Scene scene)
        {
            Debug.Log($"SceneManager: Loading scene {scene.SceneIndex}: {scene.SceneName}");
            if (CurrentScene != null)
            {
                Debug.Log("SceneManager: Checking for don't destroy from old scene");
                CheckForDontDestroy(scene);

                List<string> ItemsFromScene(Scene x)
                {
                    List<string> items = [];
                    foreach (MonteBehaviour mb in x.Behaviours)
                    {
                        foreach (IComponent comp in mb.Components)
                        {
                            try
                            {
                                if (comp.File != null)
                                    items.Add(comp.File);
                            }
                            catch (NotImplementedException) { }
                        }
                    }
                    return items;
                }

                Debug.Log("SceneManager: Fetcing files to load from components");
                List<string> itemsToUnload = ItemsFromScene(CurrentScene).Except(ItemsFromScene(scene)).ToList();
                Debug.Log($"SceneManager: {itemsToUnload.Count} items found to unload");

                if (itemsToUnload.Count > 0)
                    Debug.Log("SceneManager: Unloading old files");
                    itemsToUnload.ForEach(x => ContentManager.UnloadItem(x));

                Debug.Log("SceneManager: Unloading previous scene");
                CurrentScene.Unload();
            }

            CurrentScene = scene;

            Debug.Log("SceneManager: Loading new scene");
            CurrentScene.Load();
        }

        /// <summary>
        /// Add a scene into the Scenemanare list of scenes
        /// </summary>
        /// <param name="scene">scene to add to list</param>
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