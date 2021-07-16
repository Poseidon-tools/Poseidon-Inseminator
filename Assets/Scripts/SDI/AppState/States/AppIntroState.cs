namespace SDI.AppState.States
{
    using Core.StateMachine;
    using Core.ViewManager;
    using Views;

    public class AppIntroState : State<ApplicationState>
    {
        #region Private Variables
        private ViewManager viewManager;
        private ApplicationIntroView introView;
        
        #endregion
        #region Public Methods
        public AppIntroState(IStateManager<ApplicationState> stateManager, ApplicationState stateType, ViewManager viewManager) 
            : base(stateManager, stateType)
        {
            this.viewManager = viewManager;
            introView = viewManager.GetView<ApplicationIntroView>();
           
        }
        public override void OnEnter()
        {
            viewManager.SwitchView<ApplicationIntroView>();
            introView.NextButton.onClick.AddListener(OnNextHandler);
        }
        public override void OnExit()
        {
            introView.NextButton.onClick.RemoveListener(OnNextHandler);
        }
        #endregion
        #region Private Methods
        private void OnNextHandler()
        {
            stateManager.SwitchState(ApplicationState.ExampleState);
        }
        #endregion
    }
}