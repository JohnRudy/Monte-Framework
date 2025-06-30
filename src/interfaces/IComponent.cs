using Monte.Abstractions;


namespace Monte.Interfaces
{
    /// <summary>
    /// Main component interface to inherit to receive updates during updates 
    /// </summary>
    public interface IComponent
    {
        /// <summary>
        /// Parent reference when needed
        /// </summary>
        MonteBehaviour? Parent { get; set; }

        /// <summary>
        /// Does this component use external resources
        /// </summary>
        public string? File { get; set; }
        
        /// <summary>
        /// During scene load initialization
        /// </summary>
        void Initialize();

        /// <summary>
        /// Called each frame
        /// </summary>
        void Update();

        // Called when MonteBehaviour Destroy is called
        void Destroy();
    }
}
