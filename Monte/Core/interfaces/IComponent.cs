using Monte.Abstractions;


namespace Monte.Interfaces
{
    public interface IComponent
    {
        Entity Parent { get; set; }

        void Initialize();
        void Update();
        void Destroy();
    }
}
