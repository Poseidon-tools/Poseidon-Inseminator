namespace CubbyDI.Scripts.Example.ExampleInstallers
{
    using Core.ViewManager;
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ExampleSceneCubbyInstaller : CubbyInstaller
    {
        #region Inspector
        [SerializeField] private ViewManager sceneViewManager;
        [SerializeField] private MessageData sampleMessage;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(DependencyResolver dependencyResolver)
        {
            dependencyResolver.Bind<ITextLogger>(new TestLogger(), "TestLogger");
            dependencyResolver.Bind<ITextLogger>(new GreenTextLogger(), "GreenTextLogger");
            dependencyResolver.Bind<ITextLogger>(new CustomLogger(Color.red, 60), "CustomLoggerRed60");
            
            dependencyResolver.Bind<ViewManager>(sceneViewManager);
            
            dependencyResolver.Bind<MessageData>(sampleMessage);
        }
        #endregion
    }
}