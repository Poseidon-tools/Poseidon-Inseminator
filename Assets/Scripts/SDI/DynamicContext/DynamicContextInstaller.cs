namespace SDI.DynamicContext
{
    using Core.ViewManager;
    using Inseminator.Scripts.Example;
    using Inseminator.Scripts.Installers;
    using Inseminator.Scripts.Resolver;
    using UnityEngine;

    public class DynamicContextInstaller : InseminatorInstaller
    {
        [SerializeField] private ViewManager viewManager;
        public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
        {
            viewManager.Initialize();
            inseminatorDependencyResolver.Bind<ITextLogger>(new GreenTextLogger());

            inseminatorDependencyResolver.Bind<ViewManager>(viewManager);
        }
    }
}