namespace trunkDI.Scripts.Example.ExampleInstallers
{
    using Core.ViewManager;
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ExampleSceneTrunkInstaller : TrunkInstaller
    {
        #region Inspector
        [SerializeField] private ViewManager sceneViewManager;
        [SerializeField] private MessageData sampleMessage;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(TrunkDependencyResolver trunkDependencyResolver)
        {
            trunkDependencyResolver.Bind<ITextLogger>(new TestLogger(), "TestLogger");
            trunkDependencyResolver.Bind<ITextLogger>(new GreenTextLogger(), "GreenTextLogger");
            trunkDependencyResolver.Bind<ITextLogger>(new CustomLogger(Color.red, 60), "CustomLoggerRed60");
            
            trunkDependencyResolver.Bind<ViewManager>(sceneViewManager);
            
            trunkDependencyResolver.Bind<MessageData>(sampleMessage);
        }
        #endregion
    }
}