namespace Core.StateMachine
{
    using System;

    public interface IStateManager<T>
    {
        event Action<T> OnStateChanged;
        void SwitchState(T stateType);
        void SwitchToPrevious();
    }
}