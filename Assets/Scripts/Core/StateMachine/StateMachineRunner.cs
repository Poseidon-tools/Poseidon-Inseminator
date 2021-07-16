namespace Core.StateMachine
{
    using System;
    using CustomMessages.Tools;
    using MessageDispatcher;
    using MessageDispatcher.Interfaces;
    using Sirenix.OdinInspector;
    using UnityEngine;

    [Serializable]
    public class StateMachineRunner<T,U> where U : Enum where T : StateManager<U>, new()
    {
        #region Public Variables
        public T StateManager { get; private set; }
        #endregion
        #region Private Variables
        [ShowInInspector] private bool isInitialized;
        private IMessageDispatcher messageDispatcher;
        #endregion
        #region Public API

        public StateMachineRunner()
        {
            StateManager = new T();
        }
        
        public void Initialize(State<U>[] initializedStates, U initialState)
        {
            messageDispatcher = MessageDispatcher.Instance;

            Debug.Log("Initializing state runner...");
            StateManager.Initialize(initializedStates, initialState);
            isInitialized = true;

            // stinky: the last place where better idea is missing
            // but: it's solving problem with nested state runners in normal/nested states
            // because we don't have to fetch anything via reflection from deep levels
            // SceneContainer.Instance.ResolveStateMachineDependencies(this);
            messageDispatcher.Send(new StateMachineToolMessages.OnStateRunnerStatusChanged(this, typeof(U), typeof(T), isInitialized));
        }

        public void Dispose()
        {
            StateManager.Dispose();
            isInitialized = false;
            messageDispatcher.Send(new StateMachineToolMessages.OnStateRunnerStatusChanged(this, typeof(U), typeof(T), isInitialized));
        }
        #endregion
    }
}