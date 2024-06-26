﻿namespace InseminatorExamples.AppState.States
{
    using Core.ViewManager;
    using Inseminator.Scripts;
    using Poseidon.StateMachine;
    using Views;

    public class AppIntroState : State<ApplicationState>
    {
        #region Private Variables
        [InseminatorAttributes.Inseminate] private ViewManager viewManager;
        private ApplicationIntroView introView;
        #endregion
        #region Public Methods
        public override ApplicationState StateType => ApplicationState.Intro;

        public override void OnEnter()
        {
            introView = viewManager.GetView<ApplicationIntroView>();
            viewManager.SwitchView<ApplicationIntroView>();
            introView.NextButton.onClick.AddListener(OnNextHandler);
        }
        public override void OnExit()
        {
            introView.NextButton.onClick.RemoveListener(OnNextHandler);
        }
        #endregion
        #region Private Methods
        private void OnNextHandler()
        {
            StateMachine.SwitchState(ApplicationState.ExampleState);
        }
        #endregion
    }
}