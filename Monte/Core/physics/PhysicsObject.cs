using System.Numerics;
using Monte.Abstractions;
using Monte.Interfaces;


namespace Monte.Physics
{
    public class PhysicsObject : Enablable, IComponent
    {
        Entity _parent;
        public Entity Parent { get => _parent; set => _parent = value; }
        public List<Collider> ParentColliders
        {
            get
            {
                List<Collider> cols = new();
                if (Parent is null) return new();
                return Parent.GetComponentsOfType<Collider>().ToList();
            }
        }
        public bool IsEnabled => Parent is not null && Parent.Enabled && Enabled;

        public Vector2 Velocity = Vector2.Zero;
        public float Mass = 1.0f;

        public PhysicsObject(Entity parent)
        {
            _parent = parent;
            var po = _parent.GetComponentInstance<PhysicsObject>();
            if (po == null)
                _parent.Components.Add(this);
            else
                throw new ArgumentException("Cannot add two separate physics object per entity!");
        }

        public override void OnEnable() { }
        public override void OnDisable() { }

        public void Initialize() => OnInitialize();
        public virtual void OnInitialize() { }

        public void Update()
        {
            if (Enabled)
                OnUpdate();
        }
        public void OnUpdate() { }

        public void Destroy() => OnDestroy();
        public void OnDestroy() { }


    }
}