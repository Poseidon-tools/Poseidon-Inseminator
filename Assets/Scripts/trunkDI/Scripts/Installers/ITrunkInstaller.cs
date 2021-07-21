namespace trunkDI.Scripts.Installers
{
    using Resolver;

    public interface ITrunkInstaller
    {
        void InstallBindings(TrunkDependencyResolver trunkDependencyResolver);
    }
}