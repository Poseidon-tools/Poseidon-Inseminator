namespace Core.StateMachine
{
    using System;

    public abstract class State<T>  where T : Enum
    {
        #region Protected Variables
        protected readonly T StateType;
        protected IStateManager<T> stateManager;
        #endregion

        #region Public Methods
        protected State(IStateManager<T> stateManager, T stateType)
        {
            StateType = stateType;
            this.stateManager = stateManager;
        }

        public T GetStateType() => StateType;
        public virtual void OnEnter()
        {
        }
        public virtual void OnExit() { }
        public virtual void OnResume() { }
        public bool IsTypeOf(T stateType)
        {
            return GetStateType().Equals(stateType);
        }
        #endregion
    }
}