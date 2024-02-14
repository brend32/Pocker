namespace AurumGames.CompositeRoot
{
    /// <summary>
    /// Simple state machine
    /// </summary>
    /// <typeparam name="T">State type</typeparam>
    public sealed class StateMachine<T>
    {
        public delegate void StatedEventArgs(T previous, T current);
        
        /// <summary>
        /// State changed
        /// </summary>
        public event StatedEventArgs StateChanged;

        public T CurrentState { get; private set; }
        public T PreviousState { get; private set; }


        public StateMachine(T current)
        {
            CurrentState = current;
        }

        /// <summary>
        /// Change state
        /// </summary>
        /// <param name="state">New state</param>
        public void SetState(T state)
        {
            ChangeState(state);
        }

        private void ChangeState(T state)
        {
            PreviousState = CurrentState;
            CurrentState = state;
            StateChanged?.Invoke(PreviousState, CurrentState);
        }
    }
}