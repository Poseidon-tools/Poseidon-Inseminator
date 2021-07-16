namespace SDI.AppState
{
    using Core.MessageDispatcher;
    using Core.StateMachine;
    using Core.ViewManager;
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
        
        [SerializeField] 
        private StateMachineRunner<AppStateManager, ApplicationState> stateMachineRunner;
        #endregion
        #region Unity Methods
        private void Awake()
        {
            stateMachineRunner = new StateMachineRunner<AppStateManager, ApplicationState>();
            appViewManager.Initialize();
            stateMachineRunner.Initialize(new State<ApplicationState>[]
            {
                new AppIntroState(stateMachineRunner.StateManager, ApplicationState.Intro, appViewManager),
                new ApplicationExampleState(stateMachineRunner.StateManager, ApplicationState.ExampleState, appViewManager),
                new ApplicationOutroState(stateMachineRunner.StateManager, ApplicationState.Outro, appViewManager), 
            }, ApplicationState.Intro);
        }
        #endregion
    }
}