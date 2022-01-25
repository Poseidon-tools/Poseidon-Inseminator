namespace InseminatorExamples.DynamicContext
{
    using Core.ViewManager;
    using Inseminator.Scripts;
    using Inseminator.Scripts.Example;
    using Inseminator.Scripts.Installers;
    using Inseminator.Scripts.Resolver;
    using UnityEngine;

    public class DynamicContextInstaller : InseminatorInstaller
    {
        [InseminatorAttributes.Inseminate] private MessageData sceneMessage;
        [SerializeField] private ViewManager viewManager;
        public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
        {
            viewManager.Initialize();
            inseminatorDependencyResolver.Bind<ITextLogger>(new GreenTextLogger());

            inseminatorDependencyResolver.Bind<ViewManager>(viewManager);

            sceneMessage = ResolveInParent<MessageData>(inseminatorDependencyResolver.Parent);
            inseminatorDependencyResolver.Bind<MessageData>(sceneMessage);
        }
    }
}