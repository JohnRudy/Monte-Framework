using Monte.Abstractions;
using Monte.Interfaces;


namespace Monte.Audio
{
    public class AudioListener : IComponent
    {
        Entity _parent;
        public Entity Parent { get => _parent; set => _parent = value; }

        public AudioListener(Entity parent)
        {
            if (parent.GetType() != typeof(Camera))
                throw new Exception("Only a scenes Camera object should have a audio listener in it!");
            _parent = parent;
            _parent.Components.Add(this);
        }

        public void Destroy() { }
        public void Initialize() { }
        public void Update() { }
    }
}