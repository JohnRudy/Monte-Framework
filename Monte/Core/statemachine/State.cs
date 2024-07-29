using Monte.Abstractions;
using Monte.Interfaces;


namespace Monte.States
{
    public abstract class State : IComponent
    {
        Entity _parent;
        public Entity Parent
        {
            get => _parent;
            set => _parent = value;
        }
        StateMachine? StateMachine;

        bool IsAllowedToExecute { get => StateMachine?.CurrentState == this; }

        public State(Entity parent) {
            _parent = parent;
            _parent.Components.Add(this);
        }

        public void InitializeState(StateMachine sm) => StateMachine = sm;

        public virtual void OnStateEnter()
        {
            if (!IsAllowedToExecute) return;
        }
        public virtual void OnStateExit()
        {
            if (!IsAllowedToExecute) return;
        }
        public virtual void OnStateUpdate()
        {
            if (!IsAllowedToExecute) return;
        }

        void IComponent.Destroy() { }

        void IComponent.Initialize()
        {
            if (StateMachine is null && Parent is not null)
            {
                StateMachine? sm = Parent.GetComponentInstance<StateMachine>();
                if (sm is not null)
                {
                    sm.AddState(this);
                    StateMachine = sm;
                }
            }
        }

        void IComponent.Update()
        {
            if (!IsAllowedToExecute) return;
        }
    }
}