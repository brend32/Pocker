using UnityEngine;

namespace AurumGames.CompositeRoot
{
    public interface ILazyDependency
    {
        internal void Resolve(object value);
    }
    
    /// <summary>
    /// Dependency that wasn't been registered during Context.Resolve call
    /// And lately was registered 
    /// </summary>
    /// <typeparam name="T">Dependency</typeparam>
    public sealed class LazyDependency<T> : ILazyDependency where T : class
    {
        public delegate void ReceivedDependencyArgs(T value);
        
        /// <summary>
        /// Value
        /// </summary>
        public T Value { get; private set; }
        /// <summary>
        /// Is resolved
        /// </summary>
        public bool HasValue { get; private set; }

        private event ReceivedDependencyArgs Listeners;

        void ILazyDependency.Resolve(object value)
        {
            Value = (T)value;
            HasValue = true;
            
            Debug.Log("Notify " + typeof(T).FullName);
            Listeners?.Invoke(Value);
        }

        /// <summary>
        /// Subscribes on dependency resolution
        /// </summary>
        /// <param name="listener">Listener callback</param>
        public void Notify(ReceivedDependencyArgs listener)
        {
            if (HasValue)
            {
                listener?.Invoke(Value);
                return;
            }

            Listeners += listener;
        } 
    }
}