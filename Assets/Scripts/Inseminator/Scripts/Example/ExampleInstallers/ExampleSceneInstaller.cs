namespace Inseminator.Scripts.Example.ExampleInstallers
{
    using Core.ViewManager;
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ExampleSceneInstaller : InseminatorInstaller
    {
        #region Inspector
        [SerializeField] private ViewManager sceneViewManager;
        [SerializeField] private MessageData sampleMessage;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(PoseidonDependencyResolver poseidonDependencyResolver)
        {
            poseidonDependencyResolver.Bind<ITextLogger>(new TestLogger(), "TestLogger");
            poseidonDependencyResolver.Bind<ITextLogger>(new GreenTextLogger(), "GreenTextLogger");
            poseidonDependencyResolver.Bind<ITextLogger>(new CustomLogger(Color.red, 60), "CustomLoggerRed60");
            
            poseidonDependencyResolver.Bind<ViewManager>(sceneViewManager);
            
            poseidonDependencyResolver.Bind<MessageData>(sampleMessage);
        }
        #endregion
    }
}