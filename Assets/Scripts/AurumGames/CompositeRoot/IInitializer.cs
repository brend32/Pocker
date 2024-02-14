using System;

namespace AurumGames.CompositeRoot
{
    public interface IInitializer
    {
        event Action<IInitializer> InitializationEnd;
        void Initialize(Context scope);
    }
}