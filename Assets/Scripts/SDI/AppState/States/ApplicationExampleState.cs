namespace SDI.AppState.States
{
    using Core.ViewManager;
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using Poseidon.StateMachine;
    using Views;

    public class ApplicationExampleState : State<ApplicationState>
    {
        #region Private Variables
        [InseminatorAttributes.Inseminate]
        private ViewManager viewManager;

        [InseminatorAttributes.Inseminate]
        private MessageData messageData;

        [InseminatorAttributes.Inseminate(InstanceId = "CustomLoggerRed60")] 
        private ITextLogger testLogger;

        private ApplicationExampleView exampleView;

        [InseminatorAttributes.Surrogate]
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