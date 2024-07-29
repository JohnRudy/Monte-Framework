using Monte.Scenes;
using Monte.Abstractions;


namespace Monte.Internal
{
    internal interface IEntityFactory
    {
        T Create<T>() where T : Entity;
        T Create<T>(params object[] args) where T : Entity;
    }

    // T Might be null, warning disabled because exceptions are handled.
#pragma warning disable CS8600
    internal class EntityFactory : IEntityFactory
    {
        public T Create<T>() where T : Entity
        {
            try
            {
                T instance = (T)Activator.CreateInstance(typeof(T)) ?? throw new InvalidOperationException($"Could not create an instance of type {typeof(T).FullName}");
                return instance;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create an instance of type {typeof(T).FullName}", ex);
            }
        }

        public T Create<T>(params object[] args) where T : Entity
        {
            try
            {
                T instance = (T)Activator.CreateInstance(typeof(T), args) ?? throw new InvalidOperationException($"Could not create an instance of type {typeof(T).FullName} with the provided arguments");
                return instance;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create an instance of type {typeof(T).FullName} with the provided arguments", ex);
            }
        }
    }
#pragma warning restore CS8600

    internal static class EntityInstantiator
    {
        private static readonly IEntityFactory entityFactory = new EntityFactory();

        public static Entity Instantiate<T>() where T : Entity, new()
        {
            T instance = entityFactory.Create<T>();
            SceneManager.CurrentScene.entities.Add(instance);
            instance.Initialize();
            return instance;
        }

        public static Entity Instantiate<T>(params object[] args) where T : Entity
        {
            T instance = entityFactory.Create<T>(args);
            SceneManager.CurrentScene.entities.Add(instance);
            instance.Initialize();
            return instance;
        }
    }

}