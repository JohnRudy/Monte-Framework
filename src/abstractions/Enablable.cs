namespace Monte.Abstractions
{
    /// <summary>
    /// Helper abstract class to inherit when wanting enablable behaviour on a object.
    /// </summary>
    public abstract class Enablable
    {
        private bool _Enabled = true;

        /// <summary>
        /// boolean value which by setting to an !value calls the appropriate virtual method
        /// </summary>
        public bool Enabled { get => _Enabled; set => SetEnabled(value); }
        
        /// <summary>
        /// Helper method to call the appropriate enabled method same as setting the Enabled bool.
        /// </summary>
        /// <param name="enabled">Value to set the method to</param>
        public void SetEnabled(bool enabled)
        {
            if (_Enabled != enabled)
            {
                _Enabled = enabled;
                if (_Enabled) OnEnable();
                else OnDisable();
            }
        }

        /// <summary>
        /// User method to implement. Called when setting Enabled to True when it was False.
        /// </summary>
        public virtual void OnEnable() { }

        /// <summary>
        /// User method to implement. Called when setting Enabled to False when it was True.
        /// </summary>
        public virtual void OnDisable() { }
    }
}