namespace Inseminator.Scripts.Example.ExampleInstallers
{
    using Installers;
    using Resolver;
    using UnityEngine;

    public class ExampleGameObjectInstaller : InseminatorInstaller
    {
        #region Inspector
        [SerializeField] private MessageData sampleMessage;
        [SerializeField] private MessageData secondaryMessage;
        #endregion
        
        #region Public Methods
        public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
        {
            inseminatorDependencyResolver.Bind<MessageData>(sampleMessage, "SampleMessage");
            inseminatorDependencyResolver.Bind<MessageData>(secondaryMessage, "SecondaryMessage");
        }
        #endregion
    }
}