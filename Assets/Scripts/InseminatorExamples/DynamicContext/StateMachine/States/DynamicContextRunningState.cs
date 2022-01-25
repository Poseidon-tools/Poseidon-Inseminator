namespace InseminatorExamples.DynamicContext.StateMachine.States
{
    using Core.ViewManager;
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using Poseidon.StateMachine;
    using RunningState.States;
    using Views;

    public class DynamicContextRunningState : State<DynamicContextStateType>
    {
        #region Public API
        public override DynamicContextStateType StateType => DynamicContextStateType.Running;
        #endregion

        #region Private Variables
        [InseminatorAttributes.Inseminate] private ITextLogger textLogger;
        [InseminatorAttributes.Inseminate] private ViewManager viewManager;

        private StateMachine<RunningStateType> stateMachine = new StateMachine<RunningStateType>(
            new State<RunningStateType>[]
            {
                new RunningStartingState(),
                new RunningUpdateState()
            }, RunningStateType.Starting);

        private DynamicContextStatusView dynamicContextStatusView;
        #endregion

        public override void OnEnter()
        {
            dynamicContextStatusView = viewManager.GetView<DynamicContextStatusView>();
            textLogger.LogMessage("Entered running state.", dynamicContextStatusView.StatusRenderer);
            stateMachine.Run();
        }

        public override void OnExit()
        {
            stateMachine.Dispose();
        }
    }
}