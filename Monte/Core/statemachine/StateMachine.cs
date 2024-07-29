using Monte.Abstractions;
using Monte.Interfaces;


namespace Monte.States
{
    public class StateMachine : Enablable, IComponent
    {
        public State? CurrentState;
        Entity _parent;
        public Entity Parent { get => _parent; set => _parent = value; }
        public List<State> _states = new();

        public StateMachine(Entity parent) {
            _parent = parent;
            _parent.Components.Add(this);
        }

        private void MakeSwitch(State state)
        {
            CurrentState?.OnStateExit();
            CurrentState = state;
            CurrentState.OnStateEnter();
        }

        public bool SwitchState<SwitchState>() where SwitchState : State
        {
            State? state = _states.OfType<SwitchState>().FirstOrDefault();

            if (CurrentState == state) return true;

            if (state is not null)
            {
                MakeSwitch(state);
                return true;
            }

            if (Parent is null) return false;

            State? comp = Parent.GetComponentInstance<SwitchState>();
            if (comp is not null)
            {
                _states.Add(comp);
                MakeSwitch(comp);
                return true;
            }
            return false;
        }

        private bool HasState(Type stateType) => _states.Any(s => stateType.IsInstanceOfType(s));

        public void AddState(State state)
        {
            var stateType = state.GetType();

            if (!HasState(stateType))
            {
                _states.Add(state);
            }
        }

        public override void OnEnable() { }
        public override void OnDisable() { }
        public virtual void OnUpdate() { }

        void IComponent.Initialize() => _states.ForEach(x => x.InitializeState(this));

        void IComponent.Update()
        {
            if (Enabled)
            {
                OnUpdate();
                CurrentState?.OnStateUpdate();
            }
        }

        void IComponent.Destroy() { }
    }
}