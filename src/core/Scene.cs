using static SDL.SDL_pixels;
using Monte.Abstractions;


namespace Monte.Core
{
    /// <summary>
    /// Main scene class to use to add behaviours to. 
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// This scenes index in build scenes
        /// </summary>
        public int SceneIndex;

        /// <summary>
        /// This scenes name 
        /// </summary>
        public string SceneName;

        /// <summary>
        /// Flat color to draw as background
        /// </summary>
        public SDL_Color BackgroundColor = new() { r = 35, g = 35, b = 35, a = 255 };

        /// <summary>
        /// List of behaviours in this scene
        /// </summary>
        public List<MonteBehaviour> Behaviours = new();

        public Scene(int sceneIndex, string sceneName)
        {
            SceneIndex = sceneIndex;
            SceneName = sceneName;
            SceneManager.AddScene(this);
        }

        internal void Load()
        {
            Debug.Log($"Scene: loading scene");
            OnLoad();
            Debug.Log($"Scene: loading behaviours");
            Behaviours.ForEach(x => x.Initialize());
            Debug.Log($"Scene: Scene load done");
        }

        internal void Unload()
        {
            OnUnload();
            Behaviours.ForEach(x => x.Destroy());
        }

        internal void Update()
        {
            OnUpdate();
            // To not get collection modified errors. This means removing a behaviour takes a frame.
            List<MonteBehaviour> copyBehaviours = new List<MonteBehaviour>(Behaviours); 
            copyBehaviours.ForEach(x => x.Update());
        }

        /// <summary>
        /// User method to implement that happens once during when SceneManager.LoadScene is called.
        /// </summary>
        public virtual void OnLoad() { }
        
        /// <summary>
        /// User method to implement that happens once during when SceneManager.LoadScene is called.
        /// </summary>
        public virtual void OnUnload() { }

        /// <summary>
        /// User method to implement that happens each gameloop update.
        /// </summary>
        public virtual void OnUpdate() { }

        public override string ToString() => $"{SceneName} {SceneIndex}";
    }
}