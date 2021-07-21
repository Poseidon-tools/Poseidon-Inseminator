namespace trunkDI.Scripts.Example.ExampleInstallers
{
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ExampleGameObjectTrunkInstaller : TrunkInstaller
    {
        #region Inspector
        [SerializeField] private MessageData sampleMessage;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(TrunkDependencyResolver trunkDependencyResolver)
        {
            trunkDependencyResolver.Bind<MessageData>(sampleMessage);
        }
        #endregion
    }
}