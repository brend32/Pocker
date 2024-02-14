using System;

namespace AurumGames.CompositeRoot
{
    public class OneTimeAction
    {
        public bool Executed { get; private set; }

        public void Call(Action action)
        {
            if (Executed)
                return;
            
            action.Invoke();
        }

        public void Reset()
        {
            Executed = false;
        }
    }
}