namespace InseminatorExamples.DynamicContext.StateMachine.States.RunningState.States
{
    using System;
    using Core.ViewManager;
    using Cysharp.Threading.Tasks;
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using Poseidon.StateMachine;
    using Views;

    public class RunningStartingState : State<RunningStateType>
    {
        #region Public API
        public override RunningStateType StateType => RunningStateType.Starting;
        #endregion

        #region Private Variables
        [InseminatorAttributes.Inseminate] private ViewManager viewManager;
        [InseminatorAttributes.Inseminate] private ITextLogger textLogger;
        #endregion

        public override void OnEnter()
        {
            base.OnEnter();
            textLogger.LogMessage("Starting...", viewManager.GetView<DynamicContextStatusView>().StatusRenderer);
            WaitAndSwitch();
        }

        private async void WaitAndSwitch(float timeInSeconds = 3f)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeInSeconds));
            StateMachine.SwitchState(RunningStateType.Update);
        }
    }
}