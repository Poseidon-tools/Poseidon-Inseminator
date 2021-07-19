namespace CubbyDI.Scripts.Example.ExampleInstallers
{
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ExampleScriptableObjectCubbyInstaller : CubbyInstaller
    {
        #region Inspector
        [SerializeField] private MessageData sampleMessage;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(DependencyResolver dependencyResolver)
        {
            dependencyResolver.Bind<MessageData>(sampleMessage);
        }
        #endregion
    }
}