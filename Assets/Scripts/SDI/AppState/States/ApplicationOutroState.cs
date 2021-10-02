namespace SDI.AppState.States
{
    using Core.StateMachine;
    using Core.ViewManager;
    using PoseidonDI.Scripts;
    using PoseidonDI.Scripts.Factory;
    using Views;

    public class ApplicationOutroState : State<ApplicationState>
    {
        #region Private Variables
        [PoseidonAttributes.Injectable] private ViewManager viewManager;
        [PoseidonAttributes.Injectable] private PoseidonFactory factory;
        private ApplicationOutroView outroView;

        private int maxItems = 4;
        private int itemCounter;
        #endregion
        #region Public Methods
        public ApplicationOutroState(IStateManager<ApplicationState> stateManager, ApplicationState stateType) 
            : base(stateManager, stateType)
        {
        }
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