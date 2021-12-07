namespace SDI.DynamicContext
{
    using Inseminator.Scripts.Example;
    using Inseminator.Scripts.Installers;
    using Inseminator.Scripts.Resolver;

    public class DynamicContextInstaller : InseminatorInstaller
    {
        public override void InstallBindings(InseminatorDependencyResolver inseminatorDependencyResolver)
        {
            inseminatorDependencyResolver.Bind<ITextLogger>(new GreenTextLogger());
        }
    }
}