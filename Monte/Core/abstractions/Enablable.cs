namespace Monte.Abstractions
{
    public abstract class Enablable
    {
        private bool _Enabled = true;
        public bool Enabled { get => _Enabled; set => SetEnabled(value); }

        public void SetEnabled(bool enabled)
        {
            if (_Enabled != enabled)
            {
                _Enabled = enabled;
                if (_Enabled) OnEnable();
                else OnDisable();
            }
        }
        public virtual void OnEnable() { }
        public virtual void OnDisable() { }
    }
}