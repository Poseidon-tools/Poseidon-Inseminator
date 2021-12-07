namespace SDI.DynamicContext.StateMachine.States.RunningState.States
{
    using System.Collections.Generic;
    using Core.ViewManager;
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using MEC;
    using Poseidon.StateMachine;
    using Views;

    public class RunningStartingState : State<RunningStateType>
    {
        #region Public API
        public override RunningStateType StateType => RunningStateType.Starting;
        #endregion

        #region Private Variables
        [InseminatorAttributes.Injectable] private ViewManager viewManager;
        [InseminatorAttributes.Injectable] private ITextLogger textLogger;
        #endregion

        public override void OnEnter()
        {
            base.OnEnter();
            textLogger.LogMessage("Starting...", viewManager.GetView<DynamicContextStatusView>().StatusRenderer);
            Timing.RunCoroutine(WaitAndSwitch());
        }

        private IEnumerator<float> WaitAndSwitch(float timeInSeconds = 3f)
        {
            yield return Timing.WaitForSeconds(timeInSeconds);
            StateMachine.SwitchState(RunningStateType.Update);
        }
    }
}