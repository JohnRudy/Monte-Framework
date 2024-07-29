using Monte.Components;
using Monte.Interfaces;
using Monte.Internal;

namespace Monte.Abstractions
{
    public abstract class Entity : Enablable
    {
        public List<IComponent> Components = new() { };
        public Transform Transform;
        public string Tag = "";
        internal bool DestroyOnLoad { get; private set; }

        public Entity()
        {
            DestroyOnLoad = true;
            Transform = new(this);
        }

        public void DontDestroyOnLoad(bool value) => DestroyOnLoad = value;
        
        public bool RemoveComponent(IComponent component)
        {
            Components.Remove(component);
            return true;
        }
        public bool RemoveComponentInstance<T>() where T : IComponent
        {
            T? comp = Components.OfType<T>().FirstOrDefault();
            if (comp != null)
            {
                Components.Remove(comp);
                return true;
            }
            return false;
        }

        public IEnumerable<T> GetComponentsOfType<T>() where T : IComponent => Components.OfType<T>();

        public T? GetComponentInstance<T>() where T : IComponent => Components.OfType<T>().FirstOrDefault();

        internal void Initialize()
        {
            OnInitialize();
            Components.ForEach(x => x.Initialize());
        }

        public void Destroy()
        {
            OnDestroy();
            Components.ForEach(x => x.Destroy());
        }

        internal void Update()
        {
            if (Enabled)
            {
                OnUpdate();
                Components.ForEach(x => x.Update());
            }
        }
        internal void RenderUpdate(Renderer renderer)
        {
            OnRenderUpdate(renderer);
        }

        public static Entity Instantiate<T>() where T : Entity => EntityInstantiator.Instantiate<T>();
        public static Entity Instantiate<T>(object[] args) where T : Entity => EntityInstantiator.Instantiate<T>(args);

        public virtual void OnInitialize() { }
        public virtual void OnDestroy() { }
        public virtual void OnUpdate() { }
        public virtual void OnRenderUpdate(Renderer renderer) { }
        public virtual void OnCollision(Collider other) { }
        public virtual void OnCollisionStay(Collider other) { }
        public virtual void OnCollisionExit(Collider other) { }
        public virtual void OnTriggerEnter(Collider other) { }
        public virtual void OnTriggerExit(Collider other) { }
        public virtual void OnTriggerStay(Collider other) { }
    }
}