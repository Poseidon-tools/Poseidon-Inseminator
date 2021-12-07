namespace Poseidon.StateMachine
{
    using System;
    public interface IStateMachine<T> : IDisposable
    {
        event Action<T> OnStateChanged;
        void Run();
        void SwitchState(T stateType);
        void SwitchToPrevious();
    }
}