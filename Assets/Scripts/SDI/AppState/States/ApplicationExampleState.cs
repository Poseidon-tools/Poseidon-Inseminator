namespace SDI.AppState.States
{
    using Core.ViewManager;
    using Poseidon.StateMachine;
    using PoseidonDI.Scripts;
    using PoseidonDI.Scripts.Example;
    using Views;

    public class ApplicationExampleState : State<ApplicationState>
    {
        #region Private Variables
        [PoseidonAttributes.Injectable]
        private ViewManager viewManager;

        [PoseidonAttributes.Injectable]
        private MessageData messageData;

        [PoseidonAttributes.Injectable(InstanceId = "CustomLoggerRed60")] 
        private ITextLogger testLogger;

        private ApplicationExampleView exampleView;

        [PoseidonAttributes.NestedInjectable]
        private TestNestedModuleInjection nestedModuleInjection = new TestNestedModuleInjection();
        #endregion
        #region Public Methods
        public override ApplicationState StateType => ApplicationState.ExampleState;

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
            StateMachine.SwitchState(ApplicationState.Outro);
        }
        #endregion
    }
}