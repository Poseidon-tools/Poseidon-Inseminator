namespace SDI.AppState.States
{
    using Core.ViewManager;
    using Inseminator.Scripts;
    using Inseminator.Scripts.Factory;
    using Poseidon.StateMachine;
    using Views;

    public class ApplicationOutroState : State<ApplicationState>
    {
        #region Private Variables
        [InseminatorAttributes.Injectable] private ViewManager viewManager;
        [InseminatorAttributes.Injectable] private InseminatorMonoFactory factory;
        private ApplicationOutroView outroView;

        private int maxItems = 4;
        private int itemCounter;
        #endregion
        #region Public Methods
        public override ApplicationState StateType => ApplicationState.Outro;

        public override void OnEnter()
        {
            outroView = viewManager.GetView<ApplicationOutroView>();
            viewManager.SwitchView<ApplicationOutroView>();
            RegisterListeners();
        }
        public override void OnExit()
        {
            UnregisterListeners();
        }
        #endregion
        #region Private Methods
        private void RegisterListeners()
        {
            outroView.SpawnButton.onClick.AddListener(OnSpawnItemRequestedHandler);
        }
        private void UnregisterListeners()
        {
            outroView.SpawnButton.onClick.RemoveListener(OnSpawnItemRequestedHandler);
        }
        private void OnSpawnItemRequestedHandler()
        {
            if (itemCounter == maxItems)
            {
                return;
            }
            var item = factory.Create(outroView.TemplateItem, outroView.ItemsContainer);
            itemCounter++;
            item.OverrideText($"{itemCounter}");
        }
        #endregion
    }
}