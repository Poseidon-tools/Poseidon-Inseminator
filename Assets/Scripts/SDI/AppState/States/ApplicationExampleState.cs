namespace SDI.AppState.States
{
    using Core.StateMachine;
    using Core.ViewManager;
    using DI;
    using Views;

    public class ApplicationExampleState : State<ApplicationState>
    {
        #region Private Variables
        [Attributes.Injectable]
        private ViewManager viewManager;

        [Attributes.Injectable]
        private MessageData messageData;

        [Attributes.Injectable(InstanceId = "CustomLoggerRed60")] 
        private ITextLogger testLogger;

        private ApplicationExampleView exampleView;

        [Attributes.NestedInjectable]
        private TestNestedModuleInjection nestedModuleInjection = new TestNestedModuleInjection();
        #endregion
        #region Public Methods
        public ApplicationExampleState(IStateManager<ApplicationState> stateManager, ApplicationState stateType, ViewManager viewManager) 
            : base(stateManager, stateType)
        {
        }

        public override void OnEnter()
        {
            base.OnEnter();
            exampleView = viewManager.GetView<ApplicationExampleView>();

            viewManager.SwitchView<ApplicationExampleView>();
            exampleView.NextButton.onClick.AddListener(OnNextHandler);
            testLogger.LogMessage(messageData.Message, exampleView.MessageText);
            nestedModuleInjection.Alert();
        }
        public override void OnExit()
        {
            exampleView.NextButton.onClick.AddListener(OnNextHandler);
        }
        #endregion
        #region Private Methods
        private void OnNextHandler()
        {
            stateManager.SwitchState(ApplicationState.Outro);
        }
        #endregion
    }
}