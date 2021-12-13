namespace Poseidon.StateMachine
{
    using System;
    using System.Collections.Generic;
    using PlayerLoopUtils;
    using UnityEngine;

    public class StateMachine<T> : IStateMachine<T> where T : Enum
    {
        #region Private Variables
        private Dictionary<T, State<T>> states;
        private T initialState = default;
        #endregion
        
        #region Public Variables
        public event Action<T> OnStateChanged;
        public State<T> PreviousState { get; private set; }

        public State<T> CurrentState { get; protected set; }
        
        public State<T>[] States { get; private set; }
        #endregion

        #region Public Methods
        
        public StateMachine(State<T>[] statesArray, T initialState = default)
        {
            PlayerLoopRunner.OnUpdate += StateMachineManagerOnUpdate;
            PlayerLoopRunner.OnPostLateUpdate += StateMachineManagerOnLateUpdate;

            this.initialState = initialState;
            States = statesArray;
            states = new Dictionary<T, State<T>>();

            foreach (var state in statesArray)
            {
                RegisterState(state);
            }
        }
        
        public void Run()
        {
            SwitchState(initialState);
        }
        
        public void Dispose()
        {
            PlayerLoopRunner.OnUpdate -= StateMachineManagerOnUpdate;
            PlayerLoopRunner.OnPostLateUpdate -= StateMachineManagerOnLateUpdate;
            
            CurrentState?.OnExit();
            CurrentState = null; 
        }
        #endregion

        private void RegisterState(State<T> newState)
        {
            newState.StateMachine = this;
            if (states.ContainsKey(newState.StateType))
            {
                Debug.LogError($"[StatesManager] There is already {newState.StateType} state type in dictionary.");
                return;
            }

            states.Add(newState.StateType, newState);
        }

        public void SwitchState(T stateType)
        {
            if (CurrentState != null)
            {
                if (CurrentState.IsTypeOf(stateType))
                {
                    Debug.Log($"[StateManager] State {stateType} is already running");
                    return;
                }
                CurrentState.OnExit();
            }

            if (!states.ContainsKey(stateType))
            {
                Debug.LogError($"[StatesManager] There is no ({stateType.ToString()}) state in dictionary.");
                return;
            }

            //Debug.Log($"[StateManager] State {stateType} is now active");
            PreviousState = CurrentState;
            CurrentState = states[stateType];

            CurrentState.OnEnter();
            OnStateChanged?.Invoke(CurrentState.StateType);
        }
        
        public void SwitchToPrevious()
        {
            if (PreviousState == null) return;
            SwitchState(PreviousState.StateType);
        }

        private void StateMachineManagerOnUpdate()
        {
            if(CurrentState is IUpdatable updatableState)
            {
                updatableState.OnUpdate();
            }
        }
        
        private void StateMachineManagerOnLateUpdate()
        {
            if(CurrentState is ILateUpdatable updatableState)
            {
                updatableState.OnLateUpdate();
            }
        }
    }
}
