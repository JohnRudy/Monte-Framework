using static SDL2.SDL;
using Monte.Abstractions;


namespace Monte.Scenes
{
    public class Scene
    {
        public int SceneIndex;
        public string SceneName;
        public SDL_Color BackgroundColor = new() { r = 0, g = 0, b = 0, a = 255 };

        public Camera Camera = new();
        public List<Entity> entities = new();

        public Scene(int sceneIndex, string sceneName)
        {
            SceneIndex = sceneIndex;
            SceneName = sceneName;
            SceneManager.AddScene(this);
        }

        public void Load()
        {
            OnLoad();
            entities.ForEach(x => x.Initialize());
        }

        public bool LoadingIsDone(){
            OnLoad();
            entities.ForEach(x => x.Initialize());
            return true;
        }

        public void Unload()
        {
            OnUnload();
            entities.ForEach(x => x.Destroy());
        }
        internal void Update()
        {
            OnUpdate();
            entities.ForEach(x => x.Update());
        }
        internal void RenderUpdate(Renderer renderer)
        {
            OnRenderUpdate(renderer);
            entities.ForEach(x => x.RenderUpdate(renderer));
        }

        public virtual void OnLoad() { }
        public virtual void OnUnload() { }
        public virtual void OnUpdate() { }
        public virtual void OnRenderUpdate(Renderer renderer) { }
        public override string ToString() => $"{SceneName} {SceneIndex}";
    }
}