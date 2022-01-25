namespace InseminatorExamples.DynamicContext.StateMachine.States
{
    using System;
    using Core.ViewManager;
    using Cysharp.Threading.Tasks;
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using Poseidon.StateMachine;
    using Views;

    public class DynamicContextIdleState : State<DynamicContextStateType>
    {
        #region Public API
        public override DynamicContextStateType StateType => DynamicContextStateType.Idle;
        #endregion

        #region Private Variables
        [InseminatorAttributes.Inseminate] private ITextLogger textLogger;
        [InseminatorAttributes.Inseminate] private ViewManager viewManager;

        private DynamicContextStatusView dynamicContextStatusView;
        #endregion

        public override void OnEnter()
        {
            dynamicContextStatusView = viewManager.GetView<DynamicContextStatusView>();
            viewManager.SwitchView<DynamicContextStatusView>();
            textLogger.LogMessage("Entered idle state.", dynamicContextStatusView.StatusRenderer);
            WaitAndSwitch();
        }

        private async void WaitAndSwitch(float timeInSeconds = 3f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeInSeconds));
            StateMachine.SwitchState(DynamicContextStateType.Running);
        }
    }
}