namespace SDI.DynamicContext.StateMachine.States
{
    using System;
    using System.Collections.Generic;
    using Core.ViewManager;
    using Cysharp.Threading.Tasks;
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using MEC;
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

            Timing.RunCoroutine(WaitAndSwitch());
        }

        private IEnumerator<float> WaitAndSwitch(float timeInSeconds = 3f)
        {
            yield return Timing.WaitForSeconds(timeInSeconds);
            StateMachine.SwitchState(DynamicContextStateType.Running);
        }
    }
}