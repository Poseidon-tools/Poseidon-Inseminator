namespace SDI.AppState
{
    using System;
    using Core.MessageDispatcher;
    using Core.ViewManager;
    using Poseidon.StateMachine;
    using Sirenix.OdinInspector;
    using States;
    using UnityEngine;

    public class AppManager : SerializedMonoBehaviour 
    {
        #region Inspector
        [SerializeField] private ViewManager appViewManager;
        [SerializeField] private MessageDispatcher messageDispatcher;
        #endregion
        #region Private Variables

        private StateMachine<ApplicationState> appStateManager = new StateMachine<ApplicationState>(
            new State<ApplicationState>[]
            {
                new AppIntroState(),
                new ApplicationExampleState(),
                new ApplicationOutroState()
            });
        #endregion
        #region Unity Methods
        private void Awake()
        {
            appViewManager.Initialize();
        }

        private void OnEnable()
        {
            appStateManager.Run();
        }

        private void OnDisable()
        {
            appStateManager.Dispose();
        }
        #endregion
    }
}