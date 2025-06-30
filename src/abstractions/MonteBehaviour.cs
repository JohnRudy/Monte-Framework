using Monte.Components;
using Monte.Core;
using Monte.Interfaces;

namespace Monte.Abstractions
{
    /// <summary>
    /// Main behaviour class. works as a collection of components on this spesific behaviour and get's update calles each frame. Components use this parent behaviour to determine locations and position in world. 
    /// </summary>
    public abstract class MonteBehaviour : Enablable
    {
        /// <summary>
        /// List of components this behaviour has.
        /// </summary>
        public List<IComponent> Components { get; private set; } = [];

        /// <summary>
        /// Main transform object of the behaviour.
        /// </summary>
        public Transform Transform;
        internal bool DestroyOnLoad { get; private set; }

        public MonteBehaviour()
        {
            DestroyOnLoad = true;
            Transform = new();
            AddComponent(Transform);
        }

        /// <summary>
        /// Called when this behaviour should persist between scenes. Once set to true, will be loaded each time Scene.Load is called and only stops this behaviour once set to false.
        /// </summary>
        /// <param name="value">value of persisting between scenes.</param>
        public void DontDestroyOnLoad(bool value) => DestroyOnLoad = value;

        /// <summary>
        ///  Retrieves each component from behaviours Components list of given type T. 
        /// </summary>
        /// <typeparam name="T">The IComponent inherited component</typeparam>
        /// <returns>IEnumerable<T></returns>
        public IEnumerable<T> GetComponentsOfType<T>() where T : IComponent => Components.OfType<T>();

        /// <summary>
        /// Get's the first instance of type T from this behaviours component list or null.
        /// </summary>
        /// <typeparam name="T">IComponent inherited component</typeparam>
        /// <returns>T or null</returns>
        public T? GetComponentInstance<T>() where T : IComponent => Components.OfType<T>().FirstOrDefault();

        internal void Initialize()
        {
            Debug.Log($"MonteBehaviour: Initialization");
            OnInitialize();
            foreach (IComponent c in Components)
            {
                Debug.Log($"IComponent: {c} Initialization");
                c.Initialize();
                Debug.Log($"IComponent: {c} Initialization done.");
            }
            Debug.Log($"MonteBehaviour: Initialization done");

            // Components.ForEach(x => x.Initialize());
        }

        /// <summary>
        /// Add a IComponent to this behaviour and set it's parent value.
        /// </summary>
        /// <param name="component">IComponent to be added</param>
        /// <returns>True</returns>
        public bool AddComponent(IComponent component)
        {
            Components.Add(component);
            component.Parent = this;
            return true;
        }


        /// <summary>
        /// Remove a spesific IComponent from this behaviour and set it's parent to null.
        /// </summary>
        /// <param name="component">IComponent to remove</param>
        /// <returns>True if successfully removed, false if not.</returns>
        public bool RemoveComponent(IComponent component)
        {
            if (Components.Contains(component))
            {
                Components.Remove(component);
                component.Parent = null;
                return true;
            }
            return false;
        }


        /// <summary>
        /// Removes all components of type T where T is of type IComponent from this behaviours components list and sets their parents to null.
        /// </summary>
        /// <typeparam name="T">IComponent of type T</typeparam>
        public void RemoveComponentsOfType<T>() where T : IComponent
        {
            var curcomp = new List<IComponent>(Components);
            curcomp.ForEach(x =>
            {
                if (x.GetType() == typeof(T))
                    RemoveComponent(x);
            });
        }

        /// <summary>
        /// Destroy this object from the scene and do not get update calls or act upon it's components.
        /// </summary>
        public void Destroy()
        {
            OnDestroy();
            Components.ForEach(x => x.Destroy());
            SceneManager.CurrentScene?.Behaviours.Remove(this);
        }

        internal void Update()
        {
            if (Enabled)
            {
                OnUpdate();
                Components.ForEach(x => x.Update());
            }
        }


        /// <summary>
        /// User method to implement. 
        /// Called once per Scene.load
        /// </summary>
        public virtual void OnInitialize() { }

        /// <summary>
        /// User method to implement. 
        /// Called once per MonteBehaviour.Destroy
        /// </summary>
        public virtual void OnDestroy() { }

        /// <summary>
        /// User method to implement. 
        /// Called each gameloop frame
        /// </summary>
        public virtual void OnUpdate() { }

        /// <summary>
        /// User method to implement. 
        /// Called on collision events when this behaviours collider component(s) collide with another Collider component if both are not marked as static.
        /// </summary>
        public virtual void OnCollision(Collider other) { }

        /// <summary>
        /// User method to implement.
        /// Called each frame when collision between two collider components persists if both are not marked as static colliders.
        /// </summary>
        public virtual void OnCollisionStay(Collider other) { }

        /// <summary>
        /// User method to implement.
        /// Called on that frame when collision has stopped between two behaviours collider components that are not marked as static. 
        /// </summary>
        public virtual void OnCollisionExit(Collider other) { }

        /// <summary>
        /// User method to implement.
        /// Called on when this collider enters another collider that is marked as a trigger collider. 
        /// </summary>
        public virtual void OnTriggerEnter(Collider other) { }

        /// <summary>
        /// User method to implement.
        /// Called on when this collider exits another collider that is marked as a trigger collider. 
        /// </summary>
        public virtual void OnTriggerExit(Collider other) { }

        /// <summary>
        /// User method to implement.
        /// Called each frame after the first when this collider stays within another collider that is marked as a trigger collider. 
        /// </summary>
        public virtual void OnTriggerStay(Collider other) { }
    }
}