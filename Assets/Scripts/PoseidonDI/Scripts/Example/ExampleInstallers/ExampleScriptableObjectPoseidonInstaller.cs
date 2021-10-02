namespace PoseidonDI.Scripts.Example.ExampleInstallers
{
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ExampleScriptableObjectPoseidonInstaller : PoseidonInstaller
    {
        #region Inspector
        [SerializeField] private MessageData sampleMessage;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(PoseidonDependencyResolver poseidonDependencyResolver)
        {
            poseidonDependencyResolver.Bind<MessageData>(sampleMessage);
        }
        #endregion
    }
}