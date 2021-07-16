namespace SDI.AppState.States
{
    using Core.StateMachine;
    using Core.ViewManager;

    public class ApplicationOutroState : State<ApplicationState>
    {
        #region Private Variables
        private ViewManager viewManager;
        #endregion
        #region Public Methods
        public ApplicationOutroState(IStateManager<ApplicationState> stateManager, ApplicationState stateType, ViewManager viewManager) 
            : base(stateManager, stateType)
        {
            this.viewManager = viewManager;
        }
        #endregion
    }
}