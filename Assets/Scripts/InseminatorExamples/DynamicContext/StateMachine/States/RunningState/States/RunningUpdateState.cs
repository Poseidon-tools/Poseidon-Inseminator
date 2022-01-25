namespace InseminatorExamples.DynamicContext.StateMachine.States.RunningState.States
{
    using Core.ViewManager;
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using Poseidon.StateMachine;
    using UnityEngine;
    using Views;

    public class RunningUpdateState : State<RunningStateType>, IUpdatable
    {
        #region Public API
        public override RunningStateType StateType => RunningStateType.Update;
        #endregion

        #region Private Variables
        [InseminatorAttributes.Inseminate] private ViewManager viewManager;
        [InseminatorAttributes.Inseminate] private ITextLogger textLogger;

        private DynamicContextStatusView view;
        private const float TIME_TO_UPDATE = 1f;
        private float lastUpdatedTime;
        private int counter;
        #endregion

        public override void OnEnter()
        {
            view = viewManager.GetView<DynamicContextStatusView>();
        }

        public void OnUpdate()
        {
            if (!(Time.time - lastUpdatedTime > TIME_TO_UPDATE)) return;
            lastUpdatedTime = Time.time;
            textLogger.LogMessage($"Update cycle: {++counter}", view.StatusRenderer);
        }
    }
}